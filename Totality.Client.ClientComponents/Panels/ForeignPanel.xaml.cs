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
using Totality.Client.ClientComponents.ServiceReference1;

namespace Totality.Client.ClientComponents.Panels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class ForeignPanel : AbstractPanel, InPanel
    {
        public TransmitterServiceClient _client;
        AbstractDialog currentDialog;
        List<DipMsg> _messages = new List<DipMsg>();

        public ForeignPanel()
        {
            InitializeComponent();
            SendButton.click += () => createDialog(new Dialogs.Foreign.SendDialog(SendDiplomaticalMessage, false));
            IncomeButton.click += () => createDialog(new IncomeDialog(_messages, SendDiplomaticalMessage));

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

        public void receiveOrder(object sender, Order order)
        {
            canvas1.Children.Remove((UIElement)sender);
            currentDialog = null;

            if (order != null)
            {
                order.Ministery = (short)Mins.Foreign;
                Table.addOrder(new OrderRecord("Расторжение договора со страной " + order.TargetCountryName, "0", order));
            }
        }

        public void SendDiplomaticalMessage(object sender, DipMsg msg, Guid id, object button = null)
        {
            if (id == Guid.Empty)
            {
                canvas1.Children.Remove((UIElement)sender);
                currentDialog = null;
            }
            if (msg != null)
                _client.DipMsg(msg);

            if (id != Guid.Empty && _messages.Any(x=>x.Id == id))
                _messages.Remove(_messages.First(x => x.Id == id));
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

        public void ReceiveMessage(DipMsg msg)
        {
            _messages.Add(msg);
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

                button.Click += (object sender, RoutedEventArgs e) => { createDialog(new ContractDialog(receiveOrder, contracts[num])); };

                _wrap.Children.Add(button);
            }
        }
    }
}
