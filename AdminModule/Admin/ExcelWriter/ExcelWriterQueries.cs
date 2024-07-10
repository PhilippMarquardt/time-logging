using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorenMarquardtAdminModule.ExcelWriter
{
    /// <summary>
    /// This class provides static methods for the ExcelWriter.
    /// </summary>
    public static class ExcelWriterQueries
    {
        #region Task ID's
        private const int EINGESTEMPELT = 3;
        private const int AUSSTEMPELN = 4;
        private const int BETRIEBSARBEITEN = 5;
        private const int PAUSE = 6;
        private const int BUEROARBEITEN = 7;
        private const int RAUCHERPAUSE = 8;
        #endregion
        public static string GetLogInTime(string emp, DateTime day)
        {
            try
            {
                using(MotorenEntities database = new MotorenEntities())
                {
                    var logInTime = (from Zeiten in database.Zeiten
                                     where DbFunctions.TruncateTime(Zeiten.startTime) == DbFunctions.TruncateTime(day)
                                     && Zeiten.AuftragID == EINGESTEMPELT
                                     && Zeiten.Mitarbeiter == emp
                                     select Zeiten.startTime).ToList().First();
                    return logInTime.Value.ToShortTimeString();
                }
            }
            catch(Exception ex)
            {
              
                return "";
            }
        }

        public static string GetLogOutTime(string emp, DateTime day)
        {
            try
            {
                using (MotorenEntities database = new MotorenEntities())
                {
                    var logInTime = (from Zeiten in database.Zeiten
                                     where DbFunctions.TruncateTime(Zeiten.startTime) == DbFunctions.TruncateTime(day)
                                     && Zeiten.AuftragID == AUSSTEMPELN
                                     && Zeiten.Mitarbeiter == emp
                                     select Zeiten.startTime).ToList().First();
                    return logInTime.Value.ToShortTimeString();
                }
            }
            catch (Exception ex)
            {
                
                return "";
            }
        }

        public static string GetBreakTimeOfDay(string emp, DateTime day)
        {
            try
            {
                using (MotorenEntities database = new MotorenEntities())
                {
                    var allBreaks = (from Zeiten in database.Zeiten
                                     where DbFunctions.TruncateTime(Zeiten.startTime) == DbFunctions.TruncateTime(day)
                                     && Zeiten.endTime != null
                                     && (Zeiten.AuftragID == PAUSE || Zeiten.AuftragID == RAUCHERPAUSE)
                                     && Zeiten.Mitarbeiter == emp
                                     select Zeiten).ToList();
                    var span = allBreaks.Sum(x => (x.endTime - x.startTime).Value.Ticks);
                    var time = new TimeSpan(span);
                    return time.ToString(@"hh\:mm\:ss");
                }
            }
            catch (Exception ex)
            {
                
                return "";
            }
        }
    }
}
