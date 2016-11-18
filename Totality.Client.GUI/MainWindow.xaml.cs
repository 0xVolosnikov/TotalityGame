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
using Totality.CommonClasses;
using Totality.Client.ClientComponents.Dialogs;

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
        private Model.Country _countryModel;
        BackgroundWorker connectionSetter = new BackgroundWorker();
        private CallbackHandler _servCallbackHandler;
        private string _name;

        public MainWindow()
        {
            _servCallbackHandler = new CallbackHandler();
            _servCallbackHandler.CountryUpdated += _servCallbackHandler_CountryUpdated;
            //_servCallbackHandler.
            _countryModel = new Model.Country("test");

            InitializeComponent();
            setAnims();

            this.MouseWheel += MainWindow_MouseWheel;
            _ordersTable.PreviewMouseWheel += dataGrid1_PreviewMouseWheel;
            _ordersTable.MouseWheel += dataGrid1_MouseWheel;
            currentPanel = _ordersTable;

            for (int i = 1; i <= 11; i++)
            buttons.Add(FindName("but" + i) as MinisteryButton);
            foreach (MinisteryButton but in buttons)
                but.connectToButtons(buttons);

            buttons[0].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(_ordersTable);
            buttons[1].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(_industryPanel);
            buttons[2].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(_financePanel);
            buttons[3].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(_militaryPanel);
            buttons[4].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(_foreignPanel);
            buttons[5].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(_mediaPanel);
            buttons[6].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(_innerPanel);
            buttons[7].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(_securityPanel);
            buttons[8].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(_sciencePanel);
            buttons[9].MouseDown += (object sender, MouseButtonEventArgs e) => changePanel(_premierPanel);

            AbstractPanel.CountryData = _countryModel;
            AbstractPanel.Table = _ordersTable;

            AbstractDialog.CountryData = _countryModel;
            AbstractDialog.Countries.Add("Test"); //TEST
            AbstractDialog.Countries.Add("Test2"); //TEST
            AbstractDialog.Ministers.AddRange(new List<string>
            {
            "Министерство Промышленности",
            "Министерство Финансов",
            "Министерство Обороны",
            "Министерство Иностранных Дел",
            "Средства Массовой Информации",
            "Министерство Внутренних Дел",
            "Министерство Государственной Безопасности",
            "Министерство Образования и Науки",
            "Премьер-Министр"
            });


            // _connectionPanel = new ConnectionPanel();
            //_connectionPanel.Video(new Uri(String.Format(@"{0}\video2.mp4", AppDomain.CurrentDomain.BaseDirectory, "turnoff"), UriKind.Absolute));

            // _grid.Children.Add(_connectionPanel);
            //_connectionPanel.NameReceived += _connectionPanel_NameReceived;
            // _client = new ReferenceToServer.TransmitterServiceClient(new System.ServiceModel.InstanceContext(_servCallbackHandler));

            _grid.Children.Add(new NukeAttackDialog());

        }

        private void _connectionPanel_NameReceived(string name)
        {
            _name = name;
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
                FindResponse servers = discoveryClient.Find(new FindCriteria(typeof(ITransmitterService)){ Duration = TimeSpan.FromSeconds(0.5) });
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
            _client.Register(_name);
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
            changePanel(_ordersTable);
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
            Canvas.SetTop(_ordersTable, Canvas.GetTop(_ordersTable) + e.Delta*0.1);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _ordersTable.addOrder(new OrderRecord("тест", "2000"));
            
        }

        private void dataGrid1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ((OrdersTable)sender).CaptureMouse();
        }

        private void dataGrid1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ((OrdersTable)sender).ReleaseMouseCapture();
        }
    }
}
