using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Totality.Client.ClientComponents.Dialogs.Finance
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class CurrencyDialog : AbstractDialog, Dialog
    {
        public enum Orders { ChangeTaxes, PurchaseCurrency, SellCurrency, CurrencyInfusion };
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        ReceiveOrder _receiveOrder;
        private Dictionary<string, ObservableDataSource<DataPoint>> ratios = new Dictionary<string, ObservableDataSource<DataPoint>>();
        private Dictionary<string, long> accounts = new Dictionary<string, long>();
        private LineGraph pen;
        ObservableCollection<string> CountriesRatios = new ObservableCollection<string>();
        List<string> accountNames = new List<string>();

        struct DataPoint
        {
            public int step;
            public double ratio;
        }

        public CurrencyDialog(ReceiveOrder receiveOrder)
        {
            _receiveOrder = receiveOrder;
            InitializeComponent();

            CurrencyBox.ItemsSource = CountriesRatios;

            if (ratios.Any())
            {
                var rate = ratios[ratios.Keys.First()];
                rate.SetXYMapping((DataPoint p) => new Point(p.step, p.ratio));
                pen = new LineGraph(rate);
                Legend.SetDescription(pen, ratios.Keys.First());
                CurrencyPlotter.Legend.Visibility = Visibility.Hidden;
                CurrencyPlotter.Children.Add(pen);
            }

        }

        public void Update()
        {
            CountriesRatios.Clear();

            foreach (KeyValuePair<string, double> ration in CountryData.CurrencyRatios)
            {
                if (!ratios.ContainsKey(ration.Key))
                    ratios.Add(ration.Key, new ObservableDataSource<DataPoint>());

                string t = " " + String.Format("{0:0.##}", ration.Value);
                var e = ration.Key;
                e += t;
                CountriesRatios.Add(e);

                ratios[ration.Key].AppendAsync(Dispatcher, new DataPoint() { step = CurrentStep, ratio = ration.Value });

            }

            var accountRecords = new List<string>();
            accountNames.Clear();
            accounts = CountryData.CurrencyAccounts;

            foreach (KeyValuePair<string, long> account in accounts)
            {
                accountRecords.Add(account.Key + " " + String.Format("{0:0,0}", account.Value.ToString()));
                accountNames.Add(account.Key);
            }
            MoneyBox.ItemsSource = accountRecords;
            MoneyBox.UpdateLayout();
            if (accountRecords.Count > 0)
                MoneyBox.SelectedIndex = 0;
            if (CountriesRatios.Count > 0)
                CurrencyBox.SelectedIndex = 0;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void CurrencyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrencyBox.SelectedIndex != -1)
            {
                if (CurrencyPlotter.Children.Contains(pen))
                    CurrencyPlotter.Children.Remove(pen);
                ratios[Countries[CurrencyBox.SelectedIndex]].SetXYMapping((DataPoint p) => new Point(p.step, p.ratio));
                pen = new LineGraph(ratios[Countries[CurrencyBox.SelectedIndex]]);
                Legend.SetDescription(pen, Countries[CurrencyBox.SelectedIndex]);
                CurrencyPlotter.Legend.Visibility = Visibility.Hidden;
                CurrencyPlotter.Children.Add(pen);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void BuyButtonClick(object sender, RoutedEventArgs e)
        {
            if (MoneyBox.SelectedIndex != -1)
            {
                var dial = new CurrencyCountDialog(getOrderFromChildren, CountryData.CurrencyRatios[accountNames[MoneyBox.SelectedIndex]], CurrencyCountDialog.Orders.PurchaseCurrency, 0);
                canvas.Children.Add(dial);
                Canvas.SetLeft(dial, 295);
                Canvas.SetTop(dial, 68);
            }
        }

        private void SellButton_Click(object sender, RoutedEventArgs e)
        {
            if (MoneyBox.SelectedIndex != -1)
            {
                var dial = new CurrencyCountDialog(getOrderFromChildren, CountryData.CurrencyRatios[accountNames[MoneyBox.SelectedIndex]], CurrencyCountDialog.Orders.SellCurrency, (int)accounts[accountNames[MoneyBox.SelectedIndex]]);
                canvas.Children.Add(dial);
                Canvas.SetLeft(dial, 295);
                Canvas.SetTop(dial, 68);
            }

        }

        private void getOrderFromChildren(object sender, Order order)
        {
            canvas.Children.Remove((UIElement) sender);
            if (order != null)
            {
                order.CountryName = CountryData.Name;
                order.TargetCountryName = accountNames[MoneyBox.SelectedIndex];
                order.Ministery = (short)Mins.Finance;

                if (order.OrderNum == (short)Orders.PurchaseCurrency)
                    _receiveOrder(this, order, "Закупка валюты: " + accountNames[MoneyBox.SelectedIndex], (long)(CountryData.CurrencyRatios[accountNames[MoneyBox.SelectedIndex]] * order.Count));
                else
                {
                    _receiveOrder(this, order, "Продажа валюты: " + accountNames[MoneyBox.SelectedIndex], (long)(CountryData.CurrencyRatios[accountNames[MoneyBox.SelectedIndex]] * order.Count));
                }
            }
        }
    }
}
