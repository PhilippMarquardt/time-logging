using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MotorenMarquardtAdminModule
{
    public class EvaluateTasksViewModel
    {
      

        private bool _showCompetetTasks;
        public bool ShowCompletedTasks
        {
            get
            {
                return this._showCompetetTasks;
            }
            set
            {
                this._showCompetetTasks = value;             
                ReloadTask();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get
            {
                return this._searchText;
            }
            set
            {
                this._searchText = value;
                ReloadTask();
              
            }
        }

        private Auftrag _dataGridSelectedItem;
        public Auftrag DataGridSelectedItem
        {
            get
            {
                return this._dataGridSelectedItem;
            }
            set
            {
                this._dataGridSelectedItem = value;
                if(this._dataGridSelectedItem != null)
                {
                    reloadTimes();
                }
            }
        }

        public ObservableCollection<Auftrag> AllTasks { get; set; } = new ObservableCollection<Auftrag>();

        public ObservableCollection<Zeiten> Times { get; set; } = new ObservableCollection<Zeiten>();

        public EvaluateTasksViewModel()
        {
            ReloadTask();
        }

        public void DeleteTask(int ID)
        {
            Database.DeleteTask(AllTasks.Where(x => x.ID == ID).First());
            ReloadTask();       
        }

        private void ReloadTask()
        {
            AllTasks.Clear();
            foreach (var task in Database.GetAllTasks(ShowCompletedTasks, SearchText))
            {
                AllTasks.Add(task);
            }
        }

        private void reloadTimes()
        {
            Times.Clear();
            foreach (var time in Database.GetTime(DataGridSelectedItem.ID))
            {              
                Times.Add(time);
            }
        }

        public void SaveChanges()
        {
           Database.SaveChangesTasks(this.AllTasks.ToList());
           ReloadTask();
        }
    }
}
