using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MotorenMarquardtAdminModule
{
    public class MainViewModel
    {
        #region Dashboard cards logic
        public ObservableCollection<Employee> Employees { get; set; } = new ObservableCollection<Employee>();
        #endregion

        #region Dashboard properties
        public Employee SelectedItem { get; set; }
        public string SelectedEmployeeName { get; set; }
        #endregion

        public int Rows { get; set; } = 5;
        public int Columns { get; set; } = 5;

      

        public MainViewModel()
        {
            reloadEmployees();
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 60);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.BeginInvoke(
            System.Windows.Threading.DispatcherPriority.Background,
            new Action(() => reloadEmployees()));

        }
        
        /**
         * Reloads all employees with new data from the database
         **/
        private void reloadEmployees()
        {
            var emp = Database.GetAllEmployeesToday();
            foreach (var em in emp)
            {
                Employees.Remove(em);
                Employees.Add(em);
            }
        }

       
    }
}
