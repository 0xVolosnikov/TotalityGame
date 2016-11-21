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
    public partial class AlertDialog : AbstractDialog, Dialog
    {
        private enum Orders { MinisteryReorganization, LvlUp, Alert, UnAlert }
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        ReceiveOrder _receiveOrder;

        public AlertDialog(ReceiveOrder receiveOrder)
        {
            _receiveOrder = receiveOrder;
            InitializeComponent();

            if (CountryData.IsAlerted)
            {
                textBlock.Text = "Отменить чрезвычайное положение?";
            }
            else
            {
                textBlock.Text = "Объявить чрезвычайное положение?";
            }
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            Order order = new Order(CountryData.Name);
            order.Ministery = (short)Mins.Premier;
            if (CountryData.IsAlerted)
            {
                order.OrderNum = (short)Orders.UnAlert;
                _receiveOrder(this, order, "Отмена чрезвычайного положения", 0);
            }
            else
            {
                order.OrderNum = (short)Orders.Alert;
                _receiveOrder(this, order, "Объявление чрезвычайного положения", 0);
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null, null, 0);
        }
    }
}
