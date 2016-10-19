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

namespace Totality.LoggingSystem
{
    /// <summary>
    /// Логика взаимодействия для LoggingWindow.xaml
    /// </summary>
    public partial class LoggingWindow : Window
    {
        public bool needToClose = false;

        public LoggingWindow()
        {
            InitializeComponent();

            TextRange range = new TextRange(logOutBox.Document.ContentStart, logOutBox.Document.ContentEnd);
            range.ApplyPropertyValue(Paragraph.MarginProperty, new Thickness(0.5));

            Closing += LoggingWindow_Closing;
        }

        private void LoggingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!needToClose)
            {
                e.Cancel = true;
                this.Hide();
            }
            
        }

        public void CloseBackground()
        {
            needToClose = true;
            this.Close();
        }
    }
}
