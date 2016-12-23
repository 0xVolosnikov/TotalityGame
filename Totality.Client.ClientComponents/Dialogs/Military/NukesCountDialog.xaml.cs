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

namespace Totality.Client.ClientComponents.Dialogs.Military
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeCountDialog.xaml
    /// </summary>
    public partial class NukesCountDialog : AbstractDialog, Dialog
    {
        private enum Orders { GeneralMobilization, Demobilization, IncreaseUranium, MakeNukes, MakeMissiles, NukeStrike, StartWar }
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        private ReceiveOrder _receiveOrder;

        public NukesCountDialog(ReceiveOrder receiveOrder)
        {
            _receiveOrder = receiveOrder;
            InitializeComponent();
            var b = Math.Max(CountryData.Money / Constants.NukeCost, (CountryData.FinalHeavyIndustry - CountryData.UsedHIpower) / Constants.NukeHeavyPower);
            b = Math.Max(b, CountryData.ResUranus / Constants.NukeUranusCost);

            integerUpDown.Maximum = (int)b;
            
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            Order order = new Order(CountryData.Name);
            order.Count = (long)integerUpDown.Value;
            order.OrderNum = (short)Orders.MakeNukes;
            order.Ministery = (short)Mins.Military;
            _receiveOrder(this, order, "Производство ядерных ракет", Constants.NukeCost * order.Count);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null, null, 0);
        }
    }
}
