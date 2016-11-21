using Totality.Client.ClientComponents.Dialogs;
using Totality.Client.ClientComponents.Dialogs.Military;
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
using Totality.Client.ClientComponents.Dialogs.Premier;
using Totality.CommonClasses;

namespace Totality.Client.ClientComponents.Panels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class PremierPanel : AbstractPanel, InPanel
    {
        Dialog currentDialog;

        public PremierPanel()
        {
            InitializeComponent();
            ReorganizeButton.click += () => createDialog<ReorganizeDialog>(new ReorganizeDialog(receiveOrder));
            AlertButton.click += () => createDialog<AlertDialog>(new AlertDialog(receiveOrder));
            LvlupButton.click += () => createDialog<LvlupDialog>(new LvlupDialog(receiveOrder));
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

        public void Update()
        {
            if (CountryData.MinsBlocks[(short)Mins.Premier] > 0 && !isBlocked)
            {
                isBlocked = true;
                deActivateButton(ReorganizeButton, "/Totality.Client.ClientComponents;component/Images/Premier/PremierReorganizeButtonDeactivated.png");
                deActivateButton(AlertButton, "/Totality.Client.ClientComponents;component/Images/Premier/PremierAlertButtonDeactivated.png");
                deActivateButton(LvlupButton, "/Totality.Client.ClientComponents;component/Images/Premier/PremierLvlupButtonDeactivated.png");

            }
            else if (isBlocked && CountryData.MinsBlocks[(short)Mins.Premier] == 0)
            {
                isBlocked = false;
                activateButton(ReorganizeButton, "/Totality.Client.ClientComponents;component/Images/Premier/PremierReorganizeButton.png");
                activateButton(AlertButton, "/Totality.Client.ClientComponents;component/Images/Premier/PremierAlertButton.png");
                activateButton(LvlupButton, "/Totality.Client.ClientComponents;component/Images/Premier/PremierLvlupButton.png");
            }

            if (CountryData.IsAlerted)
            {
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Premier/PremierAlertButtonActive.png", UriKind.Relative);
                AlertButton.imgUp = new BitmapImage(uriSource);
                AlertButton.Update();
            }
            else if (!isBlocked)
            {
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Premier/PremierAlertButton.png", UriKind.Relative);
                AlertButton.imgUp = new BitmapImage(uriSource);
                AlertButton.Update();
            }
        }
    }
}
