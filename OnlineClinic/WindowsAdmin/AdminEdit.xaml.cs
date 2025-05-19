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

namespace OnlineClinic.WindowsAdmin
{

    public partial class AdminEdit : Window
    {
        private readonly User _user;
        private readonly UserRepository _userRepository;

        public AdminEdit(User user, UserRepository userRepository)
        {
            InitializeComponent();

            _user = user;
            _userRepository = userRepository;

            LastName.Text = user.LastName;
            FirstName.Text = user.FirstName;
            FamaliName.Text = user.MiddleName;
            DateBirthday.SelectedDate = user.BirthDate;
            txtLogin.Text = user.Login;
            SpecialityTxt.Text = user.Speciality;
            switch (user.Role) 
            {
                case "администратор": AdminRadio.IsChecked = true; break;
                case "регистратор": RegistrarRadio.IsChecked = true; break;
                case "врач": DoctorRadio.IsChecked = true; break;
            
            }
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
        private void EditUser_Click(object sender, RoutedEventArgs e)
        {
            _user.LastName = LastName.Text;
            _user.FirstName = FirstName.Text;
            _user.MiddleName = FamaliName.Text;
            _user.BirthDate = DateBirthday.SelectedDate.Value;
            _user.Login = txtLogin.Text;
            _user.Speciality = SpecialityTxt.Text;
            if (AdminRadio.IsChecked == true) _user.Role = "администратор";
            else if (RegistrarRadio.IsChecked == true) _user.Role = "регистратор";
            else if (DoctorRadio.IsChecked == true) _user.Role = "врач";

            var result = MessageBox.Show("Вы уверены, что хотите измененить этого пользователя?",
                       "Подтверждение изменения",
                       MessageBoxButton.YesNo,
                       MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _userRepository.UpdateUser(_user);
                DialogResult = true;
                Close();
            }

        }
        private void DelUser_CLick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите удалить этого пользователя?",
                                   "Подтверждение удаления",
                                   MessageBoxButton.YesNo,
                                   MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _userRepository.DeleteUser(_user.UserID);
                DialogResult = true;
                Close();
            }
        }

    }
}
