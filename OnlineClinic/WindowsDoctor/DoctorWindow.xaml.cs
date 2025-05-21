using OnlineClinic.WindowsDoctor;
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
using System.Xml.Linq;

namespace OnlineClinic
{
    public partial class DoctorWindow : Window
    {
        private readonly UpcomingVisitsRepository _visitsRepository;
        private readonly int _doctorId;
        private readonly string _doctorName;
        private List<UpcomingVisit> _upcomingVisits;
        string _connectionString;
        public DoctorWindow(int UserID)
        {
            _doctorId = UserID;
            string connectionString = Properties.Settings.Default.masterConnectionString;
            _connectionString = Properties.Settings.Default.masterConnectionString;
            InitializeComponent();

            _visitsRepository = new UpcomingVisitsRepository(connectionString);

            LoadUpcomingVisits();

            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(1);
            timer.Tick += (s, e) => RefreshVisits();
            timer.Start();
        }
        private void LoadUpcomingVisits()
        {
            try
            {
                _upcomingVisits = _visitsRepository.GetUpcomingVisits(_doctorId);
                VisitsDataGrid.ItemsSource = _upcomingVisits;

                UpdateStatusText();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
        private void RefreshVisits()
        {
            // Обновляем только если окно активно
            if (IsActive)
            {
                LoadUpcomingVisits();
            }
        }
        private void UpdateStatusText()
        {
            int total = _upcomingVisits.Count;
            int overdue = _upcomingVisits.Count(v => v.TimeUntilVisit == "Просрочено");
            int now = _upcomingVisits.Count(v => v.TimeUntilVisit == "Сейчас");

            StatusText.Text = $"Всего: {total} | Ожидают: {total - overdue - now} | Сейчас: {now} | Просрочено: {overdue}";
            RecordCountText.Text = $"Последнее обновление: {DateTime.Now:HH:mm}";
        }
        private void StartVisit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int visitId)
            {
                // Получаем данные о визите
                var visit = _upcomingVisits.FirstOrDefault(v => v.VisitID == visitId);
                if (visit != null)
                {
                    var visitWindow = new DoctorVisitSession(
                        visitId: visit.VisitID,
                        patientId: visit.PatientID,
                        doctorId: _doctorId,
                        connectionString: _connectionString);

                    if (visitWindow.ShowDialog() == true)
                    {
                        LoadUpcomingVisits(); // Обновляем список после завершения приема
                    }
                }
            }
        }
        private void CancelVisit_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int visitId)
            {
                var result = MessageBox.Show("Вы уверены, что хотите отменить этот визит?",
                                           "Подтверждение отмены",
                                           MessageBoxButton.YesNo,
                                           MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        CancelVisit(visitId);
                        LoadUpcomingVisits();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при отмене визита: {ex.Message}",
                                      "Ошибка",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Error);
                    }
                }
            }
        }
        private void CancelVisit(int visitId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "DELETE FROM Visits WHERE VisitID = @VisitId";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@VisitId", visitId);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ExitButton.ExitButton_Click(sender, e);
        }
    }
}
