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
using Totality.Client.ClientComponents.Dialogs.Security;
using Totality.CommonClasses;
using Totality.Client.ClientComponents.ServiceReference1;
using Totality.Model.Diplomatical;

namespace Totality.Client.ClientComponents.Panels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class SecurityPanel : AbstractPanel, InPanel
    {
        Dialog currentDialog;
        public TransmitterServiceClient _client;

        public SecurityPanel()
        {
            InitializeComponent();
            PurgeButton.click += () => createDialog<PurgeDialog>(new PurgeDialog(receiveOrder));
            CounterspyButton.click += () => createDialog<CounterspyDialog>(new CounterspyDialog(receiveOrder));
            ShadowingButton.click += () => createDialog<ShadowingDialog>(new ShadowingDialog(receiveOrder));
            NetsButton.click += () => createBigDialog<NetsDialog>(new NetsDialog(receiveOrder, _client));
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

        private void createBigDialog<T>(Dialog dialog) where T : UIElement
        {
            if (currentDialog == null)
            {
                currentDialog = dialog;
                canvas1.Children.Add((T)currentDialog);
                Canvas.SetLeft((T)currentDialog, (Width - ((UserControl)currentDialog).Width)/2.0 );
                Canvas.SetTop((T)currentDialog, (41 + Height - ((UserControl)currentDialog).Height) / 2.0);
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

            if (CountryData.MinsBlocks[(short)Mins.Security] > 0 && !isBlocked)
            {
                isBlocked = true;
                deActivateButton(NetsButton, "/Totality.Client.ClientComponents;component/Images/Security/SecurityNetsButtonDeactivated.png");
                deActivateButton(ShadowingButton, "/Totality.Client.ClientComponents;component/Images/Security/SecurityShadowingButtonDeactivated.png");
                deActivateButton(CounterspyButton, "/Totality.Client.ClientComponents;component/Images/Security/SecurityCounterspyButtonDeactivated.png");
                deActivateButton(PurgeButton, "/Totality.Client.ClientComponents;component/Images/Security/SecurityPurgeButtonDeactivated.png");

            }
            else if (isBlocked && CountryData.MinsBlocks[(short)Mins.Security] == 0)
            {
                isBlocked = false;
                activateButton(NetsButton, "/Totality.Client.ClientComponents;component/Images/Security/SecurityNetsButton.png");
                activateButton(ShadowingButton, "/Totality.Client.ClientComponents;component/Images/Security/SecurityShadowingButton.png");
                activateButton(CounterspyButton, "/Totality.Client.ClientComponents;component/Images/Security/SecurityCounterspyButton.png");
                activateButton(PurgeButton, "/Totality.Client.ClientComponents;component/Images/Security/SecurityPurgeButton.png");
            }
        }
    }
}
