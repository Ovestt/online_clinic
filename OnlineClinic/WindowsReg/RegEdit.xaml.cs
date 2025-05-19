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
    public partial class RegEdit : Window
    {
        private readonly Person _person;
        private readonly PersonRepository _personRepository;
        public RegEdit(Person person, PersonRepository personRepository)
        {
            InitializeComponent();

            _person = person;
            _personRepository = personRepository;

            LastName.Text = person.LastName;
            FirstName.Text = person.FirstName;
            FamaliName.Text = person.MiddleName;
            DateBirthday.SelectedDate = person.BirthDate;
            txtSnils.Text = person.SNILS;
            txtSex.Text = person.Gender;
            txtTel.Text= person.PhoneNumber;
            txtAddress.Text = person.RegistrationAddress;
            txtFactAddress.Text = person.ActualAddress;
        }
        public void ExitButton_Click(object sender, EventArgs e) 
        {
            DialogResult = false;
            Close();
        }
        public void SaveUser_Click(object sender, EventArgs e)
        {
            _person.LastName = LastName.Text;
            _person.FirstName = FirstName.Text;
            _person.MiddleName = FamaliName.Text;
            _person.BirthDate = DateBirthday.SelectedDate.Value;
            _person.SNILS = txtSnils.Text;
            _person.Gender = txtSex.Text;
            _person.PhoneNumber = txtTel.Text;
            _person.RegistrationAddress = txtAddress.Text;
            _person.ActualAddress = txtFactAddress.Text;
            var result = MessageBox.Show("Вы уверены, что хотите измененить этого пациента?",
           "Подтверждение изменения",
           MessageBoxButton.YesNo,
           MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                _personRepository.UpdatePerson(_person);
                DialogResult = true;
                Close();
            }

        }
        public void history_Click(object sender, EventArgs e)
        {
            DialogResult = false;
            Close();
        }
        public void RegSession_Click(object sender, EventArgs e)
        {
            var editWindow = new RegSession(_person, _personRepository);
            editWindow.ShowDialog();

        }
    }
    
}
