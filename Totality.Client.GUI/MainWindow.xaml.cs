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
using System.Windows.Media.Animation;
using Totality.Client.ClientComponents;
using Totality.Client.GUI.ReferenceToServer;
using Totality.Client.ClientComponents.Panels;
using System.ServiceModel.Discovery;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Totality.Client.GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<MinisteryButton> buttons = new List<MinisteryButton>();
        DoubleAnimation slideToLeft;
        DoubleAnimation slideToCenter;
        DoubleAnimation slideRightToCenter;
        ConnectionPanel _connectionPanel;
        UserControl currentPanel;
        TransmitterServiceClient _client;
        private Country _country;
        BackgroundWorker connectionSetter = new BackgroundWorker();
        private CallbackHandler _servCallbackHandler;

        public MainWindow()
        {
            _servCallbackHandler = new CallbackHandler();
            _servCallbackHandler.CountryUpdated += _servCallbackHandler_CountryUpdated;

            InitializeComponent();
            setAnims();
            this.MouseWheel += MainWindow_MouseWheel;
            this.dataGrid1.PreviewMouseWheel += dataGrid1_PreviewMouseWheel;
            this.dataGrid1.MouseWheel += dataGrid1_MouseWheel;
            currentPanel = dataGrid1;
            for (int i = 1; i <= 11; i++)
            buttons.Add(FindName("but" + i) as MinisteryButton);
            foreach (MinisteryButton but in buttons)
                but.connectToButtons(buttons);

            buttons[0].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(dataGrid1); 
            buttons[1].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(militaryPanel);
            buttons[2].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(financePanel);

            _connectionPanel = new ConnectionPanel();
            //_connectionPanel.
            _grid.Children.Add(_connectionPanel);
            _connectionPanel.StartSpinning();
            _client = new ReferenceToServer.TransmitterServiceClient(new System.ServiceModel.InstanceContext(_servCallbackHandler));
            
            connectionSetter.DoWork += FindServer;
            connectionSetter.RunWorkerCompleted += ServerFound;
            connectionSetter.RunWorkerAsync();

        }

        private void FindServer(object sender, DoWorkEventArgs e)
        {
            DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint());

            bool needToStop = false;

            while (!needToStop)
            {
                FindResponse servers = discoveryClient.Find(new FindCriteria(typeof(ITransmitterService)){ Duration = TimeSpan.FromSeconds(0.2) });
                System.Console.WriteLine(servers.Endpoints.Count);
                if (servers.Endpoints.Count > 0)
                {
                    needToStop = true;
                    _client.Endpoint.Address = servers.Endpoints[0].Address;
                }
            }
            discoveryClient.Close();
        }

        private void ServerFound(object sender, RunWorkerCompletedEventArgs e)
        {
            _client.Open();
            this.Dispatcher.Invoke( () => _client.InnerDuplexChannel.Faulted += ClientChannelFaulted);
            _client.Register("Hello, world!");
            this.Dispatcher.Invoke( () => _connectionPanel.Close());
        }


        private void ClientChannelFaulted(object sender, EventArgs e)
        {
            _client.Abort();
            _client = new ReferenceToServer.TransmitterServiceClient(new System.ServiceModel.InstanceContext(_servCallbackHandler));
            _connectionPanel.Dispatcher.Invoke(_connectionPanel.Open);
            connectionSetter.RunWorkerAsync();
        }

        private void _servCallbackHandler_CountryUpdated(Country country)
        {
            _country = country;
            _header.UpdateMoney(country.Money);
            _header.UpdateNukes(country.NukesCount);
            _header.UpdateMissiles(country.MissilesCount);
            _header.UpdateMood((short)country.Mood);
        }

        private void setAnims()
        {
            slideToLeft = new DoubleAnimation(0, -1200, TimeSpan.FromSeconds(0.1));
            slideToLeft.AccelerationRatio = 0.8;
            slideToCenter = new DoubleAnimation(-1200, 0, TimeSpan.FromSeconds(0.1));
            slideToCenter.DecelerationRatio = 0.2;
            slideRightToCenter = new DoubleAnimation(1240, 0, TimeSpan.FromSeconds(0.1));
            slideRightToCenter.DecelerationRatio = 0.2;
        }

        private void changePanel(UserControl newPanel)
        {
            if (newPanel != currentPanel)
            {
                currentPanel.BeginAnimation(Canvas.LeftProperty, slideToLeft);
                newPanel.BeginAnimation(Canvas.LeftProperty, slideRightToCenter);
                currentPanel = newPanel;
            }
        }

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Canvas.SetTop(this.dataGrid1, Canvas.GetTop(this.dataGrid1) + e.Delta*0.1);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            dataGrid1.addOrder(new OrderRecord("тест", "2000"));
            
        }

        private void dataGrid1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ((ordersTable)sender).CaptureMouse();
        }

        private void dataGrid1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ((ordersTable)sender).ReleaseMouseCapture();
        }
    }
}
