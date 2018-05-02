using Expression.Blend.SampleData.SampleDataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Microsoft.AspNet.SignalR.Client;

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
            this.DataContext = sd;
        }
        private IHubProxy MyHub { get; set; }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            datagrid.ItemsSource = sd.Collection;

            //Set connection
            var connection = new HubConnection("http://localhost:60599/");
            //Make proxy to hub based on hub name on server
            MyHub = connection.CreateHubProxy("RealTimeHub");
            //Start connection


            //var ollection = new ItemCollection
            //    {
            //        new Item
            //        {
            //            SettingsName = "BidRecommendationService",
            //            SettingsValue = "{IsStacked=False}"
            //        }
            //    };

            MyHub.On<string>("addMessage", param => {
                //Console.WriteLine(param);

                param = param.Replace("Server:", "");

                var keyValue = param.Split(':');

                var findIndex = sd.Collection.ToList().FindIndex(v => v.SettingsName == keyValue[0]);

                if (findIndex > -1)
                {
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        var item = sd.Collection.ElementAt(findIndex).SettingsValue = keyValue[1];
                        sd.Collection.RemoveAt(findIndex);
                        sd.Collection.Insert(findIndex, new Item
                        {
                            SettingsName = keyValue[0],
                            SettingsValue = keyValue[1]
                        });
                    });


                }
                else if (findIndex == -1 && param.Contains(':'))
                {
                    App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                    {
                        sd.Collection.Add(new Item
                        {
                            SettingsName = keyValue[0],
                            SettingsValue = keyValue[1]
                        });
                    });
                }

                sd.DynamicText += param + Environment.NewLine;
                

                //setting.
            });

            var isConnected = false;

            while (!isConnected)
            {
                connection.Start().ContinueWith(task =>
                {
                    if (task.IsFaulted)
                    {
                        Console.WriteLine("There was an error opening the connection:{0}",
                            task.Exception.GetBaseException());
                        sd.ConnectionStatus = "There was an error opening the connection";
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        isConnected = true;
                        sd.ConnectionStatus = "Connected!";
                    }

                }).Wait();
            }


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

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {

            MyHub.Invoke<string>("ConfigSettings", InputData.Text).Wait();
        }
    }
}
