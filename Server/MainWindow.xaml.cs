﻿using Totality.CommonClasses;
using Totality.Processors.Diplomatical;
using Totality.Processors.Main;
using Totality.Processors.News;
using Totality.Processors.Nuke;
using Totality.transmitterService;
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


namespace Totality.GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Transmitter _transmitter = new Transmitter();
        private MainProcessor _mainProcessor;
        private DiplomaticalProcessor _dipProcessor;
        private NewsProcessor _newsProcessor;
        private NukeProcessor _nukeProcessor;
        private Dictionary<string, Country> _countries = new Dictionary<string, Country>();

        public MainWindow()
        {
            InitializeComponent();

            _mainProcessor = new MainProcessor();
            _nukeProcessor = new NukeProcessor(ref _countries, ref _transmitter);
            _newsProcessor = new NewsProcessor();
            _dipProcessor = new DiplomaticalProcessor();
        }

        private void startListening_Click(object sender, RoutedEventArgs e)
        {
            listeningStatusDisplay.Fill = Brushes.DarkGreen;
        }
    }
}
