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
using Totality.Client.ClientComponents.Dialogs.Industry;
using Totality.CommonClasses;

namespace Totality.Client.ClientComponents.Panels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class IndustryPanel : AbstractPanel, InPanel
    {
        Dialog currentDialog;

        public IndustryPanel()
        {
            InitializeComponent();
            HeavyButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Повысить тяжелую промышленную мощь?", "Повышение тяжелой промышленной мощи", ImproveDialog.Orders.ImproveHeavy ) );
            LightButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Повысить легкую промышленную мощь?", "Повышение легкой промышленной мощи", ImproveDialog.Orders.ImproveLight));
            OilButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Повысить производство нефти?", "Повышение производства нефти", ImproveDialog.Orders.IncreaseOil));
            SteelButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Повысить производство стали?", "Повышение производства стали", ImproveDialog.Orders.IncreaseSteel));
            WoodButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Повысить производство древесины?", "Повышение производства древесины", ImproveDialog.Orders.IncreaseWood));
            AgroButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Повысить производство с/х продукции?", "Повышение производства с/х продукции", ImproveDialog.Orders.IncreaseAgricultural));
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
            HeavyLabel.Content = (int)CountryData.FinalHeavyIndustry;
            LightLabel.Content = (int)CountryData.FinalLightIndustry;
            OilLabel.Content = (int)CountryData.FinalOil;
            SteelLabel.Content = (int)CountryData.FinalSteel;
            WoodLabel.Content = (int)CountryData.FinalWood;
            AgroLabel.Content = (int)CountryData.FinalAgricultural;
            
            OilLine.Width = 320 * (CountryData.UsedOil / CountryData.FinalOil);
            SteelLine.Width = 320 * (CountryData.UsedSteel / CountryData.FinalSteel);
            WoodLine.Width = 320 * (CountryData.UsedWood / CountryData.FinalWood);
            AgroLine.Width = 320 * (CountryData.UsedAgricultural / CountryData.FinalAgricultural);

            UsedOilLabel.Content = (int)CountryData.UsedOil;
            UsedSteelLabel.Content = (int)CountryData.UsedSteel;
            UsedWoodLabel.Content = (int)CountryData.UsedWood;
            UsedAgroLabel.Content = (int)CountryData.UsedAgricultural;

            if (OilLine.Width > UsedOilLabel.Width)
            Canvas.SetLeft(UsedOilLabel, Canvas.GetLeft(OilLine) + (OilLine.Width - UsedOilLabel.Width)/2.0);
            else
            Canvas.SetLeft(UsedOilLabel, Canvas.GetLeft(OilLine));

            if (SteelLine.Width > UsedSteelLabel.Width)
                Canvas.SetLeft(UsedSteelLabel, Canvas.GetLeft(SteelLine) + (SteelLine.Width - UsedSteelLabel.Width) / 2.0);
            else
                Canvas.SetLeft(UsedSteelLabel, Canvas.GetLeft(SteelLine));

            if (WoodLine.Width > UsedWoodLabel.Width)
                Canvas.SetLeft(UsedWoodLabel, Canvas.GetLeft(WoodLine) + (WoodLine.Width - UsedWoodLabel.Width) / 2.0);
            else
                Canvas.SetLeft(UsedWoodLabel, Canvas.GetLeft(WoodLine));

            if (AgroLine.Width > UsedAgroLabel.Width)
                Canvas.SetLeft(UsedAgroLabel, Canvas.GetLeft(AgroLine) + (AgroLine.Width - UsedAgroLabel.Width) / 2.0);
            else
                Canvas.SetLeft(UsedAgroLabel, Canvas.GetLeft(AgroLine));


            if (CountryData.MinsBlocks[(short)Mins.Industry] > 0 && !isBlocked)
            {
                isBlocked = true;
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Panels/Images/Industry/heavyIndustryButtonDeactivated.png", UriKind.Relative);
                HeavyButton.imgUp = new BitmapImage(uriSource);
                HeavyButton.Update();
                HeavyButton.IsEnabled = false;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Panels/Images/Industry/lightIndustryButtonDeactivated.png", UriKind.Relative);
                LightButton.imgUp = new BitmapImage(uriSource);
                LightButton.Update();
                LightButton.IsEnabled = false;

                OilButton.IsEnabled = false;
                SteelButton.IsEnabled = false;
                WoodButton.IsEnabled = false;
                AgroButton.IsEnabled = false;
            }
            else if (isBlocked && CountryData.MinsBlocks[(short)Mins.Industry] == 0)
            {
                isBlocked = false;
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Panels/Images/Industry/heavyIndustryButton.png", UriKind.Relative);
                HeavyButton.imgUp = new BitmapImage(uriSource);
                HeavyButton.Update();
                HeavyButton.IsEnabled = true;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Panels/Images/Industry/lightIndustryButton.png", UriKind.Relative);
                LightButton.imgUp = new BitmapImage(uriSource);
                LightButton.Update();
                LightButton.IsEnabled = true;

                OilButton.IsEnabled = true;
                SteelButton.IsEnabled = true;
                WoodButton.IsEnabled = true;
                AgroButton.IsEnabled = true;
            }
        }
    }
}
