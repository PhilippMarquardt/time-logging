using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorenMarquardtAdminModule
{
    class MonthlyAnalysisViewModel : INotifyPropertyChanged
    {
        #region Properties
        private ObservableCollection<MonthlyAnalysisDetailView> _detailView = new ObservableCollection<MonthlyAnalysisDetailView>();
        private DateTime _datetime = DateTime.Today;
        private int _rows;
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
        }
        public int Columns { get; set; } = 5;

        private string _selectedEmployee;
        public string SelectedEmployee
        {
            get
            {
                return this._selectedEmployee;
            }
            set
            {
                this._selectedEmployee = value;
                OnPropertyChanged("SelectedEmployee");
                reloadCalendar();
            }
        }

        private CalendarItem _calendarItem;
        public CalendarItem SelectedDay
        {
            get
            {
                return this._calendarItem;
            }
            set
            {
                this._calendarItem = value;
                OnPropertyChanged("SelectedDay");
            }
        }

        private ObservableCollection<string> _allEmployees = new ObservableCollection<string>();
        public ObservableCollection<string> AllEmployees
        {
            get
            {
                return this._allEmployees;
            }
            set
            {
                this._allEmployees = value;
                OnPropertyChanged("AllEmployees");
            }
        }

        public ObservableCollection<CalendarItem> CalendarItems { get; set; } = new ObservableCollection<CalendarItem>();

        public DateTime SelectedDate
        {
            get
            {
                return this._datetime;
            }
            set
            {
                this._datetime = value;
                OnPropertyChanged("SelectedDate");
                reloadCalendar();
            }
        }

        public ObservableCollection<MonthlyAnalysisDetailView> DetailView
        {
            get
            {
                return this._detailView;
            }
            set
            {
                this._detailView = value;
                OnPropertyChanged("DetailView");
            }
        }
        #endregion

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
        public MonthlyAnalysisViewModel()
        {
            foreach(var empName in Database.GetAllEmployeesName())
            {
                AllEmployees.Add(empName);
            }
            this.SelectedDate = DateTime.Today;
            
        }
        public void reloadCalendar()
        {
            //Return if invalid datetime
            if (this.SelectedDate == new DateTime()) return;
            CalendarItems.Clear();
            reloadCalendarCardsFilled(GetAllDaysOfMonth(), this.SelectedEmployee);
        }
        
        public void reloadDetailView(System.Windows.Controls.DataGrid grid)
        {
            try
            {
                var details = Database.GetDayDetail(SelectedEmployee, DateTime.Parse(SelectedDay.ActualDate));
                DetailView.Clear();
                foreach (var det in details)
                {
                    DetailView.Add(det);
                }
            }
            catch (Exception) { }           
        }
        


        #region private methods
        /**
         * Reloads all days to be displayed like windows calendar.
         **/
        private void reloadCalendarCards(List<DateTime> allDays, string emp)
        {
            //Fill the last days of the new month so the number is evenly devidible by the columns
            var lastDate = allDays[allDays.Count - 1];
            float initialAllDayCount = allDays.Count;
            float allDayCount = (float)allDays.Count;
            while (Math.Ceiling(allDayCount / (float)Columns) % Columns != 1)
            {
                lastDate = lastDate.AddDays(1);
                allDays.Add(lastDate);
                allDayCount += 1.0f;
            }


            // Reload first items with day of week
            for (int i=0;i<Columns;i++)
            {
                CalendarItems.Add(new CalendarItem(allDays[i].DayOfWeek.ToString(), allDays[i].Date.ToShortDateString(), Database.GetTotalWorktimeOfDay(emp, allDays[i]).ToString()));
            }
            // Reload items without day of week
            for(int i=Columns;i<allDays.Count;i++)
            {
                CalendarItems.Add(new CalendarItem("", allDays[i].Date.ToShortDateString(), Database.GetTotalWorktimeOfDay(emp, allDays[i]).ToString()));
            }
           
            this.Rows = (int)Math.Ceiling(initialAllDayCount / (float)Columns);
        }
        
        /**
        * Reloads all days to be displayed like Calendar in Windows API.
        **/
        private void reloadCalendarCardsFilled(List<DateTime> allDays, string emp)
        {
            // Reload first items with day of week
            for (int i = 0; i < Columns; i++)
            {
                if (allDays[i].Month == SelectedDate.Month)
                {
                    CalendarItems.Add(new CalendarItem(allDays[i].DayOfWeek.ToString(), allDays[i].Date.ToShortDateString(), Database.GetTotalWorktimeOfDay(emp, allDays[i]).ToString()));
                }
                else
                {
                    CalendarItems.Add(new CalendarItem(allDays[i].DayOfWeek.ToString(), "", ""));
                }
            }
            // Reload items without day of week
            for (int i = Columns; i < allDays.Count; i++)
            {
                if (allDays[i].Month == SelectedDate.Month)
                {
                    CalendarItems.Add(new CalendarItem("", allDays[i].Date.ToShortDateString(), Database.GetTotalWorktimeOfDay(emp, allDays[i]).ToString()));
                }
                else
                {
                    CalendarItems.Add(new CalendarItem("", "", ""));
                }
            }

            //TODO: Make this beautiful and not ugly af like this shit xD
            var itemsToRemove = new ObservableCollection<CalendarItem>();
            for(int i = 0; i < CalendarItems.Count; i++)
            {
                DateTime timeToCheck = new DateTime();
                if(CalendarItems[i].ActualDate != "")
                     timeToCheck = DateTime.Parse(CalendarItems[i].ActualDate);
                     if(timeToCheck.DayOfWeek == DayOfWeek.Saturday || timeToCheck.DayOfWeek == DayOfWeek.Sunday)
                        itemsToRemove.Add(CalendarItems[i]);



            }
            foreach(var it in itemsToRemove)
            {
                CalendarItems.Remove(it);
            }
            
            // Fill the number of cards so it's evenly devidible by the number of columns.
            float allDayCount = (float)allDays.Count - itemsToRemove.Count();
            while (Math.Ceiling(allDayCount / (float)Columns) % Columns != 1)
            {
                CalendarItems.Add(new CalendarItem("", "", ""));
                allDayCount += 1.0f;
            }
            this.Rows = (int)Math.Ceiling(((float)allDays.Count - itemsToRemove.Count()) / (float)Columns);
        }

        private List<DateTime> GetAllDaysOfMonth()
        {
            List<DateTime> allDays = new List<DateTime>();
            var startDate = this.SelectedDate;
            var firstDayOfMonth = new DateTime(startDate.Year, startDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var firstDayCopy = new DateTime(firstDayOfMonth.Year, firstDayOfMonth.Month, firstDayOfMonth.Day);
            while(firstDayCopy.DayOfWeek != DayOfWeek.Monday)
            {
                firstDayCopy = firstDayCopy.AddDays(-1);
                allDays.Add(firstDayCopy);
            }
            allDays.Reverse();
            
            while(!(firstDayOfMonth == lastDayOfMonth))
            {
                allDays.Add(firstDayOfMonth);
                firstDayOfMonth = firstDayOfMonth.AddDays(1);               
            }
            allDays.Add(lastDayOfMonth);
            return allDays;
        }
        #endregion

    }
}
