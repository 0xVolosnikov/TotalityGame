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

namespace Totality.Client.ClientComponents.Dialogs.Military
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class NukeStrikeDialog : UserControl, Dialog
    {
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        ReceiveOrder receiveOrder;

        public NukeStrikeDialog(ReceiveOrder receiveOrder )
        {
            this.receiveOrder = receiveOrder;
            InitializeComponent();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            //receiveOrder(this, new Order((int)Ministers.MinDef, "strike", new List<int>{ 0, 1 }));
            canvas.Children.Add(new NukeStrikeCountDialog(receiveOrderFromChildren));
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        public void receiveOrderFromChildren(object sender, Order order)
        {
            order.CountryName = "Test";
            order.TargetCountryName = "Test2";
            receiveOrder(this, order, "Ядерный удар", 0);
        }
    }
}
