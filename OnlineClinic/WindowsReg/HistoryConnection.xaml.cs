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
    public partial class HistoryConnection : Window
    {
        private readonly VisitRepository _visitRepository;
        private int _patientId;
        private string _patientName;
        public HistoryConnection(Person person, PersonRepository personRepository)
        {
            string connectionString = Properties.Settings.Default.masterConnectionString;
            InitializeComponent();

            _patientId = person.ID;
            _patientName = person.FullName;
            _visitRepository = new VisitRepository(connectionString);

            LoadPatientInfo();
            LoadVisitHistory();
        }
        private void LoadPatientInfo()
        {
            Title = $"История пациента: {_patientName}";
            DataContext = new { PatientName = _patientName };
        }

        private void LoadVisitHistory()
        {
            var visits = _visitRepository.GetPatientHistory(_patientId);
            VisitsDataGrid.ItemsSource = visits;
        }
        public void ExitButton_Click(object sender, EventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
