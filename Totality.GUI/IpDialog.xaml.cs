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
    public partial class IpDialog : UserControl
    {
        public delegate void GetIp(object sender, string ip);
        public event GetIp IpReceived;

        public IpDialog()
        {
            InitializeComponent();
            var hostName = System.Net.Dns.GetHostName();
            List<IPAddress> serverIps = new List<IPAddress>(System.Net.Dns.GetHostEntry(hostName).AddressList);
            comboBox.ItemsSource = serverIps;

            if (serverIps.Count > 0)
            comboBox.SelectedIndex = 0;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            IpReceived.Invoke(this, comboBox.SelectedValue.ToString());
        }
    }
}
