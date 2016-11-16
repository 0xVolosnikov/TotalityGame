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
using Totality.Client.ClientComponents.Dialogs.Industry;

namespace Totality.Client.ClientComponents
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class IndustryPanel : UserControl, InPanel
    {
        Dialog currentDialog;
        public OrdersTable Table;
        public Country CountryData;

        public IndustryPanel()
        {
            InitializeComponent();
            HeavyButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, CountryData, "Повысить тяжелую промышленную мощь?", "Повышение тяжелой промышленной мощи") );
            LightButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, CountryData, "Повысить легкую промышленную мощь?", "Повышение легкой промышленной мощи"));
            OilButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, CountryData, "Повысить производство нефти?", "Повышение производства нефти"));
            SteelButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, CountryData, "Повысить производство стали?", "Повышение производства стали"));
            WoodButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, CountryData, "Повысить производство древесины?", "Повышение производства древесины"));
            AgroButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, CountryData, "Повысить производство с/х продукции?", "Повышение производства с/х продукции"));
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
                Table.addOrder(new OrderRecord(text, price.ToString(), order));

            canvas1.Children.Remove((UIElement)sender);
            currentDialog = null;
        }
    }
}
