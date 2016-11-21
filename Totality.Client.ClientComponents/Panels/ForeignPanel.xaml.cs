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
using Totality.Model.Diplomatical;
using Totality.Client.ClientComponents.Dialogs.Foreign;
using Totality.CommonClasses;

namespace Totality.Client.ClientComponents.Panels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class ForeignPanel : AbstractPanel, InPanel
    {
        Dialog currentDialog;

        public ForeignPanel()
        {
            InitializeComponent();
            SendButton.click += () => createDialog<Dialogs.Foreign.SendDialog>(new Dialogs.Foreign.SendDialog(SendDiplomaticalMessage));
            IncomeButton.click += () => createDialog<IncomeDialog>(new IncomeDialog());

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

        public void receiveOrder(object sender, Order order)
        {
            canvas1.Children.Remove((UIElement)sender);
            currentDialog = null;     
        }

        public void SendDiplomaticalMessage(object sender, DipMsg msg)
        {
            canvas1.Children.Remove((UIElement)sender);
            currentDialog = null;
        }

        public void Update()
        {
            if (CountryData.MinsBlocks[(short)Mins.Foreign] > 0 && !isBlocked)
            {
                isBlocked = true;
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Foreign/ForeignSendButtonDeactivated.png", UriKind.Relative);
                SendButton.imgUp = new BitmapImage(uriSource);
                SendButton.Update();
                SendButton.IsEnabled = false;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Foreign/ForeignInButtonDeactivated.png", UriKind.Relative);
                IncomeButton.imgUp = new BitmapImage(uriSource);
                IncomeButton.Update();
                IncomeButton.IsEnabled = false;
            }
            else if (isBlocked && CountryData.MinsBlocks[(short)Mins.Foreign] == 0)
            {
                isBlocked = false;
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Foreign/ForeignSendButton.png", UriKind.Relative);
                SendButton.imgUp = new BitmapImage(uriSource);
                SendButton.Update();
                SendButton.IsEnabled = true;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Foreign/ForeignInButton.png", UriKind.Relative);
                IncomeButton.imgUp = new BitmapImage(uriSource);
                IncomeButton.Update();
                IncomeButton.IsEnabled = true;
            }
        }
    }
}
