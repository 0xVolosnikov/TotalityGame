using Totality.Client.ClientComponents.Dialogs;
using Totality.Client.ClientComponents.Dialogs.Military;
using Totality.Client.ClientComponents.Dialogs.Common;
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
            ExtractButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Улучшить технологии добывающей промышленности?", "Улучшение технологий добывающей промышленности"));
            HeavyButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Улучшить технологии тяжелой промышленности?", "Улучшение технологий тяжелой промышленности"));
            LightButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Улучшить технологии легкой промышленности?", "Улучшение технологий легкой промышленности"));
            MilitaryButton.click += () => createDialog<ImproveDialog>(new ImproveDialog(receiveOrder, "Улучшить военные технологии?", "Военных технологий"));
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
    }
}
