using Totality.CommonClasses;
using Totality.Processors;
using Totality.Processors.Diplomatical;
using Totality.Processors.Main;
using Totality.Processors.News;
using Totality.Processors.Nuke;
using Totality.TransmitterService;
using Totality.Model;
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
using Totality.Model.Interfaces;

namespace Totality.GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LoggingSystem.Logger _logger = new LoggingSystem.Logger();
        private ITransmitter _transmitter;
        private IDataLayer _dataLayer = new DataLayer.DataLayer();
        private MainProcessor _mainProcessor;
        private DiplomaticalProcessor _dipProcessor;
        private NewsProcessor _newsProcessor;
        private NukeProcessor _nukeProcessor;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;

            _transmitter = new Transmitter(_logger);
            _mainProcessor = new MainProcessor(_dataLayer, _logger);
            _nukeProcessor = new NukeProcessor( _transmitter, _dataLayer, _logger);
            _newsProcessor = new NewsProcessor();
            _dipProcessor = new DiplomaticalProcessor(_dataLayer, _logger);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _logger.killLoggingWindow();
        }

        private void startListening_Click(object sender, RoutedEventArgs e)
        {
            listeningStatusDisplay.Fill = Brushes.DarkGreen;
        }

        private void buttonLogOpen_Click(object sender, RoutedEventArgs e)
        {
            _logger.showLoggingWindow();
        }
    }
}
