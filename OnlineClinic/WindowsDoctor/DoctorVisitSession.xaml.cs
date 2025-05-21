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
    public partial class DoctorVisitSession : Window
    {
        private readonly int _visitId;
        private readonly int _doctorId;
        private readonly string _connectionString;
        private int _patientId;

        public DoctorVisitSession(int visitId, int patientId, int doctorId, string connectionString)
        {
            InitializeComponent();

            _visitId = visitId;
            _patientId = patientId; 
            _doctorId = doctorId;
            _connectionString = connectionString;

            LoadVisitData();
            LoadPatientData();
        }


        private void LoadPatientData()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT 
                    p.LastName + ' ' + p.FirstName + ' ' + p.MiddleName as PatientName,
                    CONVERT(varchar, p.BirthDate, 104) as BirthDate,
                    CASE WHEN p.Gender = 'M' THEN 'Мужской' ELSE 'Женский' END as Gender,
                    p.PhoneNumber,
                    p.RegistrationAddress,
                    p.SNILS
                FROM Persons p
                WHERE p.ID = @PatientId";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@PatientId", _patientId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Заполнение информации о пациенте
                                FIO.Text = reader["PatientName"].ToString();
                                DateBirthday.Text = $"Дата рождения: {reader["BirthDate"]}";
                                GenderTxt.Text = $"Пол: {reader["Gender"]}";
                                Phone.Text = $"Телефон: {reader["PhoneNumber"]}";
                                address.Text = $"Адрес: {reader["RegistrationAddress"]}";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных пациента: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
        private void LoadVisitData()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT 
                    v.VisitDateTime,
                    v.Notes
                FROM Visits v
                WHERE v.VisitID = @VisitId";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@VisitId", _visitId);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Заполнение информации о приеме
                                VisitDateTimeText.Text = Convert.ToDateTime(reader["VisitDateTime"]).ToString("dd.MM.yyyy HH:mm");
                                VisitNotesTextBox.Text = reader["Notes"]?.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных визита: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
        public void CloseSeans_Click(object sender, EventArgs e)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Проверяем, не закрыт ли уже визит
                    string checkQuery = "SELECT EndDateTime FROM Visits WHERE VisitID = @VisitId";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@VisitId", _visitId);
                        var endTime = checkCommand.ExecuteScalar();

                        if (endTime != null && endTime != DBNull.Value)
                        {
                            MessageBox.Show("Этот визит уже был завершен ранее", "Информация",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                            return;
                        }
                    }

                    // Обновляем время завершения визита
                    string updateQuery = @"
                UPDATE Visits 
                SET 
                    EndDateTime = GETDATE(),
                    Notes = @Notes
                WHERE VisitID = @VisitId";

                    using (var updateCommand = new SqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@VisitId", _visitId);
                        updateCommand.Parameters.AddWithValue("@Notes", VisitNotesTextBox.Text ?? string.Empty);

                        int rowsAffected = updateCommand.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            this.DialogResult = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Не удалось завершить визит", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при завершении визита: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public void ExitButton_Click(object sender, EventArgs e)
        {
            DialogResult = false;
            Close();
        }
        public void DoctorHistory_Click(object sender, EventArgs e)
        {
            var editWindow = new DoctorHistory(_doctorId, _patientId, _connectionString);
            editWindow.ShowDialog();
        }
        public void DiagnosesHistory_Click(object sender, EventArgs e)
        {
            var editWindow = new DiagnosesHistory(_doctorId, _patientId, _connectionString);
            editWindow.ShowDialog();
        }
        public void Presctiptions_Click(object sender, EventArgs e)
        {
            var editWindow = new PresriptionsHistory(_doctorId, _patientId, _connectionString);
            editWindow.ShowDialog();
        }
        public void SendReferrals_Click(object sender, EventArgs e)
        {
            var editWindow = new Referrals(_doctorId, _patientId, _connectionString);
            editWindow.ShowDialog();
        }
        public void SendDiagnoses_Click(object sender, EventArgs e)
        {
            var editWindow = new Diagnoses(_doctorId, _patientId, _connectionString);
            editWindow.ShowDialog();
        }
        public void SentPrecriptons_Click(object sender, EventArgs e)
        {
            var editWindow = new Precriptions(_doctorId, _patientId, _connectionString);
            editWindow.ShowDialog();
        }        
        public void SentNotWork_Click(object sender, EventArgs e)
        {
            var editWindow = new sick(_doctorId, _patientId, _connectionString);
            editWindow.ShowDialog();
        }
    }
}
