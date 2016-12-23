using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Логика взаимодействия для IpDialog.xaml
    /// </summary>
    public partial class PointDialog : UserControl
    {
        public delegate void Closing(object sender);
        public event Closing Close;

        public PointDialog(List<string> points)
        {
            InitializeComponent();
            listBox.ItemsSource = points;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Close.Invoke(this);
        }
    }
}
