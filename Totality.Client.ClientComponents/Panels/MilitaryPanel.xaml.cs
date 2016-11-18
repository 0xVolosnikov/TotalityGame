using Totality.Client.ClientComponents.Dialogs;
using Totality.Client.ClientComponents.Dialogs.Military;
using Totality.Client.ClientComponents.Dialogs.Common;
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

namespace Totality.Client.ClientComponents.Panels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class MilitaryPanel : AbstractPanel, InPanel
    {
        Dialog currentDialog;

        public MilitaryPanel()
        {
            InitializeComponent();
            
            NukeStrikeButton.click += () => createDialog<NukeStrikeDialog>(new NukeStrikeDialog(receiveOrder));
            WarButton.click += () => createDialog<WarDialog>(new WarDialog(receiveOrder));
            NukesButton.click += () => createDialog<NukesCountDialog>(new NukesCountDialog(receiveOrder));
            MissilesButton.click += () => createDialog<PROcountDialog>(new PROcountDialog(receiveOrder));
            UranusButton.click += () => createDialog<UranusDialog>(new UranusDialog(receiveOrder));
            MobilizationButton.click += () => createDialog<MobilizeDialog>(new MobilizeDialog(receiveOrder));
            
        }

        private void createDialog<T>(Dialog dialog) where T : UIElement
        {
            if (currentDialog == null)
            {
                currentDialog = dialog;
                canvas1.Children.Add((T)currentDialog);
                Canvas.SetLeft((T)currentDialog, 295);
                Canvas.SetTop((T)currentDialog, 68);
            }
        }

        public void receiveOrder(object sender, Order order, string text, long price)
        {
            if (order != null)
            Table.addOrder(new OrderRecord(text, price.ToString() , order));

            canvas1.Children.Remove((UIElement)sender);
            currentDialog = null;     
        }
    }
}
