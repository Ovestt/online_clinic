using OnlineClinic.WindowsReg;
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
            
        }
        public void ExitButton_Click(object sender, EventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
