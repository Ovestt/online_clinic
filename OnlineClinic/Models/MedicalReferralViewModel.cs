using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace YourApp.ViewModels
{
    public class MedicalReferralViewModel : BaseViewModel
    {
        private readonly string _connectionString;
        private readonly int _currentDoctorId;
        private readonly int _currentPatientId;

        public MedicalReferralModel Referral { get; } = new MedicalReferralModel();
        public ObservableCollection<string> ServiceTypes { get; } = new ObservableCollection<string>();
        public ObservableCollection<string> Specialities { get; } = new ObservableCollection<string>();
        public ObservableCollection<DiagnosisItem> Diagnoses { get; } = new ObservableCollection<DiagnosisItem>();

        public ICommand SaveCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand CancelCommand { get; }

        public MedicalReferralViewModel(int doctorId, int patientId, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            _currentDoctorId = doctorId;
            _currentPatientId = patientId;
            _connectionString = connectionString;

            Referral.PatientID = patientId;
            Referral.DoctorUserID = doctorId;
            Referral.ReferralDate = DateTime.Now;

            LoadInitialData();
            LoadPatientData();
            GenerateReferralNumber();

            SaveCommand = new RelayCommand(SaveReferral);
            PrintCommand = new RelayCommand(PrintReferral, () => Referral.ReferralID > 0);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void LoadInitialData()
        {
            ServiceTypes.Clear();
            ServiceTypes.Add("Консультация");
            ServiceTypes.Add("Диагностика");
            ServiceTypes.Add("Лечение");
            ServiceTypes.Add("Обследование");

            Specialities.Clear();
            Specialities.Add("Терапевт");
            Specialities.Add("Хирург");
            Specialities.Add("Невролог");
            Specialities.Add("Кардиолог");
        }

        private void LoadPatientData()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    var query = @"
                        SELECT 
                            p.FirstName, p.LastName, p.MiddleName, 
                            uc.FIO AS DoctorName
                        FROM Persons p
                        CROSS JOIN UserCredentialsDB uc
                        WHERE p.ID = @PatientID
                        AND uc.UserID = @DoctorID";

                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", _currentPatientId);
                        cmd.Parameters.AddWithValue("@DoctorID", _currentDoctorId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Referral.PatientName = $"{reader["LastName"]} {reader["FirstName"]} {reader["MiddleName"]}".Trim();
                                Referral.DoctorName = reader["DoctorName"].ToString();
                            }
                        }
                    }
                    Diagnoses.Clear();
                    var diagnosisQuery = @"
                        SELECT DiagnosisID, DiagnosisName 
                        FROM Diagnoses 
                        WHERE PatientID = @PatientID
                        ORDER BY DateCreated DESC";

                    using (var cmd = new SqlCommand(diagnosisQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", _currentPatientId);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Diagnoses.Add(new DiagnosisItem
                                {
                                    ID = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);

                Referral.PatientName = "Неизвестный пациент";
                Referral.DoctorName = "Неизвестный врач";
            }
        }

        private void GenerateReferralNumber()
        {
            Referral.ReferralNumber = $"REF-{DateTime.Now:yyyyMMdd-HHmmss}";
        }

        private void SaveReferral()
        {
            if (string.IsNullOrWhiteSpace(Referral.Purpose))
            {
                MessageBox.Show("Укажите цель направления", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = @"
                        INSERT INTO MedicalReferrals (
                            ReferralNumber, PatientID, DoctorUserID, ReferralDate,
                            Purpose, Speciality, ServiceType, DiagnosisID
                        ) VALUES (
                            @ReferralNumber, @PatientID, @DoctorUserID, @ReferralDate,
                            @Purpose, @Speciality, @ServiceType, @DiagnosisID
                        );
                        SELECT SCOPE_IDENTITY();";

                    connection.Open();
                    using (var cmd = new SqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@ReferralNumber", Referral.ReferralNumber);
                        cmd.Parameters.AddWithValue("@PatientID", Referral.PatientID);
                        cmd.Parameters.AddWithValue("@DoctorUserID", Referral.DoctorUserID);
                        cmd.Parameters.AddWithValue("@ReferralDate", Referral.ReferralDate);
                        cmd.Parameters.AddWithValue("@Purpose", Referral.Purpose);
                        cmd.Parameters.AddWithValue("@Speciality", Referral.Speciality ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ServiceType", Referral.ServiceType ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DiagnosisID", Referral.DiagnosisID ?? (object)DBNull.Value);

                        var newId = cmd.ExecuteScalar();
                        if (newId != null)
                        {
                            Referral.ReferralID = Convert.ToInt32(newId);
                            MessageBox.Show("Направление успешно сохранено", "Успех",
                                MessageBoxButton.OK, MessageBoxImage.Information);


                            CommandManager.InvalidateRequerySuggested();
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                MessageBox.Show($"Ошибка базы данных: {sqlEx.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintReferral()
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
                    PagePadding = new Thickness(50),
                    FontFamily = new FontFamily("Times New Roman"),
                    FontSize = 14
                };

                // Добавляем содержимое
                document.Blocks.Add(new Paragraph(new Run("МЕДИЦИНСКОЕ НАПРАВЛЕНИЕ"))
                {
                    FontSize = 18,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center
                });

                // Основная информация
                var info = new Paragraph();
                info.Inlines.Add(new Run($"Номер: {Referral.ReferralNumber}\n"));
                info.Inlines.Add(new Run($"Дата: {Referral.ReferralDate:dd.MM.yyyy}\n"));
                info.Inlines.Add(new Run($"Пациент: {Referral.PatientName}\n"));
                info.Inlines.Add(new Run($"Врач: {Referral.DoctorName}\n\n"));
                info.Inlines.Add(new Run($"Цель направления: {Referral.Purpose}\n"));

                if (!string.IsNullOrEmpty(Referral.Speciality))
                    info.Inlines.Add(new Run($"Специальность: {Referral.Speciality}\n"));

                info.Inlines.Add(new Run($"Тип услуги: {Referral.ServiceType}\n"));

                if (Referral.DiagnosisID.HasValue)
                {
                    var diagnosis = Diagnoses.FirstOrDefault(d => d.ID == Referral.DiagnosisID);
                    if (diagnosis != null)
                        info.Inlines.Add(new Run($"Диагноз: {diagnosis.Name}\n"));
                }

                document.Blocks.Add(info);

                // Подпись
                var sign = new Paragraph(new Run($"\n\nВрач: _________________ /{Referral.DoctorName}/"))
                {
                    TextAlignment = TextAlignment.Right,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                document.Blocks.Add(sign);

                // Печатаем
                document.PagePadding = new Thickness(printDialog.PrintableAreaWidth * 0.1);
                printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator,
                                        "Медицинское направление");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel()
        {

            Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.DataContext == this)
                    {
                        window.Close();
                        break;
                    }
                }
            });
        }
    }

    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}