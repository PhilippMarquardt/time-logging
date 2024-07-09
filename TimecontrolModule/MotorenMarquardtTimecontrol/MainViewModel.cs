using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MotorenMarquardtTimecontrol
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Property changed area
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {

            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {

                handler(this, new PropertyChangedEventArgs(name));

            }

        }
        #endregion
        #region Dashboard Attributes
        private Grid MainGrid;
        private Dashboard _dashboard;
        private TaskSelection _taskSelection;
        private int _rows = 4;
        private ListBoxItem _listBoxItem; //selected listbox item
        #endregion

        #region Task Selection Attributes
        private int _dataGridRowHeight = 100; //Row height of datagrid cell
        private bool enableReloadTask = true; // If true, the tasks in the datagrid will be reloaded.
        #endregion

        #region Dashboard Properties
        public ObservableCollection<Auftrag> DataGridItems { get; set; } = new ObservableCollection<Auftrag>();
        public ListBoxItem SelectedItem
        {
            get
            {
                return this._listBoxItem;
            }
            set
            {
                this._listBoxItem = value;
                if (value != null)
                {
                    LastSelectedEmployee = value.Name;
                    enableReloadTask = false;
                    MainGrid.Children.Clear();
                    UserControl usc = new TaskSelection();
                    usc.DataContext = this;
                    MainGrid.Children.Add(_taskSelection);
                }
            }
        } //Selected Employee card on the dashboard(Listbox.SelectedItem)

        public string LastSelectedEmployee { get; set; }
        public ObservableCollection<ListBoxItem> ListBoxItems { get; set; } = new ObservableCollection<ListBoxItem>(); //Employee cards. TODO: Update name
        public int Columns { get; set; } = 4; // Number of Columns on the dashboard.
        public int Rows
        {
            get
            {
                return this._rows;
            }
            set
            {
                this._rows = value;
                OnPropertyChanged("Rows");
            }
        } //Number of Rows on the dashboard.
        #endregion
        #region Task Selection Properties
        public int DataGridRowHeight
        {
            get
            {
                return this._dataGridRowHeight;
            }
            set
            {
                this._dataGridRowHeight = value;
                OnPropertyChanged("DataGridRowHeight");
            }
        } // Height of Datagrid Row in Task Selection.
        public Auftrag DataGridSelectedItem { get; set; } // Selected Auftrag in datagrid.
        #endregion
        public MainViewModel(Grid mainGrid, Dashboard board, TaskSelection selection)
        {
            this.MainGrid = mainGrid;
            this._dashboard = board;
            this._taskSelection = selection;         
            LoadEmployees();
            ReloadTasks();
            InitializeTimers();
            _dashboard.DataContext = this;
            _taskSelection.DataContext = this;
        }
        #region private methods
        private void InitializeTimers()
        {
            //This timer reloads task every minute if it is enabled
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimerReloadTasks_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 30);
            dispatcherTimer.Start();
            //This timer reloads the employee after 500 seconds
            System.Windows.Threading.DispatcherTimer checkEmployeeNumberTimer = new System.Windows.Threading.DispatcherTimer();
            checkEmployeeNumberTimer.Tick += new EventHandler(dispatcherTimerCheckNewEmployee_Tick);
            checkEmployeeNumberTimer.Interval = new TimeSpan(0, 0, 500);
            checkEmployeeNumberTimer.Start();
        }
        /// <summary>
        /// Reloads employees if there is a new one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimerCheckNewEmployee_Tick(object sender, EventArgs e)
        {
            if(this.ListBoxItems.Count != Database.GetAllEmployees().Count)
            {
                LoadEmployees();
            }
        }

        /// <summary>
        /// Reloads tasks, if enabled.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimerReloadTasks_Tick(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() => ReloadTasks()));
            if (this.enableReloadTask)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new Action(() => ReloadAllEmployee()));
            }
        }
        
        /// <summary>
        /// Reloads all employees on the dashboard.
        /// </summary>
        private void LoadListBoxItems()
        {
            ListBoxItems.Clear();
            var allEmps = Database.GetAllEmployees();
            foreach (var emp in allEmps)
            {
                ListBoxItems.Add(emp);
            }
            SetRowSize(allEmps.Count);
        }

        /// <summary>
        /// Updates tasks of a specific employee.
        /// </summary>
        [Obsolete]
        private void _reloadAllEmployee()
        {
            foreach (var it in ListBoxItems)
            {
                it.Task = Database.GetSpecificEmployee(it.Name).Task;
                it.TaskName = Database.GetSpecificEmployee(it.Name).TaskName;
            }

        }
        /// <summary>
        /// Updates the amount of rows of the dashboard, so that all employee cards fit.
        /// </summary>
        /// <param name="relativeSize"></param>
        private void SetRowSize(int relativeSize)
        {
            Rows = (int)(double)Math.Ceiling(Convert.ToDouble(relativeSize) / Convert.ToDouble(Columns));
        }
        /// <summary>
        /// Reloads task datagrid on the task selection.
        /// </summary>
        private void ReloadTasks()
        {
            if (enableReloadTask)
            {
                DataGridItems.Clear();
                var tasks = Database.GetAllTasksNotCompleted().OrderBy(x => x.Kunde);
                foreach (var task in tasks)
                {
                    DataGridItems.Add(task);
                }
            }
        }
        #endregion
        #region public methods
        /// <summary>
        /// Reloads all employees on the dashboard.
        /// </summary>
        public void LoadEmployees()
        {
            LoadListBoxItems();
        }
        /// <summary>
        /// Loads the dashboard and enables the Reload Task Attribute.
        /// </summary>
        public void ChangeToDashboard()
        {
            enableReloadTask = true;
            MainGrid.Children.Clear();
            _dashboard.listBoxEmployees.UnselectAll();
            MainGrid.Children.Add(_dashboard);

        }
        /// <summary>
        /// Updates current Task and Task Number of a specific employee.
        /// </summary>
        /// <param name="name" The Employee to be updated.></param>
        public void ReloadSpecificEmployee(string name)
        {
            var item = ListBoxItems.Where(x => x.Name == name);
            var newTask = Database.GetSpecificEmployee(item.First().Name);
            if (newTask != null)
            {
                item.First().Task = newTask.Task;
                item.First().TaskName = newTask.TaskName;
            }
        }
        /// <summary>
        /// Reloads all employee cards on the dashboard in the background.
        /// </summary>
        public void ReloadAllEmployee()
        {
            Application.Current.Dispatcher.BeginInvoke(
                 DispatcherPriority.Background,
                 new Action(() => LoadEmployees()));
        }
        #endregion

    }


}
