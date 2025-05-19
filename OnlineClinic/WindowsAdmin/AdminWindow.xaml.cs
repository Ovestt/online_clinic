using System;
using System.Collections.Generic;
using System.Data;
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
using System.Data.SqlClient;
using OnlineClinic.WindowsAdmin;


namespace OnlineClinic
{

    public partial class AdminWindow : Window
    {
        private readonly UserRepository _userRepository;
        private List<User> _users;
        public AdminWindow()
        {
            InitializeComponent();
            string connectionString = Properties.Settings.Default.masterConnectionString;
            _userRepository = new UserRepository(connectionString);

            LoadUsers();
        }
        private void LoadUsers()
        {
            _users = _userRepository.GetAllUsers();
            UsersDataGrid.ItemsSource = _users;
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ExitButton.ExitButton_Click(sender, e);
        }
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (UsersDataGrid.SelectedItem is User selectedUser)
            {
                var editWindow = new AdminEdit(selectedUser, _userRepository);
                editWindow.ShowDialog();
                LoadUsers();
            }

        }
        private void RegButton_click(object sender, RoutedEventArgs e)
        {
            var regWindow = new AdminReg(_userRepository);
            if (regWindow.ShowDialog() == true)
            {
                LoadUsers();
            }
        }
    }
}
