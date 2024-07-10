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
    /// Interaktionslogik für EvaluateTimes.xaml
    /// </summary>
    public partial class EvaluateTimes : UserControl
    {
        public EvaluateTimes()
        {
            InitializeComponent();
        }

        private void btnOpenDetails_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag != null)
            {
                var id = (sender as Button).Tag.ToString();
                (this.DataContext as EvaluateTimesViewModel).ReloadTask(id);
                DetailView.IsOpen = true;
            }
        }

        private void btnChangeTaskNumber_Click(object sender, RoutedEventArgs e)
        {
            ChangeTaskNumber.IsOpen = true;
        }

        private void saveNewTaskNumber_Click(object sender, RoutedEventArgs e)
        {          
            (this.DataContext as EvaluateTimesViewModel).SaveNewTaskNumber(txtBoxNewTaskNumber.Text);
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as EvaluateTimesViewModel).SaveChanges();
        }

        private void btnAddTime_Click(object sender, RoutedEventArgs e)
        {
            AddTime.IsOpen = true;
        }

        private void btnSaveNewTime_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as EvaluateTimesViewModel).AddTime();
        }

        private void btnDeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as EvaluateTimesViewModel).DeleteTimeSelected();
        }
    }
}
