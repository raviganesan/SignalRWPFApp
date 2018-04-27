using Expression.Blend.SampleData.SampleDataSource;
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

namespace SignalRWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SampleDataSource sd = new SampleDataSource();
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            datagrid.ItemsSource = sd.Collection;
        }

        private void DataGrid_TargetUpdated(object sender, DataTransferEventArgs e)
        {

        }   

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            sd.Collection[0].SettingsValue = "1234";

        }

        private async void ContentControl_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            (sender as ContentControl).Background = new SolidColorBrush(Colors.Yellow);
            await Task.Delay(400);
            (sender as ContentControl).Background = new SolidColorBrush(Colors.Transparent);

        }
    }
}
