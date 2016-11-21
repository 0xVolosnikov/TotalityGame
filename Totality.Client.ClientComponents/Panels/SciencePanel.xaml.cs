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
using Totality.Client.ClientComponents.Dialogs.Science;
using Totality.CommonClasses;

namespace Totality.Client.ClientComponents.Panels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class SciencePanel : AbstractPanel, InPanel
    {
        Dialog currentDialog;

        public SciencePanel()
        {
            InitializeComponent();
            ExtractButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Улучшить технологии добывающей промышленности?", "Улучшение технологий добывающей промышленности", ImproveDialog.Orders.ImproveExtract));
            HeavyButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Улучшить технологии тяжелой промышленности?", "Улучшение технологий тяжелой промышленности", ImproveDialog.Orders.ImproveHeavy));
            LightButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Улучшить технологии легкой промышленности?", "Улучшение технологий легкой промышленности", ImproveDialog.Orders.ImproveLight));
            MilitaryButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Улучшить военные технологии?", "Улучшение военных технологий", ImproveDialog.Orders.ImproveMilitary));
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
            ExtractLabel.Content = CountryData.ExtractScienceLvl;
            HeavyLabel.Content = CountryData.HeavyScienceLvl;
            LightLabel.Content = CountryData.LightScienceLvl;
            MilitaryLabel.Content = CountryData.MilitaryScienceLvl;

            ExtractLine.Height = (19 + 273 * (CountryData.ExtractExperience / (double)CountryData.ExtractScLvlUpExp));
            Canvas.SetTop(ExtractLine, 295 -  ExtractLine.Height);
            HeavyLine.Height = 19 + 273 * (CountryData.HeavyExperience / (double)CountryData.HeavyScLvlUpExp);
            Canvas.SetTop(HeavyLine, 295 - HeavyLine.Height);
            LightLine.Height = 19 + 273 * (CountryData.LightExperience / (double)CountryData.LightScLvlUpExp);
            Canvas.SetTop(LightLine, 295 - LightLine.Height);
            MilitaryLine.Height = 19 + 273 * (CountryData.MilitaryExperience / (double)CountryData.MilitaryScLvlUpExp);
            Canvas.SetTop(MilitaryLine, 295 - MilitaryLine.Height);

            if (CountryData.MinsBlocks[(short)Mins.Science] > 0 && !isBlocked)
            {
                isBlocked = true;
                deActivateButton(ExtractButton, "/Totality.Client.ClientComponents;component/Images/Science/ScienceExtractButtonDeactivated.png");
                deActivateButton(HeavyButton, "/Totality.Client.ClientComponents;component/Images/Science/ScienceHeavyButtonDeactivated.png");
                deActivateButton(LightButton, "/Totality.Client.ClientComponents;component/Images/Science/ScienceLightButtonDeactivated.png");
                deActivateButton(MilitaryButton, "/Totality.Client.ClientComponents;component/Images/Science/ScienceMilitaryButtonDeactivated.png");

            }
            else if (isBlocked && CountryData.MinsBlocks[(short)Mins.Science] == 0)
            {
                isBlocked = false;
                activateButton(ExtractButton, "/Totality.Client.ClientComponents;component/Images/Science/ScienceExtractButton.png");
                activateButton(HeavyButton, "/Totality.Client.ClientComponents;component/Images/Science/ScienceHeavyButton.png");
                activateButton(LightButton, "/Totality.Client.ClientComponents;component/Images/Science/ScienceLightButton.png");
                activateButton(MilitaryButton, "/Totality.Client.ClientComponents;component/Images/Science/ScienceMilitaryButton.png");
            }
        }
    }
}
