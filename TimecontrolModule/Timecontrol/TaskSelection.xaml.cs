using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MotorenMarquardtTimecontrol
{
    /// <summary>
    /// Interaktionslogik für TaskSelection.xaml
    /// </summary>
    public partial class TaskSelection : UserControl
    {
        #region Task ID's
        private const int EINGESTEMPELT = 3;
        private const int AUSSTEMPELN = 4;
        private const int BETRIEBSARBEITEN = 5;
        private const int PAUSE = 6;
        private const int BUEROARBEITEN = 7;
        private const int RAUCHERPAUSE = 8;
        #endregion

        public TaskSelection()
        {
            InitializeComponent();
        }

        private void ButtonSelectTask_Click(object sender, RoutedEventArgs e)
        {try
            {
                var senderTag = (sender as Button).Tag.ToString();
                string selectedEmp = (this.DataContext as MainViewModel).LastSelectedEmployee;
                switch (senderTag)
                {
                    case "Einstempeln":
                        InsertTask(selectedEmp, EINGESTEMPELT);
                        break;
                    case "Ausstempeln":
                        InsertTask(selectedEmp, AUSSTEMPELN);
                        break;
                    case "Pause":
                        InsertTask(selectedEmp, PAUSE);
                        break;
                    case "Raucherpause":
                        InsertTask(selectedEmp, RAUCHERPAUSE);
                        break;
                    case "Betriebsarbeiten":
                        InsertTask(selectedEmp, BETRIEBSARBEITEN);
                        break;
                    case "Bueroarbeiten":
                        InsertTask(selectedEmp, BUEROARBEITEN);
                        break;
                    case "Auftrag":
                        if ((this.DataContext as MainViewModel).DataGridSelectedItem == null)
                            throw new Exception("Bitte einen Auftrag auswählen");
                        int selectedTaskID = (this.DataContext as MainViewModel).DataGridSelectedItem.ID;
                        InsertTask(selectedEmp, selectedTaskID);
                        break;
                }
            (this.DataContext as MainViewModel).ChangeToDashboard();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message); //TODO: Exception handler class
            }
        }

        private void InsertTask(string empName, int taskID)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Database.InsertNewTask(empName, taskID);
                ReloadEmployeeAfterInsertion(empName);
            }).Start();
            
        }

        private void ReloadEmployeeAfterInsertion(string name)
        {
            Application.Current.Dispatcher.BeginInvoke(
               DispatcherPriority.Background,
               new Action(() => (this.DataContext as MainViewModel).ReloadSpecificEmployee(name)));
        }
    
    }

     


    

   
}
