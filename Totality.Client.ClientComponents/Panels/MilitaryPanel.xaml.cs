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
using Totality.CommonClasses;

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

        public void Update()
        {
            UranusLabel.Content = (int)CountryData.ResUranus;

            if (CountryData.MinsBlocks[(short)Mins.Military] > 0 && !isBlocked)
            {
                isBlocked = true;
                deActivateButton(WarButton, "/Totality.Client.ClientComponents;component/Images/Military/WarButtonDeactivated.png");
                deActivateButton(MobilizationButton, "/Totality.Client.ClientComponents;component/Images/Military/WarButtonDeactivated.png");
                deActivateButton(MissilesButton, "/Totality.Client.ClientComponents;component/Images/Military/MissilesButtonDeactivated.png");
                deActivateButton(NukesButton, "/Totality.Client.ClientComponents;component/Images/Military/NukesButtonDeactivated.png");
                deActivateButton(UranusButton, "/Totality.Client.ClientComponents;component/Images/Military/UranusButtonDeactivated.png");

            }
            else if (isBlocked && CountryData.MinsBlocks[(short)Mins.Military] == 0)
            {
                isBlocked = false;
                activateButton(WarButton, "/Totality.Client.ClientComponents;component/Images/Military/WarButton.png");
                activateButton(MobilizationButton, "/Totality.Client.ClientComponents;component/Images/Military/WarButton.png");
                activateButton(MissilesButton, "/Totality.Client.ClientComponents;component/Images/Military/MissilesButton.png");
                activateButton(NukesButton, "/Totality.Client.ClientComponents;component/Images/Military/NukesButton.png");
                activateButton(UranusButton, "/Totality.Client.ClientComponents;component/Images/Military/UranusButton.png");
            }

            if (CountryData.IsMobilized)
            {
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Military/MobilizeButtonActive.png", UriKind.Relative);
                MobilizationButton.imgUp = new BitmapImage(uriSource);
                MobilizationButton.Update();
            }
            else if (!isBlocked)
            {
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Military/MobilizeButton.png", UriKind.Relative);
                MobilizationButton.imgUp = new BitmapImage(uriSource);
                MobilizationButton.Update();
            }
        }
    }
}
