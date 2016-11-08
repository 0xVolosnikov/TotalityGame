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

namespace Totality.Client.ClientComponents.Dialogs.Finance
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class InvestDialog : UserControl, Dialog
    {
        public delegate void ReceiveOrder(Object sender, Order order);
        ReceiveOrder receiveOrder;
        Mins minister;

        public InvestDialog(ReceiveOrder receiveOrder, Mins minister)
        {
            this.receiveOrder = receiveOrder;
            this.minister = minister;
            InitializeComponent();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            receiveOrder(this, null);
        }
    }
}
