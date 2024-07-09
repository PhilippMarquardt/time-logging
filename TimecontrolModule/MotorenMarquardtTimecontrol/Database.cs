using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorenMarquardtTimecontrol
{
    public static class Database
    {
        #region Task ID's
        private const int EINGESTEMPELT = 3;
        private const int AUSSTEMPELN = 4;
        private const int BETRIEBSARBEITEN = 5;
        private const int PAUSE = 6;
        private const int BUEROARBEITEN = 7;
        private const int RAUCHERPAUSE = 8;
        #endregion
        public static List<ListBoxItem> GetAllEmployees()
        {
            try
            {
                using (MotorenEntities database = new MotorenEntities())
                {
                    List<ListBoxItem> allItems = new List<ListBoxItem>();
                    var allEmpFromDatabase = (from Mitarbeiter in database.Mitarbeiter
                                              select Mitarbeiter).ToList();
                    
                    foreach (var emp in allEmpFromDatabase)
                    {
                        var lastTaskID = (from Zeiten in database.Zeiten
                                          where Zeiten.Beendet == false && Zeiten.Mitarbeiter == emp.Mitarbeiter1
                                          select Zeiten).ToList();
                        string lastTask = "";
                        string lastTaskName = "";
                        if (lastTaskID.Count > 0)
                        {
                            var last = lastTaskID.Last().AuftragID;
                            lastTask  = (from Auftrag in database.Auftrag
                                            where Auftrag.ID == last
                                            select Auftrag).ToList().First().Auftragsnummer;
                            lastTaskName = (from Auftrag in database.Auftrag
                                        where Auftrag.ID == last
                                        select Auftrag).ToList().First().Beschreibung;
                        }
                        allItems.Add(new ListBoxItem(emp.Mitarbeiter1, lastTask, lastTaskName));
                    }
                    return allItems;
                }
            }
            catch (InvalidOperationException ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public static ListBoxItem GetSpecificEmployee(string name)
        {
            try
            {
                using (MotorenEntities database = new MotorenEntities())
                {
                     
                    var lastTaskID = (from Zeiten in database.Zeiten
                                        where Zeiten.Beendet == false && Zeiten.Mitarbeiter == name
                                        select Zeiten).ToList().First().AuftragID;
                    var lastTask = (from Auftrag in database.Auftrag
                                    where Auftrag.ID == lastTaskID
                                    select Auftrag).ToList().First();
                    
                    return new ListBoxItem(name, lastTask.Auftragsnummer, lastTask.Beschreibung);
                }
            }
            catch (Exception)
            {
                return new ListBoxItem(name, "", ""); ;
            }
        }

        public static List<Auftrag> GetAllTasksNotCompleted()
        {
            try
            {
                using (MotorenEntities database = new MotorenEntities())
                {
                    
                    var allTasksNotCompleted = (from Auftrag in database.Auftrag
                                                where Auftrag.Beendet == false
                                                select Auftrag).ToList();
                    allTasksNotCompleted.Sort((x, y) => x.Kunde.CompareTo(y.Kunde));
                    return allTasksNotCompleted;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return null;
            }
        }
   
        public static void InsertNewTask(string emp, int taskID)
        {
            try
            {
                if(taskID == EINGESTEMPELT)
                {
                    if (IsCheckedIn(emp))
                    {
                        throw new Exception("Es wurde sich heute schon eingestempelt");
                    }
                }
                else if (!IsCheckedIn(emp))
                {
                    throw new Exception("Bitte erst einstempeln");
                }
                using (MotorenEntities database = new MotorenEntities())
                {
                    var lastTask = (from Zeiten in database.Zeiten
                                    where Zeiten.Beendet == false
                                    && Zeiten.Mitarbeiter == emp
                                    select Zeiten).ToList();
                    if (lastTask.Count != 0)
                    {
                        lastTask.First().Beendet = true;
                        lastTask.First().endTime = DateTime.Now;
                    }
                    Zeiten newTime = new Zeiten();
                    newTime.AuftragID = taskID;
                    newTime.Beendet = false;
                    newTime.startTime = DateTime.Now;
                    newTime.Mitarbeiter = emp;
                    newTime.Nachgetragen = false;
                    database.Zeiten.Add(newTime);
                    database.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + "\n" + "Error at inserting task");
            }
        }

        private static bool IsCheckedIn(string emp)
        {
            try
            {
                using (MotorenEntities database = new MotorenEntities())
                {
                    var checkedIn = (from Zeiten in database.Zeiten
                                     where Zeiten.Mitarbeiter == emp
                                     && DbFunctions.TruncateTime(Zeiten.startTime) == DateTime.Today
                                     && Zeiten.AuftragID == EINGESTEMPELT
                                     select Zeiten).ToList();
                    return checkedIn.Count > 0 ? true : false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
