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
using Totality.Client.ClientComponents.Dialogs.Inner;
using Totality.CommonClasses;

namespace Totality.Client.ClientComponents.Panels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class InnerPanel : AbstractPanel, InPanel
    {
        Dialog currentDialog;

        public InnerPanel()
        {
            InitializeComponent();
            RepressionsButton.click += () => createDialog<RepressionsDialog>(new RepressionsDialog(receiveOrder));
            SuppressButton.click += () => createDialog<SuppressDialog>(new SuppressDialog(receiveOrder));
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
            if (CountryData.MinsBlocks[(short)Mins.Inner] > 0 && !isBlocked)
            {
                isBlocked = true;
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Inner/RepressionsButtonDeactivated.png", UriKind.Relative);
                RepressionsButton.imgUp = new BitmapImage(uriSource);
                RepressionsButton.Update();
                RepressionsButton.IsEnabled = false;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Inner/SuppressButtonDeactivated.png", UriKind.Relative);
                SuppressButton.imgUp = new BitmapImage(uriSource);
                SuppressButton.Update();
                SuppressButton.IsEnabled = false;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Inner/InnerLvlUpButtonDeactivated.png", UriKind.Relative);
                LvlupButton.imgUp = new BitmapImage(uriSource);
                LvlupButton.Update();
                LvlupButton.IsEnabled = false;
            }
            else if (isBlocked && CountryData.MinsBlocks[(short)Mins.Inner] == 0)
            {
                isBlocked = false;
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Inner/RepressionsButton.png", UriKind.Relative);
                RepressionsButton.imgUp = new BitmapImage(uriSource);
                RepressionsButton.Update();
                RepressionsButton.IsEnabled = true;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Inner/SuppressButton.png", UriKind.Relative);
                SuppressButton.imgUp = new BitmapImage(uriSource);
                SuppressButton.Update();
                SuppressButton.IsEnabled = true;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Inner/InnerLvlUpButton.png", UriKind.Relative);
                LvlupButton.imgUp = new BitmapImage(uriSource);
                LvlupButton.Update();
                LvlupButton.IsEnabled = true;
            }

            if (CountryData.IsRepressed)
            {
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Inner/RepressionsButtonActive.png", UriKind.Relative);
                RepressionsButton.imgUp = new BitmapImage(uriSource);
                RepressionsButton.Update();
            }
            else if (!isBlocked)
            {
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Inner/RepressionsButton.png", UriKind.Relative);
                RepressionsButton.imgUp = new BitmapImage(uriSource);
                RepressionsButton.Update();
            }
        }
    }
}
