using Totality.Client.ClientComponents.Dialogs;
using Totality.Client.ClientComponents.Dialogs.Finance;
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
using Totality.Client.ClientComponents.Panels;

namespace Totality.Client.ClientComponents.Dialogs.SecretPanels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class SecretFinancePanel : SecretAbstractPanel, InPanel
    {
        private AbstractDialog _currentDialog;
        private CurrencyDialog _currencyDialog;
        public delegate void ReceiveOrder(object sender, Order order);
        ReceiveOrder _receiveOrder;

        public SecretFinancePanel(ReceiveOrder receiveOrder)
        {
            InitializeComponent();


            _receiveOrder = receiveOrder;

            CurrencyButton.click += () => createCurrencyDialog<CurrencyDialog>();
            InterventionButton.click += () => createDialog(new InterventionDialog(SReceiveOrder));
            TaxesButton.click += () => createDialog(new TaxesDialog(SReceiveOrder));
        }

        private void createDialog(AbstractDialog dialog)
        { 
            if (_currentDialog == null)
            {
                _currentDialog = dialog;
                canvas1.Children.Add(_currentDialog);
                Canvas.SetLeft(_currentDialog, (Width - dialog.Width)/2);
                Canvas.SetTop(_currentDialog, (Height - dialog.Height)/2 );
            }
        }

        private void createCurrencyDialog<T>() where T : UIElement
        {
            if (_currentDialog == null)
            {
                _currencyDialog = new CurrencyDialog(SReceiveOrder);
                canvas1.Children.Add(_currencyDialog);
                Canvas.SetLeft(_currencyDialog, (Width - _currencyDialog.Width) / 2);
                Canvas.SetTop(_currencyDialog, (Height - _currencyDialog.Height) / 2);
                _currencyDialog.Update();
            }
        }

        public void SReceiveOrder(object sender, Order order, string text, long price)
        {
                order.Value = order.OrderNum;
                order.OrderNum = (short)2;

            _receiveOrder(this, order);

                canvas1.Children.Remove((UIElement)sender);
                _currentDialog = null;
        }

        public void Update()
        {

            if (CountryData.MinsBlocks[(short)Mins.Finance] > 0)
            {
                isBlocked = true;
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Finance/CurrencyButtonDeactivated.png", UriKind.Relative);
                CurrencyButton.imgUp = new BitmapImage(uriSource);
                CurrencyButton.Update();
                CurrencyButton.IsEnabled = false;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Finance/InterventionButtonDeactivated.png", UriKind.Relative);
                InterventionButton.imgUp = new BitmapImage(uriSource);
                InterventionButton.Update();
                InterventionButton.IsEnabled = false;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Finance/TaxesButtonDownDeactivated.png", UriKind.Relative);
                TaxesButton.imgUp = new BitmapImage(uriSource);
                TaxesButton.Update();
                TaxesButton.IsEnabled = false;
            }
            else if (isBlocked)
            {
                isBlocked = false;
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Finance/CurrencyButton.png", UriKind.Relative);
                CurrencyButton.imgUp = new BitmapImage(uriSource);
                CurrencyButton.Update();
                CurrencyButton.IsEnabled = true;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Finance/InterventionButton.png", UriKind.Relative);
                InterventionButton.imgUp = new BitmapImage(uriSource);
                InterventionButton.Update();
                InterventionButton.IsEnabled = true;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Finance/TaxesButton.png", UriKind.Relative);
                TaxesButton.imgUp = new BitmapImage(uriSource);
                TaxesButton.Update();
                TaxesButton.IsEnabled = true;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null);
        }
    }
}
