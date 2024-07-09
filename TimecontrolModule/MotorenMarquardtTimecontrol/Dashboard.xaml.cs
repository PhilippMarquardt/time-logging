using System.Windows.Controls;
using System.Windows.Input;


namespace MotorenMarquardtTimecontrol
{
    /// <summary>
    /// Interaktionslogik für Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private void listBoxEmployees_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //(this.DataContext as MainViewModel).SelectedItem = (this.DataContext as MainViewModel).SelectedItem;
        }

      
    }
}
