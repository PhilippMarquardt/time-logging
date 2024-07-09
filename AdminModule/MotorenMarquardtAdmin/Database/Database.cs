using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorenMarquardtAdminModule
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
        /**
         * Returns a ObeservableCollection with reloaded employees.
         **/
        public static ObservableCollection<Employee> GetAllEmployeesToday()
        {
            ObservableCollection<Employee> reloadedEmployees = new ObservableCollection<Employee>();

            MotorenEntities database = new MotorenEntities();

            DateTime today = DateTime.Today;

            var allEmployeesToday = (from Zeiten in database.Zeiten
                                     where DbFunctions.TruncateTime(Zeiten.startTime) == today && Zeiten.AuftragID == EINGESTEMPELT
                                     select Zeiten.Mitarbeiter).ToList();



            //todo: Die Queries sind einfach vom alten Programm kopiert -> Verbessern, wenn moeglich
            foreach (var emp in allEmployeesToday)
            {
                var stampedInTime = (from Zeiten in database.Zeiten
                                     where Zeiten.AuftragID == EINGESTEMPELT && DbFunctions.TruncateTime(Zeiten.startTime) == DateTime.Today && Zeiten.Mitarbeiter == emp
                                     select Zeiten.startTime).ToList().First();
               
                DateTime logInTime = (DateTime)stampedInTime;
                var lastBreakTime = (from Zeiten in database.Zeiten
                                     where Zeiten.AuftragID == PAUSE && DbFunctions.TruncateTime(Zeiten.startTime) == DateTime.Today && Zeiten.Mitarbeiter == emp
                                     select Zeiten.startTime);
                DateTime lastBreak;
                if (lastBreakTime == null || lastBreakTime.Count() == 0)
                {
                    lastBreak = new DateTime();
                }
                else
                {
                    lastBreak = (DateTime)lastBreakTime.ToList().Last();
                }



                var lastTaskID = (from Zeiten in database.Zeiten
                                  where Zeiten.Mitarbeiter == emp && Zeiten.Beendet == false
                                  select Zeiten.AuftragID).ToList().Last();

                var lastTaskName = (from Auftrag in database.Auftrag
                                    where Auftrag.ID == lastTaskID
                                    select Auftrag.Beschreibung).ToList().First();
                var empID = (from Mitarbeiter in database.Mitarbeiter
                             where Mitarbeiter.Mitarbeiter1 == emp
                             select Mitarbeiter.ID).ToList().First();

                reloadedEmployees.Add(new Employee(emp, lastTaskName, new TimeSpan(logInTime.Hour, logInTime.Minute, logInTime.Second), empID, new TimeSpan(lastBreak.Hour, lastBreak.Minute, lastBreak.Second)));
            }

            return reloadedEmployees;
        }

        public static ObservableCollection<string> GetAllEmployeesName()
        {
            MotorenEntities database = new MotorenEntities();
            ObservableCollection<string> allEmp = new ObservableCollection<string>();
            var allEmpFromDatabase = (from Mitarbeiter in database.Mitarbeiter
                                      select Mitarbeiter).ToList();
            foreach (var emp in allEmpFromDatabase)
            {
                allEmp.Add(emp.Mitarbeiter1);
            }
            return allEmp;
        }

        public static int GetTaskIDFromTaskNumber(string taskNumber)
        {
            using(MotorenEntities database = new MotorenEntities())
            {
                var task = (from Auftrag in database.Auftrag
                            where Auftrag.Auftragsnummer == taskNumber
                            select Auftrag).ToList();
                if (task.Count == 0) return -1;
                return task.First().ID;
            }
        }

        /**
         * Inserts a new task for the employee specified by the parameter. Default task is PAUSE.
         * @return true if successfull else false.
         **/
        public static bool InsertNewTask(string empName, DateTime startTime, DateTime endTime, int taskID = PAUSE)
        {
            MotorenEntities database = new MotorenEntities();

            if (startTime == null || endTime == null)
            {
                return false;
            }

            try
            {

                var lastTask = (from Zeiten in database.Zeiten
                                where Zeiten.Mitarbeiter == empName 
                                && DbFunctions.TruncateTime(Zeiten.startTime) == DbFunctions.TruncateTime(startTime) 
                                && Zeiten.startTime < startTime
                                select Zeiten).ToList();
                //there is a task before
                if (lastTask.Count > 0)
                {
                    lastTask.Sort((x,y) => x.startTime.Value.CompareTo(y.startTime.Value));
                    var last = lastTask.Last();
                    //Time is only inside the time from one task
                    if (last.endTime > endTime)
                    {
                        Zeiten _newTime = new Zeiten();
                        _newTime.AuftragID = last.AuftragID;
                        _newTime.Beendet = last.Beendet;
                        _newTime.endTime = last.endTime;
                        _newTime.Nachgetragen = true;
                        _newTime.startTime = endTime;
                        _newTime.Mitarbeiter = empName;
                        ((Zeiten)last).endTime = startTime;
                        last.Nachgetragen = true;
                        database.Zeiten.Add(_newTime);
                        database.SaveChanges();
                    }
                    //between two tasks
                    else
                    {
                        var nextTask = (from Zeiten in database.Zeiten
                                        where Zeiten.Mitarbeiter == empName && Zeiten.startTime < endTime &&
                                        DbFunctions.TruncateTime(Zeiten.startTime) == DbFunctions.TruncateTime(startTime)
                                        && Zeiten.endTime != null && Zeiten.endTime > endTime && Zeiten.startTime >= startTime
                                        select Zeiten).ToList();
                        //there is a next task
                        if (nextTask.Count > 0)
                        {
                            var next = nextTask.First();
                            next.startTime = endTime;
                            next.Nachgetragen = true;
                            ((Zeiten)last).endTime = startTime;
                            database.SaveChanges();
                        }
                        else
                        {
                            last.endTime = startTime;
                            last.Nachgetragen = true;
                            database.SaveChanges();
                        }
                    }
                    Zeiten newTime = new Zeiten();
                    newTime.AuftragID = taskID;
                    newTime.Beendet = true;
                    newTime.endTime = endTime;
                    newTime.Nachgetragen = true;
                    newTime.startTime = startTime;
                    newTime.Mitarbeiter = empName;
                    database.Zeiten.Add(newTime);
                    database.SaveChanges();
                    return true;
                }
                else
                {
                    Zeiten newTime = new Zeiten();
                    newTime.AuftragID = taskID;
                    newTime.Beendet = true;
                    newTime.startTime = startTime;
                    newTime.endTime = endTime;
                    newTime.Nachgetragen = true;
                    newTime.Mitarbeiter = empName;
                    database.Zeiten.Add(newTime);
                    database.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static bool TaskNumberExists(string taskNumber)
        {
            MotorenEntities database = new MotorenEntities();
            var auftrag = (from Auftrag in database.Auftrag
                           where Auftrag.Auftragsnummer == taskNumber
                           select Auftrag).ToList();
            if (auftrag != null && auftrag.Count > 0)
            {
                return true;
            }
            return false;
        }

        public static bool CreateNewTask(string taskID, string customer, string description)
        {
            try
            {
                MotorenEntities database = new MotorenEntities();
                Auftrag newTask = new Auftrag();
                newTask.Kunde = customer;
                newTask.Beendet = false;
                newTask.Auftragsnummer = taskID;
                newTask.Beschreibung = description;
                newTask.GeplanteZeit = TimeSpan.Zero;
                database.Auftrag.Add(newTask);
                database.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool CreateNewEmployee(string name)
        {
            try
            {
                MotorenEntities database = new MotorenEntities();
                var containsEmp = (from Mitarbeiter in database.Mitarbeiter
                                   where Mitarbeiter.Mitarbeiter1 == name
                                   select Mitarbeiter).ToList();
                if (containsEmp.Count > 0) return false;
                Mitarbeiter newEmployee = new Mitarbeiter();
                newEmployee.Mitarbeiter1 = name;
                database.Mitarbeiter.Add(newEmployee);
                database.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static TimeSpan GetTotalWorktimeOfDay(string empName, DateTime date)
        {

            using (MotorenEntities database = new MotorenEntities())
            {
                TimeSpan totalTime = TimeSpan.Zero;

                var allTimesOfDay = (from Zeiten in database.Zeiten
                                     where Zeiten.endTime != null
                                     && Zeiten.Mitarbeiter == empName
                                     && DbFunctions.TruncateTime(Zeiten.startTime) == DbFunctions.TruncateTime(date)
                                     && DbFunctions.TruncateTime(Zeiten.endTime) == DbFunctions.TruncateTime(date)
                                     && Zeiten.AuftragID != RAUCHERPAUSE
                                     && Zeiten.AuftragID != PAUSE
                                     && Zeiten.AuftragID != AUSSTEMPELN
                                     select Zeiten).ToList();
                foreach (var time in allTimesOfDay)
                {
                    var diff = time.endTime - time.startTime;
                    totalTime += new TimeSpan(diff.Value.Hours, diff.Value.Minutes, diff.Value.Seconds);
                }
                return totalTime;
            }
        }

        public static ObservableCollection<MonthlyAnalysisDetailView> GetDayDetail(string empName, DateTime date)
        {
            using (MotorenEntities database = new MotorenEntities())
            {
                ObservableCollection<MonthlyAnalysisDetailView> detail = new ObservableCollection<MonthlyAnalysisDetailView>();
                TimeSpan totalTime = TimeSpan.Zero;

                var allTimesOfDay = (from Zeiten in database.Zeiten
                                     where Zeiten.Mitarbeiter == empName
                                     && DbFunctions.TruncateTime(Zeiten.startTime) == DbFunctions.TruncateTime(date)
                                     && DbFunctions.TruncateTime(Zeiten.endTime) == DbFunctions.TruncateTime(date)
                                     select Zeiten).ToList();
                allTimesOfDay.Sort((x, y) => x.startTime.Value.CompareTo(y.startTime));
                foreach (var time in allTimesOfDay)
                {
                    var diff = time.endTime - time.startTime;
                    var task = (from Auftrag in database.Auftrag
                                       where Auftrag.ID == time.AuftragID
                                       select Auftrag).ToList().First();
                    var description = task.Beschreibung;
                    var customer = task.Kunde;
                    var startTime = time.startTime.ToString();
                    var endTime = time.endTime.ToString();
                    var edited = time.Nachgetragen;
                    detail.Add(new MonthlyAnalysisDetailView(customer, description, startTime, endTime, edited, String.Format(diff.ToString(), "{0:hh:mm:ss}")));
                }
                return detail;
            }
            
        }

        public static List<Auftrag> GetAllTaskWithMotorenMarquardt(string searchterm)
        {
            if (searchterm == "") searchterm = null; //prevent from showing every task
            try
            {
                Convert.ToInt32(searchterm);
                using (MotorenEntities database = new MotorenEntities())
                {
                    var tasks = (from Auftrag in database.Auftrag
                                 where Auftrag.Auftragsnummer.Contains(searchterm)
                                 select Auftrag).ToList();
                    return tasks;
                }
            }
            catch
            {
                using (MotorenEntities database = new MotorenEntities())
                {
                    var tasks = (from Auftrag in database.Auftrag
                                 where Auftrag.Beschreibung.Contains(searchterm)
                                 select Auftrag).ToList();
                    return tasks;
                }
            }
        }
        public static List<Auftrag> GetAllTasks(bool completed, string searchterm)
        {
            if (searchterm == "") searchterm = null; //prevent from showing every task
            int taskNumber;
            try
            {
                if (Int32.TryParse(searchterm, out taskNumber))
                {
                    using (MotorenEntities database = new MotorenEntities())
                    {
                        var tasks = (from Auftrag in database.Auftrag
                                     where Auftrag.Auftragsnummer.Contains(searchterm)
                                     && Auftrag.Kunde != "Motoren Marquardt"
                                     && Auftrag.Beendet == completed
                                     select Auftrag).ToList();
                        return tasks;
                    }
                }
                else
                {
                    using (MotorenEntities database = new MotorenEntities())
                    {
                        List<Auftrag> tasks;
                        tasks = (from Auftrag in database.Auftrag
                                 where (Auftrag.Beschreibung.Contains(searchterm) || Auftrag.Kunde.Contains(searchterm))
                                 && Auftrag.Kunde != "Motoren Marquardt"
                                 && Auftrag.Beendet == completed
                                 select Auftrag).ToList();
                        return tasks;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return new List<Auftrag>();
            }
           
        }

        public static List<Zeiten> GetAllTimesForDay(string empName, DateTime date)
        {
            using(MotorenEntities database = new MotorenEntities())
            {
                var times = (from Zeiten in database.Zeiten
                             where Zeiten.Mitarbeiter == empName
                             && DbFunctions.TruncateTime(Zeiten.startTime) == DbFunctions.TruncateTime(date)
                             select Zeiten).ToList();
                return times;
            }
        }
        public static Auftrag GetTaskToID(int ID)
        {
            using(MotorenEntities database = new MotorenEntities())
            {
                var task = (from Auftrag in database.Auftrag
                            where Auftrag.ID == ID
                            select Auftrag).ToList();
                if (task.Count == 0) return null;
                return task.First();
            }
        }

        public static bool SaveChangedTaskID(string TaskNumber, Zeiten timeToChange)
        {
            using(MotorenEntities database = new MotorenEntities())
            {
                try
                {
                    var task = (from Auftrag in database.Auftrag
                                where Auftrag.Auftragsnummer == TaskNumber
                                select Auftrag).ToList().First();
                    var time = (from Zeiten in database.Zeiten
                                where Zeiten.ID == timeToChange.ID
                                select Zeiten).ToList().First();
                    time.AuftragID = task.ID;
                    time.Nachgetragen = true;
                    database.SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        /**
         * Saves changes of a list of Auftrag to the database.
         **/
        public static void SaveChangesTasks(List<Auftrag> newTask)
        {
            using (MotorenEntities database = new MotorenEntities())
            {
                for (int i = 0; i < newTask.Count; i++)
                {
                    var id = newTask[i].ID;
                    var task = (from Auftrag in database.Auftrag
                                 where Auftrag.ID == id
                                 select Auftrag).ToList().First();
                    task.Kunde = newTask[i].Kunde;
                    task.Beschreibung = newTask[i].Beschreibung;
                    task.Beendet = newTask[i].Beendet;
                    task.Auftragsnummer = newTask[i].Auftragsnummer;
                }
                database.SaveChanges();
                System.Windows.MessageBox.Show("Erfolgreich");
            }
        }

        /**
         * Saves changes of a list of Zeiten to the database.
         **/
        public static void SaveChangesTimes(List<Zeiten> newTimes)
        {
            using (MotorenEntities database = new MotorenEntities())
            {
                for (int i = 0; i < newTimes.Count; i++)
                {
                    var id = newTimes[i].ID;
                    var times = (from Zeiten in database.Zeiten
                                where Zeiten.ID == id
                                select Zeiten).ToList().First();
                    if(times.startTime != newTimes[i].startTime || times.endTime != newTimes[i].endTime|| times.Mitarbeiter != newTimes[i].Mitarbeiter)
                    {
                        times.Nachgetragen = true;
                    }
                    times.startTime = newTimes[i].startTime;
                    times.endTime = newTimes[i].endTime;
                    times.Mitarbeiter = newTimes[i].Mitarbeiter;
                    database.SaveChanges();
                }
            }
        }

        public static List<Zeiten> GetTime(int taskID)
        {
            using(MotorenEntities database = new MotorenEntities())
            {
              
                var times = (from Zeiten in database.Zeiten
                             where Zeiten.AuftragID == taskID
                             select Zeiten).ToList();
                return times;
            }
        }

        public static bool DeleteTask(Auftrag taskToDelete)
        {
            using(MotorenEntities database = new MotorenEntities())
            {
                var task = (from Auftrag in database.Auftrag
                            where Auftrag.ID == taskToDelete.ID
                            select Auftrag).First();
                
                var taskNumber = task.ID;
                var timesOnTask = (from Zeiten in database.Zeiten
                                   where Zeiten.AuftragID == taskNumber
                                   select Zeiten).ToList().Count;
                if (timesOnTask > 0) return false;
                database.Auftrag.Remove(task);
                database.SaveChanges();
                return true;
            }           
        }

        public static void DeleteTime(Zeiten timeToDelete)
        {
            try
            {
                using(MotorenEntities database = new MotorenEntities())
                {
                    var time = (from Zeiten in database.Zeiten
                                where Zeiten.ID == timeToDelete.ID
                                select Zeiten).First();
                    database.Zeiten.Remove(time);
                    database.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
    }
}
