﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace OnlineClinic.WindowsDoctor
{
    public partial class DoctorHistory : Window
    {
        public DoctorHistory(int doctorId, int patientId, string connectionString)
        {
            InitializeComponent();
            DataContext = new VisitsViewModel(doctorId, patientId, connectionString);
        }
        public void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
