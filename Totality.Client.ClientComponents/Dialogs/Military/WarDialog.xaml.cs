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
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class WarDialog : AbstractDialog, Dialog
    {
        private enum Orders { GeneralMobilization, Demobilization, IncreaseUranium, MakeNukes, MakeMissiles, NukeStrike, StartWar }
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        private ReceiveOrder _receiveOrder;

        public WarDialog(ReceiveOrder receiveOrder)
        {
            _receiveOrder = receiveOrder;
            InitializeComponent();

            CountriesBox.ItemsSource = Countries;
            CountriesBox.SelectedIndex = 0;
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            Order order = new Order(CountryData.Name, CountriesBox.SelectedValue.ToString());
            order.OrderNum = (short)Orders.StartWar;
            order.Ministery = (short)Mins.Military;
            _receiveOrder(this, order, "Объявление войны стране: " + CountriesBox.SelectedValue.ToString(), 0);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null, null, 0);
        }
    }
}
