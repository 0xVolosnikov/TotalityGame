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

namespace Totality.Client.ClientComponents.Dialogs.Security
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class RecruitDialog : AbstractDialog, Dialog
    {
        private enum Orders { ImproveNetwork, AddAgents, OrderToAgent, Purge, CounterSpyLvlUp, ShadowingUp, IntelligenceUp, Sabotage }
        public delegate void ReceiveOrder(object sender, Order order);
        ReceiveOrder _receiveOrder;
        int _ministery;

        public RecruitDialog(ReceiveOrder receiveOrder, int ministery)
        {
            _ministery = ministery;
            _receiveOrder = receiveOrder;
            InitializeComponent();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            Order order = new Order(CountryData.Name);
            order.OrderNum = (short)Orders.AddAgents;
            order.Ministery = (short)Mins.Security;
            order.TargetMinistery = (short)_ministery;
            _receiveOrder(this, order);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null);
        }
    }
}
