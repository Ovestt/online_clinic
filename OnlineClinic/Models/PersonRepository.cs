using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class PersonRepository
{
    private readonly string _connectionString;

    public PersonRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<Person> GetAllPersons()
    {
        var persons = new List<Person>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"SELECT 
                            ID,
                            FirstName,
                            LastName,
                            MiddleName,
                            BirthDate,
                            PhoneNumber,
                            Gender,
                            SNILS,
                            RegistrationAddress,
                            ActualAddress
                          FROM Persons";

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var person = new Person
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            MiddleName = reader["MiddleName"]?.ToString(),
                            BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                            PhoneNumber = reader["PhoneNumber"]?.ToString(),
                            Gender = reader["Gender"]?.ToString(),
                            SNILS = reader["SNILS"]?.ToString(),
                            RegistrationAddress = reader["RegistrationAddress"].ToString(),
                            ActualAddress = reader["ActualAddress"]?.ToString()
                        };

                        persons.Add(person);
                    }
                }
            }
        }

        return persons;
    }

    public void AddPerson(Person person)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"INSERT INTO Persons 
                            (FirstName, LastName, MiddleName, BirthDate, 
                             PhoneNumber, Gender, SNILS, RegistrationAddress, ActualAddress)
                            VALUES 
                            (@FirstName, @LastName, @MiddleName, @BirthDate, 
                             @PhoneNumber, @Gender, @SNILS, @RegistrationAddress, @ActualAddress)";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FirstName", person.FirstName);
                command.Parameters.AddWithValue("@LastName", person.LastName);
                command.Parameters.AddWithValue("@MiddleName", (object)person.MiddleName ?? DBNull.Value);
                command.Parameters.AddWithValue("@BirthDate", person.BirthDate);
                command.Parameters.AddWithValue("@PhoneNumber", (object)person.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@Gender", (object)person.Gender ?? DBNull.Value);
                command.Parameters.AddWithValue("@SNILS", (object)person.SNILS ?? DBNull.Value);
                command.Parameters.AddWithValue("@RegistrationAddress", person.RegistrationAddress);
                command.Parameters.AddWithValue("@ActualAddress", (object)person.ActualAddress ?? DBNull.Value);

                command.ExecuteNonQuery();
            }
        }
    }

    public void UpdatePerson(Person person)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"UPDATE Persons SET
                            FirstName = @FirstName,
                            LastName = @LastName,
                            MiddleName = @MiddleName,
                            BirthDate = @BirthDate,
                            PhoneNumber = @PhoneNumber,
                            Gender = @Gender,
                            SNILS = @SNILS,
                            RegistrationAddress = @RegistrationAddress,
                            ActualAddress = @ActualAddress
                            WHERE ID = @ID";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID", person.ID);
                command.Parameters.AddWithValue("@FirstName", person.FirstName);
                command.Parameters.AddWithValue("@LastName", person.LastName);
                command.Parameters.AddWithValue("@MiddleName", (object)person.MiddleName ?? DBNull.Value);
                command.Parameters.AddWithValue("@BirthDate", person.BirthDate);
                command.Parameters.AddWithValue("@PhoneNumber", (object)person.PhoneNumber ?? DBNull.Value);
                command.Parameters.AddWithValue("@Gender", (object)person.Gender ?? DBNull.Value);
                command.Parameters.AddWithValue("@SNILS", (object)person.SNILS ?? DBNull.Value);
                command.Parameters.AddWithValue("@RegistrationAddress", person.RegistrationAddress);
                command.Parameters.AddWithValue("@ActualAddress", (object)person.ActualAddress ?? DBNull.Value);

                command.ExecuteNonQuery();
            }
        }
    }

}