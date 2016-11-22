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
using Totality.Model;

namespace Totality.Client.ClientComponents.Dialogs.Finance
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeCountDialog.xaml
    /// </summary>
    public partial class CurrencyCountDialog : AbstractDialog, Dialog
    {
        public enum Orders { ChangeTaxes, PurchaseCurrency, SellCurrency, CurrencyInfusion };
        public delegate void ReceiveOrder(object sender, Order order);
        ReceiveOrder receiveOrder;
        Orders _type;
        double _ratio = 1;

        public CurrencyCountDialog(ReceiveOrder receiveOrder, double ratio, Orders type, int max)
        {
            this.receiveOrder = receiveOrder;
            _type = type;
            _ratio = ratio;

            InitializeComponent();

            integerUpDown.ValueChanged += IntegerUpDown_ValueChanged;

            if (type == Orders.PurchaseCurrency)
                integerUpDown.Maximum = (int)(CountryData.Money / ratio);
            else
            {
                integerUpDown.Maximum = max;
            }
        }

        private void IntegerUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            CostLabel.Content = String.Format("{0:0,#}", (integerUpDown.Value * _ratio));
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            receiveOrder(this, new Order("", "") { Count = (int)integerUpDown.Value , OrderNum = (short)_type});
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            receiveOrder(this, null);
        }
    }
}
