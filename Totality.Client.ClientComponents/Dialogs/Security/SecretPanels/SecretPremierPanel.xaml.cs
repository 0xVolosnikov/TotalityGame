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

namespace Totality.Client.ClientComponents.Dialogs.SecretPanels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class SecretPremierPanel : SecretAbstractPanel, InPanel
    {
        Dialog currentDialog;
        public delegate void ReceiveOrder(object sender, Order order);
        ReceiveOrder _receiveOrder;

        public SecretPremierPanel(ReceiveOrder receiveOrder)
        {
            _receiveOrder = receiveOrder;
            InitializeComponent();
            ReorganizeButton.click += () => createDialog<ReorganizeDialog>(new ReorganizeDialog(SReceiveOrder));
            AlertButton.click += () => createDialog<AlertDialog>(new AlertDialog(SReceiveOrder));
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

        public void SReceiveOrder(object sender, Order order, string text, long price)
        {
            _receiveOrder(this, order);
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

            }
            else if (isBlocked && CountryData.MinsBlocks[(short)Mins.Premier] == 0)
            {
                isBlocked = false;
                activateButton(ReorganizeButton, "/Totality.Client.ClientComponents;component/Images/Premier/PremierReorganizeButton.png");
                activateButton(AlertButton, "/Totality.Client.ClientComponents;component/Images/Premier/PremierAlertButton.png");
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

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null);
        }
    }
}
