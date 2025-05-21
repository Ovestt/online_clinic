using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;

public class VisitsViewModel
{
    public ObservableCollection<VisitModel> Visits { get; set; } = new ObservableCollection<VisitModel>();

    public VisitsViewModel(int doctorId, int patientId, string connectionString)
    {
        LoadVisits(doctorId, patientId, connectionString);
    }

    private void LoadVisits(int doctorId, int patientId, string connectionString)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        mr.ReferralID,
                        mr.ReferralNumber,
                        mr.ReferralDate,
                        mr.Purpose,
                        mr.Speciality,
                        mr.ServiceType,
                        d.DiagnosisName,
                        p.FirstName + ' ' + p.LastName + ISNULL(' ' + p.MiddleName, '') as PatientName,
                        uc.FIO as DoctorName
                    FROM MedicalReferrals mr
                    LEFT JOIN Persons p ON mr.PatientID = p.ID
                    LEFT JOIN UserCredentialsDB uc ON mr.DoctorUserID = uc.UserID
                    LEFT JOIN Diagnoses d ON mr.DiagnosisID = d.DiagnosisID
                    WHERE mr.PatientID = @PatientID";

                if (doctorId > 0)
                {
                    query += " AND mr.DoctorUserID = @DoctorID";
                }

                query += " ORDER BY mr.ReferralDate DESC";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PatientID", patientId);

                if (doctorId > 0)
                {
                    command.Parameters.AddWithValue("@DoctorID", doctorId);
                }

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Visits.Add(new VisitModel
                    {
                        ReferralID = reader.GetInt32(0),
                        ReferralNumber = reader.GetString(1),
                        ReferralDate = reader.GetDateTime(2),
                        Purpose = reader.GetString(3),
                        Speciality = reader.IsDBNull(4) ? null : reader.GetString(4),
                        ServiceType = reader.GetString(5),
                        DiagnosisName = reader.IsDBNull(6) ? null : reader.GetString(6),
                        DoctorName = reader.IsDBNull(8) ? null : reader.GetString(8)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Ошибка при загрузке данных: {ex.Message}");
        }
    }
}

public class VisitModel
{
    public int ReferralID { get; set; }
    public string ReferralNumber { get; set; }
    public DateTime ReferralDate { get; set; }
    public string Purpose { get; set; }
    public string Speciality { get; set; }
    public string ServiceType { get; set; }
    public string DiagnosisName { get; set; }
    public string DoctorName { get; set; }
}