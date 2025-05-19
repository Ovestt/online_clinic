using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<User> GetAllUsers()
    {
        var users = new List<User>();

        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"SELECT 
                            UserID,
                            FIO,
                            BirthDate,
                            Login,
                            PasswordHash,
                            Role,
                            Speciality
                          FROM UserCredentialsDB";

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var fioParts = reader["FIO"].ToString().Split(' ');
                        var user = new User
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            LastName = fioParts.Length > 0 ? fioParts[0] : "",
                            FirstName = fioParts.Length > 1 ? fioParts[1] : "",
                            MiddleName = fioParts.Length > 2 ? fioParts[2] : "",
                            BirthDate = Convert.ToDateTime(reader["BirthDate"]),
                            Login = reader["Login"].ToString(),
                            PasswordHash = reader["PasswordHash"].ToString(),
                            Role = reader["Role"].ToString(),
                            Speciality = reader ["Speciality"].ToString(),
                        };

                        users.Add(user);
                    }
                }
            }
        }

        return users;
    }

    public void UpdateUser(User user)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"UPDATE UserCredentialsDB 
                            SET 
                                FIO = @FIO,
                                BirthDate = @BirthDate,
                                Login = @Login,
                                PasswordHash = @PasswordHash,
                                Role = @Role
                                Speciality = @Speciality,
                            WHERE UserID = @UserID";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FIO", $"{user.LastName} {user.FirstName} {user.MiddleName}");
                command.Parameters.AddWithValue("@BirthDate", user.BirthDate);
                command.Parameters.AddWithValue("@Login", user.Login);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@Speciality", user.Speciality);
                command.Parameters.AddWithValue("@UserID", user.UserID);

                command.ExecuteNonQuery();
            }
        }
    }

    public void DeleteUser(int userId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = "DELETE FROM UserCredentialsDB WHERE UserID = @UserID";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", userId);
                command.ExecuteNonQuery();
            }
        }
    }
    public void AddUser(User user)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();

            string query = @"INSERT INTO UserCredentialsDB 
                        (FIO, BirthDate, Login, PasswordHash, Role, Speciality)
                        VALUES (@FIO, @BirthDate, @Login, @PasswordHash, @Role, @Speciality)";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FIO", $"{user.LastName} {user.FirstName} {user.MiddleName}");
                command.Parameters.AddWithValue("@BirthDate", user.BirthDate);
                command.Parameters.AddWithValue("@Login", user.Login);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@Role", user.Role);
                command.Parameters.AddWithValue("@Speciality", user.Speciality);

                command.ExecuteNonQuery();
            }
        }
    }
}