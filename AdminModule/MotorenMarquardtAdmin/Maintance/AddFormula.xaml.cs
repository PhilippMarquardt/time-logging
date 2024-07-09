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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MotorenMarquardtAdminModule
{
    /// <summary>
    /// Interaktionslogik für AddFormula.xaml
    /// </summary>
    public partial class AddFormula : UserControl
    {
        public AddFormula()
        {
            InitializeComponent();         
        }

        private void AddEmployee_DialogClosing(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs)
        {
            
        }

        private void txtBoxTaskNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            string taskNumber = txtBoxTaskNumber.Text;
            if(Database.TaskNumberExists(taskNumber))
            {
                txtBoxTaskNumber.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                txtBoxTaskNumber.BorderBrush = new SolidColorBrush(Color.FromArgb(0,255,0,0));
            }
        }

        private void btnAddTask_Click(object sender, RoutedEventArgs e)
        {
            var taskNumber = txtBoxTaskNumber.Text;
            var taskDescription = txtTaskDescription.Text;
            var customer = txtTaskCustomer.Text;
            if (Database.CreateNewTask(taskNumber, customer, taskDescription))
            {
                System.Windows.MessageBox.Show("Auftrag erfolgreich hinzugefügt");
            }
            else
            {
                MessageBox.Show("Fehler beim hinzufügen des Auftrags");
            }
        }

        private void btnAddEmployee_Click(object sender, RoutedEventArgs e)
        {
            var name = txtBoxEmployeeName.Text;
            if(Database.CreateNewEmployee(name))
            {
                System.Windows.MessageBox.Show("Mitarbeiter erfolgreich hinzugefügt");
            }
            else
            {
                MessageBox.Show("Fehler beim hinzufügen des Mitarbeiters");
            }
        }
    }
}
