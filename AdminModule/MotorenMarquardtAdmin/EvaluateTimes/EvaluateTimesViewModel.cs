using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorenMarquardtAdminModule
{
    public class EvaluateTimesViewModel
    {

        private string _selectedDate = DateTime.Now.ToString();
        private string _selectedEmployee;
        private string _searchterm;
        public string Searchterm
        {
            get
            {
                return this._searchterm;
            }
            set
            {
                this._searchterm = value;
                AllTaskForSearch.Clear();
                var tasks = Database.GetAllTaskWithMotorenMarquardt(value);
                foreach(var task in tasks)
                {
                    AllTaskForSearch.Add(task.Auftragsnummer);
                }
            }
        }
        public string SelectedStartTime { get; set; }
        public string SelectedEndTime { get; set; }

        public string SelectedTaskID { get; set; }

        public ObservableCollection<string> AllTaskForSearch { get; set; } = new ObservableCollection<string>();

        public string SelectedDate
        {
            get
            {
                return this._selectedDate;
            }
            set
            {
                this._selectedDate = value;
                if(value != null && value != "" && SelectedEmployee != null && SelectedEmployee != "")
                {
                    ReloadTimes();
                }
            }
        }
        public string SelectedEmployee
        {
            get
            {
                return this._selectedEmployee;
            }
            set
            {
                this._selectedEmployee = value;
                if (value != null && value != "" && SelectedDate != null && SelectedDate != "")
                {
                    ReloadTimes();
                }
            }
        }
        public ObservableCollection<string> AllEmployees { get; set; } = new ObservableCollection<string>();

        public ObservableCollection<Zeiten> Times { get; set; } = new ObservableCollection<Zeiten>();

        public ObservableCollection<Auftrag> Task { get; set; } = new ObservableCollection<Auftrag>();

        public Zeiten SelectedTime { get; set; }
        public EvaluateTimesViewModel()
        {
            var allEmp = Database.GetAllEmployeesName();
            // INotifyPropertyChanged fuer die Observable collection hinzufuegen, wenn man die schleife sparen will und direkt zuweisung machen will
            foreach(var emp in allEmp)
            {
                AllEmployees.Add(emp);
            }
        }

        public void ReloadTask(string taskID)
        {
            this.Task.Clear();
            if(taskID != null && taskID != "")
            {
                try
                {
                    int id = Convert.ToInt32(taskID);
                    var task = Database.GetTaskToID(id);
                    if(task == null)
                    {
                        throw new Exception();
                    }
                    this.Task.Add(task);
                }
                catch
                {
                   
                }
            }
            
        }

        private void ReloadTimes()
        {
            this.Times.Clear();
            var times = Database.GetAllTimesForDay(this.SelectedEmployee, DateTime.Parse(SelectedDate));
            times.Sort((x, y) => x.startTime.Value.CompareTo(y.startTime));
            foreach(var time in times)
            {
                this.Times.Add(time);
            }
        }

        public void SaveNewTaskNumber(string newNumber)
        {
            if (newNumber == null || newNumber == "") return;
            if (SelectedTime == null) return;
            if (Convert.ToInt32(newNumber) <= 8) return;
            var succ = Database.SaveChangedTaskID(newNumber, SelectedTime);
            if (succ)
            {
                System.Windows.MessageBox.Show("Erfolgreich  gespeichert");
            }
            else
            {
                System.Windows.MessageBox.Show("Fehler beim speichern");
            }
            ReloadTimes();
        }

        public void SaveChanges()
        {
            Database.SaveChangesTimes(Times.ToList());
        }

        public void AddTime()
        {
           
            try
            {
                string taskID = SelectedTaskID.ToString();
                string empName = SelectedEmployee.ToString();
                TimeSpan start = TimeSpan.Parse(SelectedStartTime);
                TimeSpan end = TimeSpan.Parse(SelectedEndTime);
                DateTime startTime = DateTime.Parse(SelectedDate).Date;
                DateTime endTime = DateTime.Parse(SelectedDate).Date;
                startTime = startTime.Add(start);
                endTime = endTime.Add(end);
                int id = Database.GetTaskIDFromTaskNumber(taskID);
                Database.InsertNewTask(empName, startTime, endTime, id);
                System.Windows.MessageBox.Show("Erfolgreich eingefügt");
                ReloadTimes();
            }
            catch (Exception)
            {

            }
            
        }

        public void DeleteTimeSelected()
        {
            Database.DeleteTime(this.SelectedTime);
            ReloadTimes();
        }
    }
}
