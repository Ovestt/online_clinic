using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;

namespace OnlineClinic
{

    public partial class MainWindow : Window
    {
        string ConnectionString = Properties.Settings.Default.masterConnectionString;
        public MainWindow()
        {
            InitializeComponent();

        }
        private void PswBox_key(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) BtnLogin_Click(sender, null);
        }
        private void TextBox_key(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) BtnLogin_Click(sender, null);
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text;
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Логин или пароль не введены", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                txtLogin.Text = "";
                txtPassword.Password = "";
                txtLogin.Focus();
                return;
            }

            var validationResult = ValidateUser(login, password);
            if (validationResult.Item1)
            {
                string role = validationResult.Item2;
                int userID = validationResult.Item3; 

                Window workWindow = null;
                switch (role)
                {
                    case "администратор":
                        workWindow = new AdminWindow();
                        break;
                    case "врач":
                        workWindow = new DoctorWindow(userID);
                        break;
                    case "регистратор":
                        workWindow = new RegWindow();
                        break;
                }

                workWindow?.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                txtLogin.Text = "";
                txtPassword.Password = "";
                txtLogin.Focus();
            }
        }
        private Tuple<bool, string, int> ValidateUser(string login, string password)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string query = "SELECT Role, UserID FROM UserCredentialsDB " + 
                               "WHERE Login = @Login AND PasswordHash = @PasswordHash";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@PasswordHash", HashPassword(password));

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string role = reader["Role"].ToString();
                            int userID = Convert.ToInt32(reader["UserID"]);
                            return Tuple.Create(true, role, userID);
                        }
                        else
                        {
                            return Tuple.Create(false, "", -1);
                        }
                    }
                }
            }
        }
        public static string HashPassword(string input)
        {

            if (string.IsNullOrEmpty(input))
                throw new ArgumentNullException(nameof(input));

            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }
        private void ExitButton_Click (object sender, RoutedEventArgs e)
        {
            ExitButton.ExitButton_Click(sender, e);
        }


    }
}
