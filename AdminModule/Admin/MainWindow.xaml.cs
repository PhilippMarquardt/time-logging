using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MotorenMarquardtAdminModule
{
    /// <summary>
    /// Main Window logic
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += delegate { DragMove(); };
            this.DataContext = new MainViewModel();
        }
        
        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Visible;
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
           
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
            ButtonOpenMenu.Visibility = Visibility.Visible;
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControl usc = null;
            GridMain.Children.Clear();

            switch (((ListViewItem)((ListView)sender).SelectedItem).Name)
            {
                case "ItemHome":
                    usc = new Dashboard();
                    GridMain.Children.Add(usc);
                    break;
                case "ItemCreate":
                    usc = new AddFormula();
                    GridMain.Children.Add(usc);                 
                    break;
                case "Analysis":
                    usc = new MonthlyCalendar();
                    usc.DataContext = new MonthlyAnalysisViewModel();
                    GridMain.Children.Add(usc);
                    break;
                case "Evaluate":
                    usc = new EvaluateTasks();
                    usc.DataContext = new EvaluateTasksViewModel();
                    GridMain.Children.Add(usc);
                    break;
                case "EvaluateTimes":
                    usc = new EvaluateTimes();
                    usc.DataContext = new EvaluateTimesViewModel();
                    GridMain.Children.Add(usc);
                    break;
                default:
                    break;
            }
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = Application.Current.MainWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
    }
}
