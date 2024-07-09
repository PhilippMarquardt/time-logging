using System;
using System.Collections.Generic;
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

namespace MotorenMarquardtTimecontrol
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel mainViewModel;
        public MainWindow()
        {
            InitializeComponent();
            InitializeDashboard();
        }

        private void InitializeDashboard()
        {
            Dashboard board = new Dashboard();
            TaskSelection selection = new TaskSelection();
            mainViewModel = new MainViewModel(MainGridView, board, selection);
            board.DataContext = mainViewModel;
            selection.DataContext = mainViewModel;
            mainViewModel.ChangeToDashboard();
        }

        private void btnBackToDashboard_Click(object sender, RoutedEventArgs e)
        {
            
            mainViewModel.ChangeToDashboard();
        }

        private void btnIncreaseRowSize_Click(object sender, RoutedEventArgs e)
        {
            this.mainViewModel.DataGridRowHeight += 10;
        }

        private void btnDecreaseRowSize_Click(object sender, RoutedEventArgs e)
        {
            this.mainViewModel.DataGridRowHeight -= 10;
        }

        private void btnReloadAllEmployee_Click(object sender, RoutedEventArgs e)
        {
            this.mainViewModel.ReloadAllEmployee();
        }
    }
}
