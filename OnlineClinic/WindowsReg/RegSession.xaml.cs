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
    /// Логика взаимодействия для RegSession.xaml
    /// </summary>
    public partial class RegSession : Window
    {
        private readonly Person _person;
        private readonly PersonRepository _personRepository;
        public RegSession(Person person, PersonRepository personRepository)
        {
            InitializeComponent();
        }
        public void SaveSession_Click(object sender, RoutedEventArgs e) 
        { 
        }
    }
}
