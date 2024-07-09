using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MotorenMarquardtAdminModule
{
    /// <summary>
    /// Interaktionslogik für MonthlyCalendar.xaml
    /// </summary>
    public partial class MonthlyCalendar : UserControl
    {
        public MonthlyCalendar()
        {
            InitializeComponent();
            
        }

        private void calendarListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (calendarListBox.SelectedItem != null)
            {
                (this.DataContext as MonthlyAnalysisViewModel).reloadDetailView(detailGrid);
                DetailView.IsOpen = true;
            }
        }

        private void btnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            ExcelWriter.ExcelWriter.WriteMonthlyAnalysisToExcel((this.DataContext as MonthlyAnalysisViewModel).CalendarItems, (this.DataContext as MonthlyAnalysisViewModel).SelectedEmployee);
        }
    }
}
