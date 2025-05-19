using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace OnlineClinic.WindowsAdmin
{
    public partial class AdminReg : Window
    {
        private readonly UserRepository _userRepository;
        public AdminReg(UserRepository userRepository)
        {

            InitializeComponent();
            _userRepository = userRepository;
            DateBirthday.SelectedDate = DateTime.Today;
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e) 
        {
            DialogResult = false;
            Close();
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

        private void SaveUser_Click(object sender, RoutedEventArgs e) 
        {
            if (string.IsNullOrWhiteSpace(LastName.Text)||
            string.IsNullOrWhiteSpace(FirstName.Text)||
            string.IsNullOrWhiteSpace(FamaliName.Text)||
            string.IsNullOrWhiteSpace(txtLogin.Text)||
            string.IsNullOrWhiteSpace(txtPassword.Password)||
            !(DoctorRadio.IsChecked == true||
            RegistrarRadio.IsChecked == true||
            AdminRadio.IsChecked == true))
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            string pickRole = "";
            if (AdminRadio.IsChecked == true) pickRole = "администратор";
            else if (RegistrarRadio.IsChecked == true) pickRole = "регистратор";
            else if (DoctorRadio.IsChecked == true) pickRole = "врач";
            var newUser = new User
            {
                LastName = LastName.Text,
                FirstName = FirstName.Text,
                MiddleName = FamaliName.Text,
                BirthDate = DateBirthday.SelectedDate.Value,
                Login = txtLogin.Text,
                PasswordHash = HashPassword(txtPassword.Password),
                Role = pickRole,
                Speciality = SpecialityTxt.Text
            };

            try
            {
                _userRepository.AddUser(newUser);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пользователя: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
