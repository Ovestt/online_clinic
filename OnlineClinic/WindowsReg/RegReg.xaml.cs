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
using System.Windows.Shapes;

namespace OnlineClinic.WindowsReg
{
    /// <summary>
    /// Логика взаимодействия для RegReg.xaml
    /// </summary>
    public partial class RegReg : Window
    {
        private readonly PersonRepository _personRepository;
        public RegReg(PersonRepository personRepository)
        {
            InitializeComponent();
            _personRepository = personRepository;
            DateBirthday.SelectedDate = DateTime.Today;
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void SaveUser_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastName.Text) ||
            string.IsNullOrWhiteSpace(FirstName.Text) ||
            string.IsNullOrWhiteSpace(FamaliName.Text) ||
            string.IsNullOrWhiteSpace(txtSnils.Text) ||
            string.IsNullOrWhiteSpace(txtSex.Text) ||
            string.IsNullOrWhiteSpace(txtTel.Text) ||
            string.IsNullOrWhiteSpace(txtAddress.Text))
            {
                MessageBox.Show("Заполните все обязательные поля!", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            char sex = 'F';
            if (txtSex.Text.Trim().ToLower().StartsWith("м")) sex = 'M';
            var newPerson = new Person
            {
                LastName = LastName.Text,
                FirstName = FirstName.Text,
                MiddleName = FamaliName.Text,
                BirthDate = DateBirthday.SelectedDate.Value,
                SNILS = txtSnils.Text,
                Gender = sex.ToString(),
                PhoneNumber = txtTel.Text,
                RegistrationAddress = txtAddress.Text,
                ActualAddress = txtFactAddress.Text
            };

            try
            {
                _personRepository.AddPerson(newPerson);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пациента: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
