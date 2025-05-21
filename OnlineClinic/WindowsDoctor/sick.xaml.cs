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
    /// Логика взаимодействия для sick.xaml
    /// </summary>
    public partial class sick : Window
    {
        private readonly int _doctorId;
        private readonly int _patientId;
        private readonly string _connectionString;
        public sick(int doctorId, int patientId, string connectionString)
        {
            InitializeComponent();

            _doctorId = doctorId;
            _patientId = patientId;
            _connectionString = connectionString;

            LoadPatientInfo();
            SetDefaultValues();
            txtNumber.Text = $"SICK - {DateTime.Now:yyyyMMdd - HHmmss}";
            ;

        }
        private void LoadPatientInfo()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = "SELECT LastName, FirstName, MiddleName, BirthDate FROM Persons WHERE ID = @PatientID";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PatientID", _patientId);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string lastName = reader["LastName"].ToString();
                        string firstName = reader["FirstName"].ToString();
                        string middleName = reader["MiddleName"].ToString();
                        DateTime birthDate = Convert.ToDateTime(reader["BirthDate"]);

                        txtPatientInfo.Text = $"{lastName} {firstName} {middleName}, {birthDate:dd.MM.yyyy}";
                    }
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных пациента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SetDefaultValues()
        {
            dpStartDate.SelectedDate = DateTime.Today;
            dpEndDate.SelectedDate = DateTime.Today.AddDays(10);
            dpIssueDate.SelectedDate = DateTime.Now;
            cmbType.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;
        }
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;

            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"INSERT INTO SickLeaves (
                        Number, PatientID, DoctorUserID, Diagnosis, StartDate, EndDate, IssueDate, 
                        Status, Type, WorkPlace, Comments
                    ) VALUES (
                        @Number, @PatientID, @DoctorUserID, @Diagnosis, @StartDate, @EndDate, @IssueDate,
                        @Status, @Type, @WorkPlace, @Comments
                    )";

                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@Number", txtNumber.Text.Trim());
                    command.Parameters.AddWithValue("@PatientID", _patientId);
                    command.Parameters.AddWithValue("@DoctorUserID", _doctorId);
                    command.Parameters.AddWithValue("@Diagnosis", txtDiagnosis.Text.Trim());
                    command.Parameters.AddWithValue("@StartDate", dpStartDate.SelectedDate);
                    command.Parameters.AddWithValue("@EndDate", dpEndDate.SelectedDate);
                    command.Parameters.AddWithValue("@IssueDate", dpIssueDate.SelectedDate);
                    command.Parameters.AddWithValue("@Status", ((ComboBoxItem)cmbStatus.SelectedItem).Tag.ToString());
                    command.Parameters.AddWithValue("@Type", ((ComboBoxItem)cmbType.SelectedItem).Tag.ToString());
                    command.Parameters.AddWithValue("@WorkPlace", string.IsNullOrWhiteSpace(txtWorkPlace.Text) ? DBNull.Value : (object)txtWorkPlace.Text.Trim());
                    command.Parameters.AddWithValue("@Comments", string.IsNullOrWhiteSpace(txtComments.Text) ? DBNull.Value : (object)txtComments.Text.Trim());

                    int result = command.ExecuteNonQuery();

                    if (result > 0)
                    {
                        btnSave.IsEnabled = false;
                        PrintButton.IsEnabled = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении листа нетрудоспособности: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtNumber.Text))
            {
                MessageBox.Show("Введите номер листа нетрудоспособности", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtNumber.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDiagnosis.Text))
            {
                MessageBox.Show("Введите диагноз", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtDiagnosis.Focus();
                return false;
            }

            if (dpStartDate.SelectedDate == null || dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите даты начала и окончания", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
            {
                MessageBox.Show("Дата начала не может быть позже даты окончания", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (dpIssueDate.SelectedDate < dpStartDate.SelectedDate)
            {
                MessageBox.Show("Дата выдачи не может быть раньше даты начала", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

                var clinicName = new Paragraph(new Run("\"ЗдравЛайн\""))
                {
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                document.Blocks.Add(clinicName);

                // Заголовок
                var title = new Paragraph(new Run("ЛИСТ НЕТРУДОСПОСОБНОСТИ"))
                {
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 0, 0, 20)
                };
                document.Blocks.Add(title);

                // Основная информация
                var info = new Paragraph();
                info.Inlines.Add(new Run($"Номер: ") { FontWeight = FontWeights.Bold });
                info.Inlines.Add(new Run($"{txtNumber.Text}\n"));
                info.Inlines.Add(new Run($"Дата выдачи: ") { FontWeight = FontWeights.Bold });
                info.Inlines.Add(new Run($"{dpIssueDate.SelectedDate:dd.MM.yyyy}\n\n"));

                info.Inlines.Add(new Run("Пациент: ") { FontWeight = FontWeights.Bold });
                info.Inlines.Add(new Run($"{txtPatientInfo.Text}\n\n"));

                info.Inlines.Add(new Run("Диагноз: ") { FontWeight = FontWeights.Bold });
                info.Inlines.Add(new Run($"{txtDiagnosis.Text}\n\n"));

                info.Inlines.Add(new Run("Период нетрудоспособности: ") { FontWeight = FontWeights.Bold });
                info.Inlines.Add(new Run($"с {dpStartDate.SelectedDate:dd.MM.yyyy} по {dpEndDate.SelectedDate:dd.MM.yyyy}\n\n"));

                info.Inlines.Add(new Run("Тип: ") { FontWeight = FontWeights.Bold });
                info.Inlines.Add(new Run($"{((ComboBoxItem)cmbType.SelectedItem).Content}\n"));
                info.Inlines.Add(new Run("Статус: ") { FontWeight = FontWeights.Bold });
                info.Inlines.Add(new Run($"{((ComboBoxItem)cmbStatus.SelectedItem).Content}\n\n"));

                if (!string.IsNullOrWhiteSpace(txtWorkPlace.Text))
                {
                    info.Inlines.Add(new Run("Место работы: ") { FontWeight = FontWeights.Bold });
                    info.Inlines.Add(new Run($"{txtWorkPlace.Text}\n\n"));
                }

                document.Blocks.Add(info);

                // Комментарии (если есть)
                if (!string.IsNullOrWhiteSpace(txtComments.Text))
                {
                    var commentsTitle = new Paragraph(new Run("Особые указания:") { FontWeight = FontWeights.Bold });
                    document.Blocks.Add(commentsTitle);

                    var comments = new Paragraph(new Run(txtComments.Text));
                    document.Blocks.Add(comments);
                }

                // Подпись врача
                var doctorInfo = GetDoctorInfo(_doctorId);
                var signatureDate = new Paragraph(new Run($"Дата: {DateTime.Now:dd.MM.yyyy}"));
                document.Blocks.Add(signatureDate);

                var signature = new Paragraph();
                signature.Inlines.Add(new Run("Врач: _________________________ "));
                signature.Inlines.Add(new Run(doctorInfo));
                signature.TextAlignment = TextAlignment.Right;
                document.Blocks.Add(signature);

                // Печать документа
                printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator,
                                        "Лист нетрудоспособности");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetDoctorInfo(int doctorId)
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
            this.DialogResult = false;
            this.Close();
        }
    }
}
