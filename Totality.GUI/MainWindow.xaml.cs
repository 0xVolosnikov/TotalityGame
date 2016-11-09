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
            _host = new ServiceHost(_transmitter);
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

                ServiceDiscoveryBehavior discoveryBehavior = new ServiceDiscoveryBehavior();
                // send announcements on UDP multicast transport
                discoveryBehavior.AnnouncementEndpoints.Add(
                  new UdpAnnouncementEndpoint());

                _host.Description.Behaviors.Add(new ServiceDiscoveryBehavior());

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
            catch (Exception ex)
            {
                _logger.Error("Failed opening host! " + ex.Message);
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
    }
}
