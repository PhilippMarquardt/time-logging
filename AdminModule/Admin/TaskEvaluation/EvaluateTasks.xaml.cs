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
    /// Interaktionslogik für EvaluateTasks.xaml
    /// </summary>
    public partial class EvaluateTasks : UserControl
    {
        public EvaluateTasks()
        {
            InitializeComponent();
        }


        private void btnDeleteRow_Click(object sender, RoutedEventArgs e)
        {
            int ID = (int)((sender as Button).Tag);
            (this.DataContext as EvaluateTasksViewModel).DeleteTask(ID);
            
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
           (this.DataContext as EvaluateTasksViewModel).SaveChanges();
        }

        private void btnOpenDetails_Click(object sender, RoutedEventArgs e)
        {
            DetailView.IsOpen = true;
        }
    }
}
