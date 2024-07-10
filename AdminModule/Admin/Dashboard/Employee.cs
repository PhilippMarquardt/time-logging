using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorenMarquardtAdminModule
{
    public class Employee : INotifyPropertyChanged
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


        #region Properties
        private string _currentTask;
        private TimeSpan _logInTime;
        private TimeSpan _lastBreakTime;
        public string Name { get; set; }
        public int ID { get; set; }
        public string CurrentTask
        {
            get
            {
                return this._currentTask;
            }
            set
            {
                this._currentTask = value;
                OnPropertyChanged("CurrentTask");
            }
        }
        public TimeSpan LogInTime
        {
            get
            {
                return this._logInTime;
            }
            set
            {
                this._logInTime = value;
                OnPropertyChanged("LogInTime");
            }
        }
        public TimeSpan LastBreakTime
        {
            get
            {
                return this._lastBreakTime;
            }
            set
            {
                this._lastBreakTime = value;
                OnPropertyChanged("LastBreakTime");
            }
        }
        #endregion

        public Employee(string name, string currentTask, TimeSpan logInTime, int id, TimeSpan lastBreakTime)
        {
            this.Name = name;
            this.CurrentTask = currentTask;
            this.LogInTime = logInTime;
            this.ID = id;
            this.LastBreakTime = lastBreakTime;
        }

        public Employee() { }

        #region Equals Override
        public override bool Equals(object obj)
        {
            var item = obj as Employee;

            if (item == null)
            {
                return false;
            }

            return this.ID.Equals(item.ID);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }
        #endregion

    }
}
