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
    public partial class NukeStrikeDialog : AbstractDialog, Dialog
    {
        private enum Orders { GeneralMobilization, Demobilization, IncreaseUranium, MakeNukes, MakeMissiles, NukeStrike, StartWar }
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        ReceiveOrder _receiveOrder;

        public NukeStrikeDialog(ReceiveOrder receiveOrder)
        {
            _receiveOrder = receiveOrder;
            InitializeComponent();

            CountriesBox.ItemsSource = Countries;
            CountriesBox.SelectedIndex = 0;
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            //receiveOrder(this, new Order((int)Ministers.MinDef, "strike", new List<int>{ 0, 1 }));
            canvas.Children.Add(new NukeStrikeCountDialog(receiveOrderFromChildren));
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null, null, 0);
        }

        public void receiveOrderFromChildren(object sender, Order order)
        {
            order.CountryName = CountryData.Name;
            order.TargetCountryName = (string)CountriesBox.SelectedItem;
            order.Ministery = (short)Mins.Military;
            _receiveOrder(this, order, "Ядерный удар", 0);
        }
    }
}
