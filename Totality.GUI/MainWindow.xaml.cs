using Totality.Handlers.Diplomatical;
using Totality.Handlers.Main;
using Totality.Handlers.News;
using Totality.Handlers.Nuke;
using Totality.TransmitterService;
using System;
using System.Windows;
using System.Windows.Media;
using Totality.Model.Interfaces;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Collections.Generic;
using System.Net;
using System.Windows.Controls;
using Totality.Model;

namespace Totality.GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoggingSystem.Logger _logger = new LoggingSystem.Logger();
        private Transmitter _transmitter;
        private IDataLayer _dataLayer;
        private MainHandler _mainHandler;
        private DiplomaticalHandler _dipHandler;
        private NewsHandler _newsHandler;
        private NukeHandler _nukeHandler;
        private ServiceHost _host;
        private List<Label> _labels = new List<Label>();

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
            _transmitter = new Transmitter(_logger);
            _newsHandler = new NewsHandler();


            _dataLayer = new DataLayer.DataLayer(_logger);
            _nukeHandler = new NukeHandler(_newsHandler, _transmitter, _dataLayer, _logger);
            _mainHandler = new MainHandler(_newsHandler, _dataLayer, _logger, _nukeHandler);            
            _dipHandler = new DiplomaticalHandler(_newsHandler, _transmitter, _dataLayer, _logger);

            _mainHandler.Transmitter = _transmitter;
            _mainHandler.DipHandler = _dipHandler;
            _newsHandler.Transmitter = _transmitter;
            _transmitter.MainHandler = _mainHandler;
            _transmitter.NukeHandler = _nukeHandler;
            _transmitter.DipHandler = _dipHandler;
            _transmitter.ClientRegistered += _transmitter_ClientRegistered;
            _transmitter.ClientDisconnected += _transmitter_ClientDisconnected;
            _transmitter.ClientSendedData += _transmitter_ClientSendedData;
        }

        private void _transmitter_ClientSendedData(string name)
        {
            _labels.Find(x => (string)x.Content == name).Foreground = Brushes.Blue;
            if (_labels.TrueForAll(x => x.Foreground == Brushes.Blue || x.Foreground == Brushes.Red))
                ordersDisplay.Fill = Brushes.PowderBlue;
        }

        private void _transmitter_ClientDisconnected(string name)
        {
            Dispatcher.BeginInvoke(new Action(() => { _labels.Find(x => (string)x.Content == name).Foreground = Brushes.Red; }));
        }

        private void _transmitter_ClientRegistered(string name)
        {
            if (_labels.Exists(x => (string)x.Content == name))
            {
                _labels.Find(x => (string)x.Content == name).Foreground = Brushes.Black;
            }
            else
            {
                _labels.Add(new Label()
                {
                    Width = 351,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Content = name
                });
                _wrap.Children.Add(_labels[_labels.Count - 1]);
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
            _logger.killLoggingWindow();
        }

        private void startListening_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var ipDial = new IpDialog();
                ipDial.IpReceived += IpDial_IpReceived;
                grid.Children.Add(ipDial);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed opening host! " + ex.Message);
            }


        }

        private void IpDial_IpReceived(object sender, string ip)
        {
            grid.Children.Remove((UIElement)sender);

            try
            {
                _host = new ServiceHost(_transmitter, new Uri("net.tcp://" + ip + ":10577/transmitter"));
            }
            catch (Exception e)
            {
                _logger.Error("Something get wrong with configuring host: " + e.Message);
            }

            ip = ip.Remove(ip.LastIndexOf(".")+1);
            ip += "255";

            ServiceDiscoveryBehavior discoveryBehavior = new ServiceDiscoveryBehavior();
            // send announcements on UDP multicast transport
            discoveryBehavior.AnnouncementEndpoints.Add(
              //new UdpAnnouncementEndpoint("soap.udp://192.168.1.255:3702"));
              //new UdpAnnouncementEndpoint("soap.udp://" + ip + ":3702"));
              new UdpAnnouncementEndpoint("soap.udp://" + ip + ":3702"));

            _host.Description.Behaviors.Add(discoveryBehavior);

            // ** DISCOVERY ** //
            // add the discovery endpoint that specifies where to publish the services
            _host.Description.Endpoints.Add(new UdpDiscoveryEndpoint());

            _host.Open();

            if (_host.State == CommunicationState.Opened)
            {
                listeningStatusDisplay.Fill = Brushes.ForestGreen;
                _logger.Info("Server is listening now.");
            }
        }

        private void buttonLogOpen_Click(object sender, RoutedEventArgs e)
        {
            _logger.showLoggingWindow();
        }

        private void loadingButton_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "JSON File(*.json)|*.json";
            if (dialog.ShowDialog() == true)
            {
                _logger.Info("Loading savefile " + dialog.SafeFileName);
                _dataLayer.Load(dialog.FileName);
            }
        }

        private void _endStepButtonClick(object sender, RoutedEventArgs e)
        {
            _mainHandler.FinishStep();

            for (int i = 0; i < _labels.Count; i++)
            {
                if (_labels[i].Foreground == Brushes.Blue)
                    _labels[i].Foreground = Brushes.Black;
            }
            ordersDisplay.Fill = Brushes.Black;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _dataLayer.SetProperty("Test", "Money", 1000000000);
            _dataLayer.SetProperty("Test", "PowerHeavyIndustry", 100);
            _dataLayer.SetProperty("Test", "PowerLightIndustry", 100);
            _dataLayer.SetProperty("Test", "ResOil", 250);
            _dataLayer.SetProperty("Test", "ResSteel", 250);
            _dataLayer.SetProperty("Test", "ResWood", 250);
            _dataLayer.SetProperty("Test", "ResAgricultural", 250);
            _dataLayer.SetProperty("Test", "ExtractExperience", 5);
            _dataLayer.SetProperty("Test", "HeavyExperience", 5);
            _dataLayer.SetProperty("Test", "LightExperience", 5);
            _dataLayer.SetProperty("Test", "MilitaryExperience", 5);
            _dataLayer.SetProperty("Test", "NukesCount", 100);
            _dataLayer.SetProperty("Test", "IntelligenceLvl", 100);

            _dataLayer.SetProperty("Test2", "Money", 1000000000);
            _dataLayer.SetProperty("Test2", "PowerHeavyIndustry", 100);
            _dataLayer.SetProperty("Test2", "PowerLightIndustry", 100);
            _dataLayer.SetProperty("Test2", "ResOil", 250);
            _dataLayer.SetProperty("Test2", "ResSteel", 250);
            _dataLayer.SetProperty("Test2", "ResWood", 250);
            _dataLayer.SetProperty("Test2", "ResAgricultural", 250);
            _dataLayer.SetProperty("Test2", "ExtractExperience", 5);
            _dataLayer.SetProperty("Test2", "HeavyExperience", 5);
            _dataLayer.SetProperty("Test2", "LightExperience", 5);
            _dataLayer.SetProperty("Test2", "MilitaryExperience", 5);
            _dataLayer.SetProperty("Test2", "NukesCount", 100);

            _dataLayer.SetCurrencyOnStock("Test2", 2500000000);

            // _nukeHandler.StartAttack();
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            var points = new List<string>();
            var countries = _dataLayer.GetCountries();
           
            foreach (Country country in countries.Values)
            {
                long p = 0;
                p += country.Money/10;
                p += (long)(country.MilitaryPower * 10000);
                p += (long)(country.PowerHeavyIndustry * 10000);
                p += (long)(country.PowerLightIndustry * 10000);
                p += (long)(country.NukesCount * 50000);
                if (country.IsRiot)
                    p -= 1000000;

                points.Add(country.Name + " " + String.Format("{0:0,0}", p));
            }

            

            var pointsDial = new PointDialog(points);
            grid.Children.Add(pointsDial);
            pointsDial.Close += (object o) => grid.Children.Remove((UIElement)o);
        }
    }
}
