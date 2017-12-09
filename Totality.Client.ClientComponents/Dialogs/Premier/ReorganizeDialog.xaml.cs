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

namespace Totality.Client.ClientComponents.Dialogs.Premier
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class ReorganizeDialog : AbstractDialog, Dialog
    {
        private enum Orders { MinisteryReorganization, LvlUp, Alert, UnAlert }
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        ReceiveOrder _receiveOrder;

        public ReorganizeDialog(ReceiveOrder receiveOrder)
        {
            _receiveOrder = receiveOrder;
            InitializeComponent();

            MinistersBox.ItemsSource = Ministers;
            MinistersBox.SelectedIndex = 0;
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            Order order = new Order(CountryData.Name);
            order.TargetMinistery = (short)MinistersBox.SelectedIndex;
            order.OrderNum = (short)Orders.MinisteryReorganization;
            order.Ministery = (short)Mins.Premier;
            _receiveOrder(this, order, "Реорганизация министерства: " + MinistersBox.SelectedItem, (long)(CountryData.InflationCoeff*600000));
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null, null, 0);
        }
    }
}
