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
    /// Логика взаимодействия для PresriptionsHistory.xaml
    /// </summary>
    public partial class PresriptionsHistory : Window
    {
        private readonly int _doctorId;
        private readonly int _patientId;
        private readonly string _connectionString;

        public PresriptionsHistory(int doctorId, int patientId, string connectionString)
        {
            InitializeComponent();
            _doctorId = doctorId;
            _patientId = patientId;
            _connectionString = connectionString;

            Loaded += PatientPrescriptionsWindow_Loaded;
        }
        public void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void PatientPrescriptionsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPrescriptionsData();
        }

        private void LoadPrescriptionsData()
        {
            try
            {
                var prescriptions = GetPrescriptionsForPatient();
                PrescriptionsGrid.ItemsSource = prescriptions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<Prescription> GetPrescriptionsForPatient()
        {
            var prescriptions = new List<Prescription>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT p.PrescriptionID, p.MedicationName, p.Dosage, p.IssueDate, p.ExpiryDate
                    FROM Prescriptions p
                    WHERE p.PatientID = @PatientID
                    ORDER BY p.IssueDate DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PatientID", _patientId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            prescriptions.Add(new Prescription
                            {
                                PrescriptionID = reader.GetInt32(0),
                                MedicationName = reader.GetString(1),
                                Dosage = reader.GetString(2),
                                IssueDate = reader.GetDateTime(3),
                                ExpiryDate = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4)
                            });
                        }
                    }
                }
            }

            return prescriptions;
        }
    }

    public class Prescription
    {
        public int PrescriptionID { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}

