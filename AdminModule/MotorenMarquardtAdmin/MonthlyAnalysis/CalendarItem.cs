using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorenMarquardtAdminModule
{
    public class CalendarItem
    {
        public CalendarItem(string day, string actualDate, string worktime)
        {
            this.Day = day;
            this.ActualDate = actualDate;
            this.Worktime = worktime;
        }
        public string Day { get; set; }
        public string ActualDate { get; set; }
        public string Worktime { get; set; }
    }
}
