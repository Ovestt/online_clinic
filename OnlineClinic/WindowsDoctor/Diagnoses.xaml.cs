using OnlineClinic.WindowsReg;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
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
    /// Логика взаимодействия для Diagnoses.xaml
    /// </summary>
    public partial class Diagnoses : Window
    {
        private readonly int _doctorId;
        private readonly int _patientId;
        private readonly string _connectionString;

        public Diagnoses(int doctorId, int patientId, string connectionString)
        {
            InitializeComponent();
            _doctorId = doctorId;
            _patientId = patientId;
            _connectionString = connectionString;

        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DiagnosisNameTextBox.Text))
            {
                MessageBox.Show("Название диагноза не может быть пустым!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SaveButton.IsEnabled = false;
                PrintButton.IsEnabled = true;
                AddNewDiagnosis();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении диагноза: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddNewDiagnosis()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    INSERT INTO Diagnoses (PatientID, DoctorUserID, DiagnosisName, Description, DateCreated)
                    VALUES (@PatientID, @DoctorUserID, @DiagnosisName, @Description, GETDATE())";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PatientID", _patientId);
                    command.Parameters.AddWithValue("@DoctorUserID", _doctorId);
                    command.Parameters.AddWithValue("@DiagnosisName", DiagnosisNameTextBox.Text.Trim());
                    command.Parameters.AddWithValue("@Description",
                        string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ?
                        (object)DBNull.Value : DescriptionTextBox.Text.Trim());

                    command.ExecuteNonQuery();
                }
            }
        }
        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем полные данные пациента
                var patientData = GetPatientData(_patientId);
                string patientName = patientData.FullName;
                string patientBirthDate = patientData.BirthDate?.ToString("dd.MM.yyyy") ?? "не указана";
                string patientSnils = patientData.Snils ?? "не указан";

                string doctorName = GetDoctorFullName(_doctorId);

                // Создаем диалог печати
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() != true) return;

                // Создаем документ
                var document = new FlowDocument
                {
                    PageWidth = printDialog.PrintableAreaWidth,
                    PageHeight = printDialog.PrintableAreaHeight,
                    PagePadding = new Thickness(50),
                    FontFamily = new FontFamily("Times New Roman"),
                    FontSize = 14
                };

                // Добавляем заголовок
                document.Blocks.Add(new Paragraph(new Run("МЕДИЦИНСКОЕ ЗАКЛЮЧЕНИЕ"))
                {
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                });

                // Основная информация о пациенте
                var patientInfo = new Paragraph();
                patientInfo.Inlines.Add(new Run($"Пациент: {patientName}\n"));
                patientInfo.Inlines.Add(new Run($"Дата рождения: {patientBirthDate}\n"));
                patientInfo.Inlines.Add(new Run($"СНИЛС: {patientSnils}\n"));
                patientInfo.Inlines.Add(new Run($"Дата составления: {DateTime.Now:dd.MM.yyyy}\n"));
                patientInfo.Inlines.Add(new Run($"Врач: {doctorName}\n\n"));

                document.Blocks.Add(patientInfo);

                // Диагноз
                var diagnosisInfo = new Paragraph();
                diagnosisInfo.Inlines.Add(new Run("Диагноз: ")
                {
                    FontWeight = FontWeights.Bold
                });
                diagnosisInfo.Inlines.Add(new Run($"{DiagnosisNameTextBox.Text}\n\n"));
                document.Blocks.Add(diagnosisInfo);

                // Описание
                var descriptionInfo = new Paragraph();
                descriptionInfo.Inlines.Add(new Run("Клиническая картина и обоснование: ")
                {
                    FontWeight = FontWeights.Bold
                });
                descriptionInfo.Inlines.Add(new Run($"{DescriptionTextBox.Text}\n"));
                document.Blocks.Add(descriptionInfo);

                // Подпись
                var sign = new Paragraph(new Run($"\n\nВрач: _________________ /{doctorName}/"))
                {
                    TextAlignment = TextAlignment.Right,
                    Margin = new Thickness(0, 40, 0, 0)
                };
                document.Blocks.Add(sign);

                // Печатаем
                document.PagePadding = new Thickness(printDialog.PrintableAreaWidth * 0.1);
                printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator,
                                        "Медицинское заключение");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private class PatientData
        {
            public string FullName { get; set; }
            public DateTime? BirthDate { get; set; }
            public string Snils { get; set; }
        }

        private PatientData GetPatientData(int patientId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = @"SELECT 
                            LastName + ' ' + FirstName + ISNULL(' ' + MiddleName, '') as FullName,
                            BirthDate, 
                            SNILS
                          FROM Persons 
                          WHERE ID = @PatientID";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PatientID", patientId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new PatientData
                                {
                                    FullName = reader["FullName"].ToString(),
                                    BirthDate = reader["BirthDate"] as DateTime?,
                                    Snils = reader["SNILS"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при получении данных пациента: {ex.Message}");
            }

            // Возвращаем значения по умолчанию в случае ошибки
            return new PatientData
            {
                FullName = $"Пациент ID: {patientId}",
                BirthDate = null,
                Snils = "не указан"
            };
        }

        private string GetDoctorFullName(int doctorId)
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



        public void ExitButton_Click(object sender, EventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
