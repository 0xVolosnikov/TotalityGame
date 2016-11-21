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

namespace Totality.Client.ClientComponents.Panels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class FinancePanel : AbstractPanel, InPanel
    {
        private Dialog _currentDialog;

        public FinancePanel()
        {
            InitializeComponent();
            
            CurrencyButton.click += () => createDialog<CurrencyDialog>(new CurrencyDialog(receiveOrder));
            InterventionButton.click += () => createDialog<InterventionDialog>(new InterventionDialog(receiveOrder));
            TaxesButton.click += () => createDialog<TaxesDialog>(new TaxesDialog(receiveOrder));
        }

        private void createDialog<T>(Dialog dialog) where T : UIElement
        {
            if (_currentDialog == null)
            {
                _currentDialog = dialog;
                canvas1.Children.Add((T)_currentDialog);
                Canvas.SetLeft((T)_currentDialog, 295);
                Canvas.SetTop((T)_currentDialog, 68);
            }
        }

        public void receiveOrder(object sender, Order order, string text, long price)
        {
            if (order != null)
                Table.addOrder(new OrderRecord(text, price.ToString(), order));

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
    }
}
