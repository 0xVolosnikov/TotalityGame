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

namespace Totality.Client.ClientComponents.Dialogs.Foreign
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class IncomeDialog : AbstractDialog, Dialog
    {
        public delegate void SendDipMessage(object sender, DipMsg msg, Guid id, object button);
        SendDipMessage _sendMessage;
        List<DipMsg> _messages;
        UIElement currentDialog;

        public IncomeDialog(List<DipMsg> messages, SendDipMessage sendMessage)
        {
            _messages = messages;
            _sendMessage = sendMessage;
            InitializeComponent();

            _wrap.Children.Clear();
            var c = new FontFamilyConverter();

            for (int i = 0; i < _messages.Count(); i++)
            {
                var num = i;

                string text = "";

                switch (_messages[num].Type)
                {
                    case DipMsg.Types.Alliance:
                        text = "Союз";
                        break;
                    case DipMsg.Types.CurrencyAlliance:
                        text = "Валютный союз";
                        break;
                    case DipMsg.Types.Other:
                        text = "Другое";
                        break;
                    case DipMsg.Types.Trade:
                        text = "Торговля";
                        break;
                    case DipMsg.Types.MilitaryTraining:
                        text = "Военные учения";
                        break;
                    case DipMsg.Types.Peace:
                        text = "Мирный договор";
                        break;
                }

                var button = new System.Windows.Controls.Button()
                {
                    Background = null,
                    Content = _messages[num].From + ": " + text,
                    FontSize = 14,
                    FontFamily = (FontFamily)c.ConvertFrom("Arial black"),
                Cursor = Cursors.Hand,
                    ClickMode = ClickMode.Press
                };
                var mes = _messages[num];
                button.Click += (object sender, RoutedEventArgs e) => { createDialog(new SendDialog(receiveMessage, true, mes, button));};

                _wrap.Children.Add(button);
            }
        }

        private void receiveMessage(object sender, DipMsg msg, Guid id, object button)
        {
            currentDialog = null;

            canvas.Children.Remove((UIElement)sender);

            if (button != null)
                _wrap.Children.Remove((UIElement)button);

            if (_messages.Any(x => x.Id == id))
            _messages.Remove(_messages.First(x => x.Id == id));

                _sendMessage(this, msg, id, null);
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _sendMessage(this, null, Guid.Empty, null);
        }

        private void createDialog(AbstractDialog dialog)
        {
            if (currentDialog == null)
            {
                currentDialog = dialog;
                canvas.Children.Add(currentDialog);
                Canvas.SetLeft(currentDialog, (Width - dialog.Width)/2);
                Canvas.SetTop(currentDialog, (Height - dialog.Height) / 2);
            }
        }

    }
}
