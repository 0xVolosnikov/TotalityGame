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

            ServiceDiscoveryBehavior discoveryBehavior = new ServiceDiscoveryBehavior();
            // send announcements on UDP multicast transport
            discoveryBehavior.AnnouncementEndpoints.Add(
              //new UdpAnnouncementEndpoint("soap.udp://192.168.1.255:3702"));
              new UdpAnnouncementEndpoint());
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
    }
}
