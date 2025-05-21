using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class VisitRepository
{
    private readonly string _connectionString;

    public VisitRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<VisitHistory> GetPatientHistory(int patientId)
    {
        var visits = new List<VisitHistory>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"
                SELECT 
                    v.VisitID,
                    u.FIO as DoctorName,
                    u.Speciality,
                    v.VisitDateTime,
                    v.Notes,
                    p.LastName + ' ' + p.FirstName + ' ' + p.MiddleName as PatientName
                FROM Visits v
                JOIN UserCredentialsDB u ON v.EmployeeID = u.UserID
                JOIN Persons p ON v.PatientID = p.ID
                WHERE v.PatientID = @PatientId
                ORDER BY v.VisitDateTime DESC";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PatientId", patientId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var visit = new VisitHistory
                        {
                            VisitID = Convert.ToInt32(reader["VisitID"]),
                            DoctorName = reader["DoctorName"].ToString(),
                            Speciality = reader["Speciality"].ToString(),
                            VisitDateTime = Convert.ToDateTime(reader["VisitDateTime"]),
                            Notes = reader["Notes"]?.ToString(),
                            PatientName = reader["PatientName"].ToString()
                        };

                        visits.Add(visit);
                    }
                }
            }
        }

        return visits;
    }
}