using OnlineClinic.WindowsAdmin;
using OnlineClinic.WindowsReg;
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

namespace OnlineClinic 
{

    public partial class RegWindow : Window
    {
       
    private readonly PersonRepository _personRepository;
    private List<Person> _persons;
    public RegWindow()
        {
            InitializeComponent();
            string connectionString = Properties.Settings.Default.masterConnectionString;
            _personRepository = new PersonRepository(connectionString);
            LoadPersons();
            
        }
        private void LoadPersons()
        {
            _persons = _personRepository.GetAllPersons();
            PersonsDataGrid.ItemsSource = _persons;
        }
        private void ViewButton_click(object sender, RoutedEventArgs e) 
        {
            if (PersonsDataGrid.SelectedItem is Person selectedPerson)
            {
                var editWindow = new RegEdit(selectedPerson, _personRepository);
                if (editWindow.ShowDialog() == true)
                {
                    LoadPersons();
                }
            }
        }
        private void CreateSeans_click(object sender, RoutedEventArgs e)
        {
            if (PersonsDataGrid.SelectedItem is Person selectedPerson)
            {
                var session = new RegSession(selectedPerson, _personRepository);
                if (session.ShowDialog() == true)
                {
                    LoadPersons();
                }
            }

        }        
        private void Create_click(object sender, RoutedEventArgs e)
        {
            var addWindow = new RegReg(_personRepository);
            if (addWindow.ShowDialog() == true)
            {
                LoadPersons();
            }

        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ExitButton.ExitButton_Click(sender, e);
        }
    }
}
