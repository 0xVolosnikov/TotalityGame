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

namespace Totality.Client.ClientComponents.Dialogs.Inner
{
    /// <summary>
    /// Логика взаимодействия для ImproveDialog.xaml
    /// </summary>
    public partial class RepressionsDialog : AbstractDialog, Dialog
    {
        private enum Orders { SuppressRiot, Repressions, EndRepressions, LvlUp }
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        ReceiveOrder _receiveOrder;

        public RepressionsDialog(ReceiveOrder receiveOrder)
        {
            _receiveOrder = receiveOrder;
            InitializeComponent();

            if (CountryData.IsRepressed)
            {
                textBlock.Text = "Отменить репрессии?";
            }
            else
            {
                textBlock.Text = "Начать репрессии?";
            }
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            Order order = new Order(CountryData.Name);
            order.Ministery = (short)Mins.Inner;

            if (CountryData.IsRepressed)
            {
                _receiveOrder(this, order, "Прекратить репрессии", 0);
                order.OrderNum = (short)Orders.EndRepressions;
            }
            else
            {
                _receiveOrder(this, order, "Начать репрессии", 0);
                order.OrderNum = (short)Orders.Repressions;
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null, null, 0);
        }
    }
}
