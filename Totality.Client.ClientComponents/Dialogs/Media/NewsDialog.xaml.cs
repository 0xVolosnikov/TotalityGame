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
using Microsoft.Research.DynamicDataDisplay;
using Totality.CommonClasses;
using Totality.Model;

namespace Totality.Client.ClientComponents.Dialogs.Media
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class NewsDialog :  AbstractDialog, Dialog
    {
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        ReceiveOrder _receiveOrder;
        List<News> _news;
        List<TextBlock> leftList = new List<TextBlock>();
        List<TextBlock> rightList = new List<TextBlock>();
        private ObservableCollection<OrderResult> _results = new ObservableCollection<OrderResult>();

        public NewsDialog(ReceiveOrder receiveOrder, List<News> news, OrderResult[] results)
        {
            _news = news;
            _receiveOrder = receiveOrder;
            InitializeComponent();

            resultsGrid.ItemsSource = _results;
            resultsGrid.LoadingRow += ResultsGridOnLoadingRow;

            if (results != null)
                _results.AddMany(results);

            stepLabel.Content = CurrentStep;

            foreach (News n in news.Where(x => x.IsOur))
            {
                var record = new System.Windows.Controls.TextBlock();
                record.Width = 486;
                record.FontSize = 21;
                var c = new FontFamilyConverter();
                record.FontFamily = (FontFamily)c.ConvertFrom("Aparajita");
                record.Text = n.text;
                record.TextWrapping = TextWrapping.Wrap;
                leftList.Add(record);
                Panel1.Children.Add(leftList.Last());
            }

            foreach (News n in news.Where(x => !x.IsOur))
            {
                var record = new System.Windows.Controls.TextBlock();
                record.Width = 468;
                record.FontSize = 21;
                var c = new FontFamilyConverter();
                record.FontFamily = (FontFamily)c.ConvertFrom("Aparajita");
                record.Text = n.text;
                record.TextWrapping = TextWrapping.Wrap;
                rightList.Add(record);
                Panel2.Children.Add(rightList.Last());
            }

            long profit = 0;
            long deficit = 0;
            foreach (var res in _results)
            {
                if (res.IsDone) deficit += res.Price;
            }

            profit = (long)(CountryData.FinalLightIndustry * Constants.LightPowerProfit * (CountryData.TaxesLvl / 100.0));
            deficitLabel.Content = deficit.ToString("N0");
            profitLabel.Content = profit.ToString("N0");

            long sum = profit - deficit;
            string sumText = sum.ToString("N0");
            if (sum > 0) sumText = "+ " + sumText;

            sumLabel.Content = sumText;
        }

        private void ResultsGridOnLoadingRow(object sender, DataGridRowEventArgs e)
        {
            OrderResult item = e.Row.Item as OrderResult;

            if (!item.IsDone) e.Row.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFD30000"));
            else e.Row.Background = Brushes.White;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null, null, 0);
        }
    }
}
