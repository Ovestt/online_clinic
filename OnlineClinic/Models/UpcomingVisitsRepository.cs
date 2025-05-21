using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class UpcomingVisitsRepository
{
    private readonly string _connectionString;

    public UpcomingVisitsRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<UpcomingVisit> GetUpcomingVisits(int doctorId)
    {
        var visits = new List<UpcomingVisit>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"
                SELECT 
                    v.VisitID,
                    v.PatientID,
                    p.LastName + ' ' + p.FirstName + ' ' + ISNULL(p.MiddleName, '') as PatientFullName,
                    v.VisitDateTime,
                    v.EndDateTime,
                    v.Notes,
                    u.Speciality,
                    p.PhoneNumber as PatientPhone,
                    CONVERT(varchar, p.BirthDate, 104) as PatientBirthDate
                FROM Visits v
                JOIN Persons p ON v.PatientID = p.ID
                JOIN UserCredentialsDB u ON v.EmployeeID = u.UserID
                WHERE v.EmployeeID = @DoctorId
                AND v.EndDateTime IS NULL -- Только незавершенные визиты
                AND v.VisitDateTime >= DATEADD(MINUTE, -30, GETDATE()) -- Визиты не старше 30 минут
                ORDER BY v.VisitDateTime ASC";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DoctorId", doctorId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var visit = new UpcomingVisit
                        {
                            VisitID = Convert.ToInt32(reader["VisitID"]),
                            PatientID = Convert.ToInt32(reader["PatientID"]),
                            PatientFullName = reader["PatientFullName"].ToString(),
                            VisitDateTime = Convert.ToDateTime(reader["VisitDateTime"]),
                            Notes = reader["Notes"]?.ToString(),
                            Speciality = reader["Speciality"].ToString(),
                            PatientPhone = reader["PatientPhone"]?.ToString(),
                            PatientBirthDate = reader["PatientBirthDate"].ToString()
                        };

                        visits.Add(visit);
                    }
                }
            }
        }

        return visits;
    }
}