using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// 
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
            

        }
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnAddBreak_Click(object sender, RoutedEventArgs e)
        {
            try
            {              
                var emp = ((MainViewModel)this.DataContext).SelectedEmployeeName;
                TimeSpan startTime = TimeSpan.Parse(timePickerStartTime.Text);
                TimeSpan endTime = TimeSpan.Parse(timePickerEndTime.Text);
                DateTime startDatetime = (DateTime)(datePickerDate.SelectedDate + startTime);
                DateTime endDateTime = (DateTime)(datePickerDate.SelectedDate + endTime);
                var succ = Database.InsertNewTask(emp, startDatetime, endDateTime);
                if(!succ)
                {
                    System.Windows.MessageBox.Show("Fehler beim einfügen");
                }
                
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        /**
         * Yes, this method is completely stupid but I don't know how to do it better...
         **/
        private void btnGetEmployeeName_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as MainViewModel).SelectedEmployeeName = (sender as Button).Tag.ToString();
        }

       
    }
}
