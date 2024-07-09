using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorenMarquardtTimecontrol
{
    /**
     * Provides a template for the MainDashboard ListBox
     **/
    public class ListBoxItem : INotifyPropertyChanged
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


        public ListBoxItem(string name, string task, string taskName)
        {
            this.Name = name;
            this.Task = task;
            this.TaskName = taskName;
        }

        private string _name;
        private string _task;
        private string _taskName;

        public string TaskName
        {
            get
            {
                return this._taskName;
            }
            set
            {
                this._taskName = value;
                OnPropertyChanged("TaskName");
            }
        }
        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Task
        {
            get
            {
                return this._task;
            }
            set
            {
                this._task = value;
                OnPropertyChanged("Task");
            }
        }
    }
}
