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

namespace Totality.Client.ClientComponents.Dialogs.Media
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class PropagandaDialog :  AbstractDialog, Dialog
    {
        private enum Orders { ChangePropDirection }
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        ReceiveOrder _receiveOrder;
        List<string> allCountries = new List<string>();

        public PropagandaDialog(ReceiveOrder receiveOrder)
        {
            _receiveOrder = receiveOrder;
            InitializeComponent();

            List<string> regimes = new List<string>{"Нейтральное", "Негативное", "Позитивное" };

            allCountries.Add(CountryData.Name);
            allCountries.AddRange(Countries);

            CountriesBox.ItemsSource = allCountries;
            RegimeBox.ItemsSource = regimes;

            CountriesBox.SelectedIndex = 0;

        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            Order order = new Order(CountryData.Name, CountriesBox.SelectedValue.ToString());
            order.OrderNum = (short)Orders.ChangePropDirection;
            order.Ministery = (short)Mins.Media;
            order.Value = (short)RegimeBox.SelectedIndex;
            _receiveOrder(this, order, "Смена направления пропаганды", 0);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null, null, 0);
        }

        private void CountriesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CountryData.MassMedia.ContainsKey(CountriesBox.SelectedValue.ToString()))
                RegimeBox.SelectedIndex = CountryData.MassMedia[CountriesBox.SelectedValue.ToString()];
            else RegimeBox.SelectedIndex = 0;
        }
    }
}
