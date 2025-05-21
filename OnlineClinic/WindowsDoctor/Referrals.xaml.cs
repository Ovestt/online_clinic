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
using YourApp.ViewModels;

namespace OnlineClinic.WindowsDoctor
{
    /// <summary>
    /// Логика взаимодействия для Referrals.xaml
    /// </summary>
    public partial class Referrals : Window
    {
        public Referrals(int doctorId, int patientId, string connectionString)
        {
            InitializeComponent();

            var viewModel = new MedicalReferralViewModel(doctorId, patientId, connectionString);

            this.DataContext = viewModel;

        }
    }
}
