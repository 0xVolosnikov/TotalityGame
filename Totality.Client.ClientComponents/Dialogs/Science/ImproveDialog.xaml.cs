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

namespace Totality.Client.ClientComponents.Dialogs.Science
{
    /// <summary>
    /// Логика взаимодействия для ImproveDialog.xaml
    /// </summary>
    public partial class ImproveDialog : AbstractDialog, Dialog
    {
        public enum Orders { ImproveExtract, ImproveHeavy, ImproveLight, ImproveMilitary }
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        ReceiveOrder _receiveOrder;
        private string _textOrder;
        private Orders _type;

        public ImproveDialog(ReceiveOrder receiveOrder, string textDial, string textOrder, Orders type)
        {
            _type = type;
            _receiveOrder = receiveOrder;
            _textOrder = textOrder;
            InitializeComponent();
            textBlock.Text = textDial;
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            Order order = new Order(CountryData.Name);
            order.Ministery = (short)Mins.Science;
            order.OrderNum = (short)_type;
            switch(_type)
            {
                case Orders.ImproveExtract:
                    _receiveOrder(this, order, _textOrder, CountryData.ExtractScLvlUpCost);
                    break;
                case Orders.ImproveHeavy:
                    _receiveOrder(this, order, _textOrder, CountryData.HeavyScLvlUpCost);
                    break;
                case Orders.ImproveLight:
                    _receiveOrder(this, order, _textOrder, CountryData.LightScLvlUpCost);
                    break;
                case Orders.ImproveMilitary:
                    _receiveOrder(this, order, _textOrder, CountryData.MilitaryScLvlUpCost);
                    break;
            }

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null, null, 0);
        }
    }
}
