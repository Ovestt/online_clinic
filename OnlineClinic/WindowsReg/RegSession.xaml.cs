using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
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

namespace OnlineClinic.WindowsReg
{
    public partial class RegSession : Window
    {
        private readonly Person _person;
        private readonly PersonRepository _personRepository;
        private string connectionString = Properties.Settings.Default.masterConnectionString;
        private List<Doctor> doctors = new List<Doctor>();
        private List<string> allTimeSlots = new List<string>();
        private int currentPatientId;

        public RegSession(Person person, PersonRepository personRepository)
        {
            InitializeComponent();
            InitializeTimeSlots();
            LoadSpecialities();

            _person = person;
            _personRepository = personRepository;
            currentPatientId = _person.ID;
        }
        private void InitializeTimeSlots()
        {
            // Утренние слоты (8:00-12:30)
            DateTime morningStart = DateTime.Today.AddHours(8);
            while (morningStart <= DateTime.Today.AddHours(12).AddMinutes(30))
            {
                allTimeSlots.Add(morningStart.ToString("HH:mm"));
                morningStart = morningStart.AddMinutes(30);
            }

            // Вечерние слоты (14:00-17:30)
            DateTime eveningStart = DateTime.Today.AddHours(14);
            while (eveningStart <= DateTime.Today.AddHours(17).AddMinutes(30))
            {
                allTimeSlots.Add(eveningStart.ToString("HH:mm"));
                eveningStart = eveningStart.AddMinutes(30);
            }
        }
        private void LoadSpecialities()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT Speciality FROM UserCredentialsDB WHERE Role = 'врач' ORDER BY Speciality";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                var specialities = new List<dynamic>();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    specialities.Add(new
                    {
                        Speciality = reader["Speciality"].ToString()
                    });
                }

                cmbSpeciality.ItemsSource = specialities;
            }
        }
        private void dpVisitDate_CalendarOpened(object sender, RoutedEventArgs e)
        {
            // Блокируем прошедшие даты и выходные
            var calendar = dpVisitDate.Template.FindName("PART_Calendar", dpVisitDate) as Calendar;
            if (calendar != null)
            {
                calendar.BlackoutDates.AddDatesInPast();

                // Блокируем выходные (суббота и воскресенье)
                DateTime startDate = DateTime.Today;
                DateTime endDate = startDate.AddMonths(3); // На 3 месяца вперед

                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                    {
                        calendar.BlackoutDates.Add(new CalendarDateRange(date));
                    }
                }
            }
        }
        private void dpVisitDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpVisitDate.SelectedDate.HasValue)
            {
                // Проверяем, что дата не в прошлом и не выходной
                if (dpVisitDate.SelectedDate.Value < DateTime.Today)
                {
                    MessageBox.Show("Нельзя выбрать прошедшую дату");
                    dpVisitDate.SelectedDate = DateTime.Today;
                    return;
                }

                if (dpVisitDate.SelectedDate.Value.DayOfWeek == DayOfWeek.Saturday ||
                    dpVisitDate.SelectedDate.Value.DayOfWeek == DayOfWeek.Sunday)
                {
                    MessageBox.Show("В выходные дни прием не ведется");
                    dpVisitDate.SelectedDate = GetNextWorkDay(dpVisitDate.SelectedDate.Value);
                    return;
                }

                if (cmbDoctor.SelectedItem != null)
                {
                    Doctor selectedDoctor = (Doctor)cmbDoctor.SelectedItem;
                    UpdateAvailableTimeSlots(selectedDoctor.UserID, dpVisitDate.SelectedDate.Value);
                }
            }

        }
        private DateTime GetNextWorkDay(DateTime date)
        {
            do
            {
                date = date.AddDays(1);
            } while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday);

            return date;
        }
        private void cmbSpeciality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbDoctor.ItemsSource = null;
            cmbDoctor.Text = null;
            dpVisitDate.SelectedDate = null;
            if (cmbSpeciality.SelectedItem == null) return;

            dynamic selectedSpeciality = cmbSpeciality.SelectedItem;
            string speciality = selectedSpeciality.Speciality;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT UserID, FIO, Speciality FROM UserCredentialsDB " +
                                 "WHERE Role = 'врач' AND Speciality = @Speciality " +
                                 "ORDER BY FIO";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Speciality", speciality);
                    connection.Open();

                    doctors.Clear();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        doctors.Add(new Doctor
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            FIO = reader["FIO"].ToString(),
                            Speciality = reader["Speciality"].ToString()
                        });
                    }
                }

                cmbDoctor.ItemsSource = doctors;
                cmbDoctor.IsEnabled = true;
                cmbTime.IsEnabled = false;
                cmbTime.SelectedItem = null;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки врачей: {ex.Message}");
            }
        }
        private void cmbDoctor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbDoctor.SelectedItem != null && dpVisitDate.SelectedDate.HasValue)
            {
                Doctor selectedDoctor = (Doctor)cmbDoctor.SelectedItem;
                UpdateAvailableTimeSlots(selectedDoctor.UserID, dpVisitDate.SelectedDate.Value);
            }

        }
        private void UpdateAvailableTimeSlots(int doctorId, DateTime selectedDate)
        {
            try
            {
                List<string> bookedTimes = new List<string>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT CONVERT(TIME, VisitDateTime) AS BookedTime 
                                   FROM Visits 
                                   WHERE EmployeeID = @DoctorId 
                                   AND CONVERT(DATE, VisitDateTime) = @SelectedDate";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DoctorId", doctorId);
                    command.Parameters.AddWithValue("@SelectedDate", selectedDate.Date);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        TimeSpan time = (TimeSpan)reader["BookedTime"];
                        bookedTimes.Add(time.ToString(@"hh\:mm"));
                    }
                }

                var availableSlots = allTimeSlots.Except(bookedTimes).ToList();
                cmbTime.ItemsSource = availableSlots;
                cmbTime.IsEnabled = availableSlots.Any();

                if (!availableSlots.Any())
                {
                    MessageBox.Show("На выбранную дату нет свободных слотов для записи.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке доступного времени: {ex.Message}");
            }
        }
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateAppointment()) return;

            Doctor selectedDoctor = (Doctor)cmbDoctor.SelectedItem;
            string selectedTime = cmbTime.SelectedItem.ToString();
            DateTime visitDate = dpVisitDate.SelectedDate.Value;

            // Создаем полную дату и время
            TimeSpan time = TimeSpan.Parse(selectedTime);
            DateTime visitDateTime = visitDate.Date.Add(time);

            try
            {
                // Проверяем, не занято ли время (на случай параллельных записей)
                if (IsTimeSlotBooked(selectedDoctor.UserID, visitDateTime))
                {
                    MessageBox.Show("Это время только что было занято. Пожалуйста, выберите другое время.");
                    UpdateAvailableTimeSlots(selectedDoctor.UserID, visitDate);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Visits (PatientID, EmployeeID, VisitDateTime, Notes)
                                   VALUES (@PatientID, @EmployeeID, @VisitDateTime, @Notes)";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PatientID", currentPatientId);
                    command.Parameters.AddWithValue("@EmployeeID", selectedDoctor.UserID);
                    command.Parameters.AddWithValue("@VisitDateTime", visitDateTime);
                    command.Parameters.AddWithValue("@Notes", txtNotes.Text ?? string.Empty);

                    connection.Open();
                    int result = command.ExecuteNonQuery();

                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании записи: {ex.Message}");
            }
        }
        private bool ValidateAppointment()
        {
            if (!dpVisitDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату приема");
                return false;
            }

            if (cmbSpeciality.SelectedItem == null)
            {
                MessageBox.Show("Выберите специальность врача");
                return false;
            }

            if (cmbDoctor.SelectedItem == null)
            {
                MessageBox.Show("Выберите врача");
                return false;
            }

            if (cmbTime.SelectedItem == null)
            {
                MessageBox.Show("Выберите время приема");
                return false;
            }

            return true;
        }
        private bool IsTimeSlotBooked(int doctorId, DateTime visitDateTime)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT COUNT(*) FROM Visits 
                                   WHERE EmployeeID = @DoctorId 
                                   AND VisitDateTime = @VisitDateTime";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DoctorId", doctorId);
                    command.Parameters.AddWithValue("@VisitDateTime", visitDateTime);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();

                    return count > 0;
                }
            }
            catch
            {
                return true; // В случае ошибки считаем, что слот занят
            }
        }
        
        
        public void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

    }
    public class Doctor
    {
        public int UserID { get; set; }
        public string FIO { get; set; }
        public string Speciality { get; set; }
    }
}
