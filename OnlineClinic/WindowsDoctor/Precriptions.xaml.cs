using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OnlineClinic.WindowsDoctor
{
    /// <summary>
    /// Логика взаимодействия для Precriptions.xaml
    /// </summary>
    public partial class Precriptions : Window
    {
        private readonly int _doctorId;
        private readonly int _patientId;
        private readonly string _connectionString;
        public Precriptions(int doctorId, int patientId, string connectionString)
        {
            InitializeComponent();

            _doctorId = doctorId;
            _patientId = patientId;
            _connectionString = connectionString;

            dpIssueDate.SelectedDate = DateTime.Now;
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMedicationName.Text))
            {
                MessageBox.Show("Введите название лекарства", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMedicationName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDosage.Text))
            {
                MessageBox.Show("Введите дозировку", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDosage.Focus();
                return;
            }

            if (dpExpiryDate.SelectedDate == null)
            {
                MessageBox.Show("Укажите срок действия рецепта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                dpExpiryDate.Focus();
                return;
            }

            if (dpExpiryDate.SelectedDate < dpIssueDate.SelectedDate)
            {
                MessageBox.Show("Срок действия не может быть раньше даты выписки", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO Prescriptions 
                                    (DoctorUserID, PatientID, MedicationName, Dosage, IssueDate, ExpiryDate) 
                                    VALUES 
                                    (@DoctorID, @PatientID, @MedicationName, @Dosage, @IssueDate, @ExpiryDate)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DoctorID", _doctorId);
                        command.Parameters.AddWithValue("@PatientID", _patientId);
                        command.Parameters.AddWithValue("@MedicationName", txtMedicationName.Text.Trim());
                        command.Parameters.AddWithValue("@Dosage", txtDosage.Text.Trim());
                        command.Parameters.AddWithValue("@IssueDate", dpIssueDate.SelectedDate);
                        command.Parameters.AddWithValue("@ExpiryDate", dpExpiryDate.SelectedDate);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            btnSave.IsEnabled = false;
                            PrintButton.IsEnabled = true;
                        }
                        else
                        {
                            MessageBox.Show("Не удалось сохранить рецепт", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем данные пациента и врача
                string patientName = GetPatientName(_patientId);
                string doctorName = GetDoctorName(_doctorId);
                string currentDate = dpIssueDate.SelectedDate?.ToString("dd.MM.yyyy") ?? DateTime.Now.ToString("dd.MM.yyyy");
                string expiryDate = dpExpiryDate.SelectedDate?.ToString("dd.MM.yyyy") ?? "не указан";

                // Создаем диалог печати
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() != true) return;

                // Создаем документ
                var document = new FlowDocument
                {
                    PageWidth = printDialog.PrintableAreaWidth,
                    PageHeight = printDialog.PrintableAreaHeight,
                    PagePadding = new Thickness(40),
                    FontFamily = new FontFamily("Times New Roman"),
                    FontSize = 14,
                    ColumnWidth = printDialog.PrintableAreaWidth - 80
                };

                // Шапка документа
                var header = new Paragraph(new Run("УЧРЕЖДЕНИЕ ЗДРАВООХРАНЕНИЯ"))
                {
                    FontSize = 12,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 5)
                };
                document.Blocks.Add(header);

                var clinicName = new Paragraph(new Run("\"Здравлайн\""))
                {
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                document.Blocks.Add(clinicName);

                // Заголовок "РЕЦЕПТ"
                var title = new Paragraph(new Run("РЕЦЕПТ"))
                {
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                document.Blocks.Add(title);

                // Информация о рецепте
                var prescriptionInfo = new Paragraph();
                prescriptionInfo.Inlines.Add(new Run($"№ ______ от {currentDate}\n\n"));
                prescriptionInfo.Inlines.Add(new Run("Пациент: ") { FontWeight = FontWeights.Bold });
                prescriptionInfo.Inlines.Add(new Run($"{patientName}\n"));
                prescriptionInfo.Inlines.Add(new Run("Лечащий врач: ") { FontWeight = FontWeights.Bold });
                prescriptionInfo.Inlines.Add(new Run($"{doctorName}\n\n"));
                document.Blocks.Add(prescriptionInfo);

                // Основная часть рецепта
                var medicationInfo = new Paragraph();
                medicationInfo.Inlines.Add(new Run("Наименование препарата: ") { FontWeight = FontWeights.Bold });
                medicationInfo.Inlines.Add(new Run($"{txtMedicationName.Text}\n"));
                medicationInfo.Inlines.Add(new Run("Дозировка: ") { FontWeight = FontWeights.Bold });
                medicationInfo.Inlines.Add(new Run($"{txtDosage.Text}\n"));
                medicationInfo.Inlines.Add(new Run("Срок действия: ") { FontWeight = FontWeights.Bold });
                medicationInfo.Inlines.Add(new Run($"до {expiryDate}\n\n"));
                document.Blocks.Add(medicationInfo);

                // Особые указания
                var instructions = new Paragraph(new Run("Особые указания: ___________________________________\n"))
                {
                    Margin = new Thickness(0, 10, 0, 0)
                };
                document.Blocks.Add(instructions);

                // Подпись врача
                var signature = new Paragraph();
                signature.Inlines.Add(new Run($"\n\nДата: {currentDate}\n"));
                signature.Inlines.Add(new Run("Врач: _________________________ "));
                signature.Inlines.Add(new Run(doctorName + "\n"));

                var mp = new Paragraph(new Run("М.П."))
                {
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                };

                document.Blocks.Add(signature);
                document.Blocks.Add(mp);

                // Печать документа
                printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator,
                                        "Медицинский рецепт");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати рецепта: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetPatientName(int patientId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"SELECT LastName + ' ' + FirstName + ISNULL(' ' + MiddleName, '') 
                          FROM Persons WHERE ID = @PatientID";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PatientID", patientId);
                        return command.ExecuteScalar()?.ToString() ?? $"Пациент ID: {patientId}";
                    }
                }
            }
            catch
            {
                return $"Пациент ID: {patientId}";
            }
        }

        private string GetDoctorName(int doctorId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT FIO FROM UserCredentialsDB WHERE UserID = @DoctorID";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DoctorID", doctorId);
                        return command.ExecuteScalar()?.ToString() ?? $"Врач ID: {doctorId}";
                    }
                }
            }
            catch
            {
                return $"Врач ID: {doctorId}";
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
