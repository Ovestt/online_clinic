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
    public partial class DiagnosesHistory : Window
    {
        private readonly int _doctorId;
        private readonly int _patientId;
        private readonly string _connectionString;

        public DiagnosesHistory(int doctorId, int patientId, string connectionString)
        {
            InitializeComponent();

            _doctorId = doctorId;
            _patientId = patientId;
            _connectionString = connectionString;

            Loaded += PatientDiagnosesWindow_Loaded;
        }
        private void PatientDiagnosesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDiagnosesData();
        }

        private void LoadDiagnosesData()
        {
            try
            {
                var diagnoses = GetDiagnosesForPatient();
                DiagnosesGrid.ItemsSource = diagnoses;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private List<Diagnosis> GetDiagnosesForPatient()
        {
            var diagnoses = new List<Diagnosis>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT d.DiagnosisID, d.DiagnosisName, d.Description, d.DateCreated
                    FROM Diagnoses d
                    WHERE d.PatientID = @PatientID
                    ORDER BY d.DateCreated DESC";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PatientID", _patientId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            diagnoses.Add(new Diagnosis
                            {
                                DiagnosisID = reader.GetInt32(0),
                                DiagnosisName = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                                DateCreated = reader.GetDateTime(3)
                            });
                        }
                    }
                }
            }

            return diagnoses;
        }
        private void ExitButton_Click(object sender, EventArgs e) 
        {
            DialogResult = false;
            Close();
        }
    }

    public class Diagnosis
    {
        public int DiagnosisID { get; set; }
        public string DiagnosisName { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; }
    }
}

