using CommonClasses;
using Processors.Diplomatical;
using Processors.Main;
using Processors.News;
using Processors.Nuke;
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
using transmitterService;

namespace Server
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Transmitter transmitter = new Transmitter();
        MainProcessor mainProcessor;
        DiplomaticalProcessor dipProcessor;
        NewsProcessor newsProcessor;
        NukeProcessor nukeProcessor;
        Dictionary<string, Country> countries = new Dictionary<string, Country>();

        public MainWindow()
        {
            InitializeComponent();

            mainProcessor = new MainProcessor();
            nukeProcessor = new NukeProcessor(ref countries, ref transmitter);
            newsProcessor = new NewsProcessor();
            dipProcessor = new DiplomaticalProcessor();
        }

        private void startListening_Click(object sender, RoutedEventArgs e)
        {
            listeningStatusDisplay.Fill = Brushes.DarkGreen;
        }
    }
}
