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
using Totality.Client.ClientComponents.Panels;
using System.ServiceModel.Discovery;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Totality.CommonClasses;
using Totality.Client.ClientComponents.Dialogs;
using Totality.Client.ClientComponents.ServiceReference1;
using Totality.Model;
using Totality.Model.Diplomatical;


namespace Totality.Client.GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private Logger _log = new Logger();
        private List<MinisteryButton> buttons = new List<MinisteryButton>();
        private DoubleAnimation slideToLeft;
        private DoubleAnimation slideToCenter;
        private DoubleAnimation slideRightToCenter;
        private ConnectionPanel _connectionPanel;
        private UserControl currentPanel;
        private TransmitterServiceClient _client;
        private Country _country;
        private Model.Country _countryModel;
        private BackgroundWorker connectionSetter = new BackgroundWorker();
        private CallbackHandler _servCallbackHandler;
        private string _name;
        private NukeAttackDialog _nukeDialog;
        private WaitingPanel _waitingPanel;
        private StatPanel _statPanel;

        public MainWindow()
        {
           // try
            {
                _servCallbackHandler = new CallbackHandler();
                _servCallbackHandler.CountryUpdated += _servCallbackHandler_CountryUpdated;
                _servCallbackHandler.NukesInitialized += _servCallbackHandler_NukesInitialized;
                _servCallbackHandler.NukesUpdated += _servCallbackHandler_NukesUpdated;
                _servCallbackHandler.NewsReceived += _servCallbackHandler_NewsReceived;
                _servCallbackHandler.ContractsReceived += _servCallbackHandler_ContractsReceived;
                _servCallbackHandler.MessageReceived += _servCallbackHandler_MessageReceived;

                InitializeComponent();
                setAnims();

                _header.StatButtonClicked += _header_StatButtonClicked;
                _statPanel = new StatPanel();
                _statPanel.Visibility = Visibility.Hidden;
                canvas1.Children.Add(_statPanel);
                Canvas.SetTop(_statPanel, 50);


                this.MouseWheel += MainWindow_MouseWheel;
                _ordersTable.PreviewMouseWheel += dataGrid1_PreviewMouseWheel;
                _ordersTable.MouseWheel += dataGrid1_MouseWheel;
                currentPanel = _ordersTable;

                for (int i = 1; i <= 10; i++)
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
                SendButton.click += createSendDialog;

                _countryModel = new Model.Country("test");
                AbstractPanel.CountryData = _countryModel;
                AbstractPanel.Table = _ordersTable;

                AbstractDialog.CountryData = _countryModel;
                AbstractDialog.CurrentStep = -1;
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


                _connectionPanel = new ConnectionPanel();
                _connectionPanel.Video(new Uri(String.Format(@"{0}\video2.mp4", AppDomain.CurrentDomain.BaseDirectory, "turnoff"), UriKind.Absolute));

               _waitingPanel = new WaitingPanel();
                _grid.Children.Add(_waitingPanel);
                _waitingPanel.Close();

                _grid.Children.Add(_connectionPanel);
                _connectionPanel.NameReceived += _connectionPanel_NameReceived;
                _client = new TransmitterServiceClient(new System.ServiceModel.InstanceContext(_servCallbackHandler));

                _securityPanel._client = _client;
                _foreignPanel._client = _client;
                _financePanel._client = _client;

            }
           // catch (Exception error)
          //  {
          //      _log.Error(error.Message);
          //      MessageBox.Show(error.Message);
         //   }
        }


        private void _servCallbackHandler_MessageReceived(Model.Diplomatical.DipMsg msg)
        {
            _foreignPanel.ReceiveMessage(msg);
        }

        private void _servCallbackHandler_ContractsReceived(Model.Diplomatical.DipContract[] contracts)
        {
            try
            {
                _foreignPanel.ReceiveContracts(contracts);
            }
            catch (Exception error)
            {
               // _log.Error(error.Message);
                MessageBox.Show(error.Message);
            }
        }

        private void _servCallbackHandler_NewsReceived(News[] news)
        {
            _mediaPanel.ReceiveNews(news);
        }

        private void _header_StatButtonClicked()
        {
            _statPanel.Visibility = Visibility.Visible;
        }

        private void _servCallbackHandler_NukesUpdated(NukeRocket[] rockets)
        {
            try
            {
                if (_nukeDialog != null)
                {
                    _nukeDialog.UpdateRockets(rockets);
                }
                else
                {
                    _nukeDialog = new NukeAttackDialog();
                    _nukeDialog.TryToShootDown += _nukeDialog_TryToShootDown;
                    _nukeDialog.UpdateRockets(rockets);
                }
            }
            catch (Exception error)
            {
                //_log.Error(error.Message);
                MessageBox.Show(error.Message);
            }
        }

        private void _nukeDialog_TryToShootDown(Guid Id)
        {
            try
            {
                _client.ShootDownNuke(_countryModel.Name, Id);
            }
            catch (Exception error)
            {
                //_log.Error(error.Message);
                MessageBox.Show(error.Message);
            }
        }

        private void _servCallbackHandler_NukesInitialized()
        {
            _nukeDialog = new NukeAttackDialog();
            _nukeDialog.TryToShootDown += _nukeDialog_TryToShootDown;
            _grid.Children.Add(_nukeDialog);
            Canvas.SetLeft(_nukeDialog, (_premierPanel.Width - (_nukeDialog).Width) / 2.0);
            Canvas.SetTop(_nukeDialog, _header.Height + 41 + 10);
        }

        private void InitializeNuke()
        {

        }

        private void _connectionPanel_NameReceived(string name)
        {
            _countryModel = new Model.Country(name);
            AbstractDialog.CountryData = _countryModel;
            AbstractPanel.CountryData = _countryModel;
            _name = name;
            _countryModel.MissilesCount = 1000;
            connectionSetter.DoWork += FindServer;
            connectionSetter.RunWorkerCompleted += ServerFound;
            connectionSetter.RunWorkerAsync();
        }

        private void FindServer(object sender, DoWorkEventArgs e)
        {
            try
            {
                DiscoveryClient discoveryClient = new DiscoveryClient(new UdpDiscoveryEndpoint("soap.udp://192.168.0.255:3702")); //"soap.udp://192.168.0.255:3702"

                bool needToStop = false;

                while (!needToStop && this.IsInitialized)
                {
                    FindResponse servers = discoveryClient.Find(new FindCriteria(typeof(ITransmitterService)) { Duration = TimeSpan.FromSeconds(1) });
                    //_log.Info(servers.Endpoints.Count.ToString());
                    if (servers.Endpoints.Count > 0)
                    {
                        needToStop = true;
                        _client.Endpoint.Address = servers.Endpoints[0].Address;

                    }
                }
                discoveryClient.Close();
            }
            catch (Exception error)
            {
                //_log.Error(error.Message);
                MessageBox.Show(error.Message);
            }
        }

        private void ServerFound(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                _client.Open();
                this.Dispatcher.Invoke(() => _client.InnerDuplexChannel.Faulted += ClientChannelFaulted);
                if (_client.Register(_name))
                {
                    this.Dispatcher.Invoke(() => _connectionPanel.Close());
                    _waitingPanel.Open();
                    _client.AskUpdateAsync(_name);
                }
                else _client.Close();
            }
            catch (Exception error)
            {
                //_log.Error(error.Message);
                MessageBox.Show(error.Message);
            }
        }


        private void ClientChannelFaulted(object sender, EventArgs e)
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _client.Abort();
                    _client = new TransmitterServiceClient(new System.ServiceModel.InstanceContext(_servCallbackHandler));
                    _securityPanel._client = _client;
                    _foreignPanel._client = _client;
                    _financePanel._client = _client;
                    _connectionPanel.Open();
                    connectionSetter.RunWorkerAsync();
                }));
            }
            catch (Exception error)
            {
                //_log.Error(error.Message);
                MessageBox.Show(error.Message);
            }

}

        private void _servCallbackHandler_CountryUpdated(Country country)
        {
            try
            {
                _ordersTable.Clear();
                _waitingPanel.Close();
                if (_nukeDialog != null)
                {
                    _grid.Children.Remove(_nukeDialog);
                    _nukeDialog = null;
                }
                _country = country;
                AbstractPanel.CountryData = country;
                AbstractDialog.CountryData = country;
                AbstractDialog.Countries = country.CurrencyRatios.Keys.ToList();
                AbstractDialog.CurrentStep = country.Step;
                _header.UpdateMoney(country.Money);
                _header.UpdateNukes(country.NukesCount);
                _header.UpdateMissiles(country.MissilesCount);
                _header.UpdateMood((short)country.Mood);
                _header.UpdateProfit((long)country.FinalLightIndustry, country.TaxesLvl);

                _ordersTable.changeMoney(country.Money);

                _industryPanel.Update();
                _financePanel.Update();
                _militaryPanel.Update();
                _mediaPanel.Update();
                _foreignPanel.Update();
                _innerPanel.Update();
                _securityPanel.Update();
                _sciencePanel.Update();
                _premierPanel.Update();

                _statPanel.Update();

                but6.DoClick();
                changePanel(_mediaPanel);
                _mediaPanel.OpenNews();

                if (country.IsRiot)
                {
                    _header.RiotOn();
                    fire.Visibility = Visibility.Visible;
                }
                else
                {
                    _header.RiotOff();
                    fire.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception e)
            {
                //_log.Error(e.Message);
                MessageBox.Show(e.Message);
            }


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

        private void dataGrid1_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ((OrdersTable)sender).CaptureMouse();
        }

        private void dataGrid1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ((OrdersTable)sender).ReleaseMouseCapture();
        }

        private void createSendDialog()
        {
            SendDialog dial = new SendDialog(sendOrders);
            canvas1.Children.Add(dial);
            Canvas.SetLeft(dial, (Width - dial.Width) / 2);
            Canvas.SetTop(dial, Canvas.GetTop(_industryPanel) + (_industryPanel.Height - dial.Height) / 2);
        }

        private void sendOrders(bool allowed, SendDialog sender )
        {
            try
            {
                if (allowed)
                {
                    List<Order> orders = _ordersTable.GetOrders();

                    _client.AddOrders(orders.ToArray(), _country.Name);
                    _waitingPanel.Open();

                }
                canvas1.Children.Remove(sender);
            }
            catch (Exception e)
            {
                //_log.Error(e.Message);
                MessageBox.Show(e.Message);
            }
        }

        
    }
}
