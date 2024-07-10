using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorenMarquardtAdminModule
{
    public class MonthlyAnalysisDetailView : INotifyPropertyChanged
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

        public MonthlyAnalysisDetailView(string customer, string description, string startTime, string endTime, bool edited, string time)
        {
            this.Customer = customer;
            this.Description = description;
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.Edited = edited;
            this.Time = time;
        }

        private string _customer;
        private string _description;
        private string _StartTime;
        private string _EndTime;
        private bool _Edited;
        private string _Time;

        public string Customer
        {
            get
            {
                return this._customer;
            }
            set
            {
                this._customer = value;
                OnPropertyChanged("Customer");
            }
        }
        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
                OnPropertyChanged("Description");
            }
        }

        public string StartTime
        {
            get
            {
                return this._StartTime;
            }
            set
            {
                this._StartTime = value;
                OnPropertyChanged("StartTime");
            }
        }
        public string EndTime
        {
            get
            {
                return this._EndTime;
            }
            set
            {
                this._EndTime = value;
                OnPropertyChanged("EndTime");
            }
        }
        public string Time
        {
            get
            {
                return this._Time;
            }
            set
            {
                this._Time = value;
                OnPropertyChanged("Time");
            }
        }
        public bool Edited
        {
            get
            {
                return this._Edited;
            }
            set
            {
                this._Edited = value;
                OnPropertyChanged("Edited");
            }
        }
    }
}
