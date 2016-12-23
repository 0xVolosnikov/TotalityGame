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
    public partial class SendDialog : AbstractDialog, Dialog
    {
        public delegate void SendDipMessage(object sender, DipMsg msg, Guid id, object button);
        SendDipMessage _sendMessage;
        bool _isReceive;
        DipMsg.Types _type;
        DipMsg _dipMsg;
        object _button;

        public SendDialog(SendDipMessage sendMessage, bool receive, DipMsg msg = null, object button = null)
        {
            _sendMessage = sendMessage;
            _isReceive = receive;
            _dipMsg = msg;
            _button = button;

            InitializeComponent();

            List<string> types = new List<string>();
            types.Add("Торговля");
            types.Add("Мирный договор");
            types.Add("Военный альянс");
            //types.Add("Валютный союз");
            types.Add("Военные учения");
            types.Add("Денежный перевод");
            types.Add("Другое");

            List<string> resources = new List<string>();
            resources.Add("Нефть");
            resources.Add("Сталь");
            resources.Add("Древесина");
            resources.Add("Сельхоз продукция");

            TradeResBox.ItemsSource = resources;
            TradeResBox.SelectedIndex = 0;

            TransferUpDown.Maximum = (int)CountryData.Money;

            AllianceCanvas.Visibility = Visibility.Hidden;
            otherCanvas.Visibility = Visibility.Hidden;
            transferCanvas.Visibility = Visibility.Hidden;
            TradeCanvas.Visibility = Visibility.Hidden;

            if (receive)
            {

                switch (msg.Type)
                {
                    case DipMsg.Types.Trade:
                        TradeCanvas.Visibility = Visibility.Visible;
                        TradeCountUpdown.Value = (int)msg.Count;
                        TradePriceIntUpDown.Value = (int)msg.Price;
                        TradeResBox.SelectedItem = msg.Resource;
                        TradeTimeUpdown.Value = msg.Time;
                        break;
                    case DipMsg.Types.Peace:
                        break;
                    case DipMsg.Types.Alliance:
                        AllianceCanvas.Visibility = Visibility.Visible;
                        AllianceTextBox.Text = msg.Text;
                        break;
                    case DipMsg.Types.CurrencyAlliance:
                        break;
                    case DipMsg.Types.MilitaryTraining:
                        break;
                    case DipMsg.Types.Transfer:
                        transferCanvas.Visibility = Visibility.Visible;
                        TransferUpDown.Value = (int)msg.Count;
                        break;
                    case DipMsg.Types.Other:
                        otherCanvas.Visibility = Visibility.Visible;
                        otherTextBox.Text = msg.Text;
                        break;
                }

                CountriesBox.ItemsSource = Countries;
                TypesBox.ItemsSource = types;

                CountriesBox.SelectedItem = msg.From;
                TypesBox.SelectedIndex = (short)msg.Type;

                AllianceCanvas.IsEnabled = false;
                otherCanvas.IsEnabled = false;
                transferCanvas.IsEnabled = false;
                TradeCanvas.IsEnabled = false;
                CountriesBox.IsEnabled = false;
                TypesBox.IsEnabled = false;

                cancelButton.Content = "Отклонить";

            }
            else
            {
                AllianceCanvas.IsEnabled = true;
                otherCanvas.IsEnabled = true;
                transferCanvas.IsEnabled = true;
                TradeCanvas.IsEnabled = true;
                CountriesBox.IsEnabled = true;
                TypesBox.IsEnabled = true;

                if (CountryData.Alliance != CountryData.Name)
                {
                    AllianceTextBox.Text = CountryData.Alliance;
                    AllianceTextBox.IsEnabled = false;
                }

                CountriesBox.ItemsSource = Countries;
                if (Countries.Count > 0)
                    CountriesBox.SelectedIndex = 0;
                TypesBox.ItemsSource = types;
                TypesBox.SelectedIndex = 0;
            }

        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isReceive)
            {
                _dipMsg.Applied = true;
                _sendMessage(this, _dipMsg, _dipMsg.Id, _button); // {Type}
            }
            else
            {
                _dipMsg = new DipMsg(CountryData.Name, (string)CountriesBox.SelectedItem);
                _dipMsg.Type = _type;
                switch (_type)
                {
                    case DipMsg.Types.Trade:
                        _dipMsg.Count = (long)TradeCountUpdown.Value;
                        _dipMsg.Price = (long)TradePriceIntUpDown.Value;
                        _dipMsg.Time = (int)TradeTimeUpdown.Value;
                        switch ((string)TradeResBox.SelectedItem)
                        {
                            case "Нефть":
                                _dipMsg.Resource = "Oil";
                                break;
                            case "Сталь":
                                _dipMsg.Resource = "Steel";
                                break;
                            case "Древесина":
                                _dipMsg.Resource = "Wood";
                                break;
                            case "Сельхоз продукция":
                                _dipMsg.Resource = "Agricultural";
                                break;
                        }
                        _dipMsg.Description = "Торговый договор";
                        break;
                    case DipMsg.Types.Peace:
                        _dipMsg.Description = "Мирный договор";
                        break;
                    case DipMsg.Types.Alliance:
                        _dipMsg.Text = AllianceTextBox.Text;
                        _dipMsg.Description = "Союзный договор";
                        break;
                    case DipMsg.Types.CurrencyAlliance:
                        _dipMsg.Description = "Договор о валютном союзе";
                        break;
                    case DipMsg.Types.MilitaryTraining:
                        _dipMsg.Description = "Военные учения";
                        break;
                    case DipMsg.Types.Transfer:
                        _dipMsg.Description = "Денежный перевод";
                        _dipMsg.Count = (long)TransferUpDown.Value;
                        break;
                    case DipMsg.Types.Other:
                        _dipMsg.Description = "Другое";
                        _dipMsg.Text = otherTextBox.Text;
                        break;
                }

                if (_button != null)
                    _sendMessage(_button, null, Guid.Empty, null);
                _sendMessage(this, _dipMsg, Guid.Empty, null);
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isReceive)
            {
                if (_button != null)
                    _sendMessage(this, null, _dipMsg.Id, _button);
            }
            else
            {
                _sendMessage(this, null, Guid.Empty, null);
            }
        }

        private void TypesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AllianceCanvas.Visibility = Visibility.Hidden;
            otherCanvas.Visibility = Visibility.Hidden;
            transferCanvas.Visibility = Visibility.Hidden;
            TradeCanvas.Visibility = Visibility.Hidden;

            switch ((string)TypesBox.SelectedItem)
            {
                case "Торговля":
                    TradeCanvas.Visibility = Visibility.Visible;
                    _type = DipMsg.Types.Trade;
                    acceptButton.IsEnabled = true;
                    break;
                case "Мирный договор":
                    _type = DipMsg.Types.Peace;
                    acceptButton.IsEnabled = true;
                    break;
                case "Военный альянс":
                    if (!_isReceive && CountryData.Alliance != CountryData.Name && !CountryData.IsBoss)
                    {
                        acceptButton.IsEnabled = false;
                        break;
                    }

                    acceptButton.IsEnabled = true;
                    AllianceCanvas.Visibility = Visibility.Visible;
                    _type = DipMsg.Types.Alliance;
                    break;
                case "Валютный союз":
                   /* if (!_isReceive && CountryData.CurrencyAlliance != CountryData.Name)
                    {
                        acceptButton.IsEnabled = false;
                        break;
                    }
                    _type = DipMsg.Types.CurrencyAlliance;
                    acceptButton.IsEnabled = true;
                    */
                    break;
                case "Военные учения":
                    _type = DipMsg.Types.MilitaryTraining;
                    acceptButton.IsEnabled = true;
                    break;
                case "Денежный перевод":
                    transferCanvas.Visibility = Visibility.Visible;
                    _type = DipMsg.Types.Transfer;
                    acceptButton.IsEnabled = true;
                    break;
                case "Другое":
                    otherCanvas.Visibility = Visibility.Visible;
                    _type = DipMsg.Types.Other;
                    acceptButton.IsEnabled = true;
                    break;
            }
        }

        private void TradeResBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((string)TradeResBox.SelectedItem)
            {
                case "Нефть":
                    TradeCountUpdown.Maximum = (int)CountryData.FinalOil;
                    break;
                case "Сталь":
                    TradeCountUpdown.Maximum = (int)CountryData.FinalSteel;
                    break;
                case "Древесина":
                    TradeCountUpdown.Maximum = (int)CountryData.FinalWood;
                    break;
                case "Сельхоз продукция":
                    TradeCountUpdown.Maximum = (int)CountryData.FinalAgricultural;
                    break;
            }
        }
    }
}
