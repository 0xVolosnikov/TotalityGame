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

namespace Totality.Client.ClientComponents.Dialogs.Industry
{
    /// <summary>
    /// Логика взаимодействия для ImproveDialog.xaml
    /// </summary>
    public partial class ImproveDialog : UserControl, Dialog
    {
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        ReceiveOrder _receiveOrder;
        private Country _country;
        private string _textOrder;

        public ImproveDialog(ReceiveOrder receiveOrder, Country country, string textDial, string textOrder)
        {
            _receiveOrder = receiveOrder;
            _country = country;
            _textOrder = textOrder;
            InitializeComponent();
            textBlock.Text = textDial;
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            Order order = new Order(_country.Name);
            _receiveOrder(this, order, _textOrder, 0);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null, null, 0);
        }
    }
}
