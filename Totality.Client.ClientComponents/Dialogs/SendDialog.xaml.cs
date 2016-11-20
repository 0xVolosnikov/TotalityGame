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
using Totality.CommonClasses;
using Totality.Model;

namespace Totality.Client.ClientComponents.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class SendDialog : AbstractDialog, Dialog
    {
        private enum Orders { ImproveNetwork, AddAgents, OrderToAgent, Purge, CounterSpyLvlUp, ShadowingUp, IntelligenceUp, Sabotage }
        public delegate void SendOrders(bool allowed, SendDialog sender);
        SendOrders _sendOrders;

        public SendDialog(SendOrders sendOrders)
        {
            _sendOrders = sendOrders;
            InitializeComponent();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            _sendOrders(true, this);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _sendOrders(false, this);
        }
    }
}
