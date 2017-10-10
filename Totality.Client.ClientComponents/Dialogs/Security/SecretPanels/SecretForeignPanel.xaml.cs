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

namespace Totality.Client.ClientComponents.Dialogs.SecretPanels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class SecretForeignPanel : SecretAbstractPanel, InPanel
    {
        AbstractDialog currentDialog;
        public delegate void ReceiveOrder(object sender, Order order);
        ReceiveOrder _receiveOrder;
        private Dictionary<string, DipContract[]> _secretBase;

        public SecretForeignPanel(ReceiveOrder receiveOrder,  DipContract[] secretBase)
        {
            _receiveOrder = receiveOrder;
            InitializeComponent();

            ReceiveContracts(secretBase);
        }

        private void createDialog(AbstractDialog dialog)
        {
            if (currentDialog == null)
            {
                currentDialog = dialog;
                canvas1.Children.Add(currentDialog);
                Canvas.SetLeft(currentDialog, (Width - dialog.Width) / 2);
                Canvas.SetTop(currentDialog, (Height - dialog.Height) / 2);
            }
        }

        public void SReceiveOrder(object sender, Order order)
        {
            _receiveOrder(this, order);
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

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null);
        }

        public void ReceiveContracts(DipContract[] contracts)
        {
            _wrap.Children.Clear();
            var c = new FontFamilyConverter();

            for (int i = 0; i < contracts.Count(); i++)
            {
                var num = i;

                var button = new System.Windows.Controls.Button()
                {
                    Background = null,
                    Content = contracts[num].Description,
                    FontSize = 14,
                    FontFamily = (FontFamily)c.ConvertFrom("Segoe WP Black"),
                    Cursor = Cursors.Hand,
                    ClickMode = ClickMode.Press
                };

                button.Click += (object sender, RoutedEventArgs e) => { createDialog(new ContractDialog(SReceiveOrder, contracts[num])); };

                _wrap.Children.Add(button);
            }
        }
    }
}
