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
using Totality.CommonClasses;
using Totality.Model;
using Totality.Model.Diplomatical;

namespace Totality.Client.ClientComponents.Dialogs.Foreign
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class ContractDialog : AbstractDialog, Dialog
    {
        public enum Orders { BreakContract };
        public delegate void ReceiveOrder(object sender, Order order);
        ReceiveOrder _receiveOrder;
        DipContract _contract;


        public ContractDialog(ReceiveOrder receiveOrder, DipContract contract)
        {
            _receiveOrder = receiveOrder;
            _contract = contract;
            InitializeComponent();

            string targetName;
            if (_contract.From != CountryData.Name)
            {
                targetName = _contract.From;
            }
            else
            {
                targetName = _contract.To;
            }

            switch (contract.Type)
            {
                case DipMsg.Types.Alliance:
                    textBlock.Text = "Союзный договор" + System.Environment.NewLine;
                    textBlock.Text += "Со страной " + targetName + System.Environment.NewLine;
                    textBlock.Text += "Наши братские страны всю историю прошли плечом к плечу. Данный договор закрепляет наши союзные обязательства." + System.Environment.NewLine;
                    textBlock.Text += "2016";
                    break;
                case DipMsg.Types.CurrencyAlliance:
                    textBlock.Text = "Договор о валютном альянсе" + System.Environment.NewLine;
                    textBlock.Text += "Со страной " + targetName + System.Environment.NewLine;
                    textBlock.Text += "Залог экономического успеха - стабильная валюта. Данный договор обязует нас совместно обеспечивать стабильность валюты союза." + System.Environment.NewLine;
                    textBlock.Text += "2016";
                    break;
                case DipMsg.Types.Other:
                    textBlock.Text = "Договор" + System.Environment.NewLine;
                    textBlock.Text += "Со страной " + targetName + System.Environment.NewLine;
                    textBlock.Text += _contract.Text + System.Environment.NewLine;
                    textBlock.Text += "2016";
                    break;
                case DipMsg.Types.Trade:
                    textBlock.Text = "Торговый договор" + System.Environment.NewLine;
                    textBlock.Text += "Со страной " + targetName + System.Environment.NewLine;
                    textBlock.Text += "О продаже страной " + _contract.From + " стране " + _contract.To  + System.Environment.NewLine;
                    switch ( _contract.Res)
                    {
                        case "Oil":
                            textBlock.Text += "Нефти " + System.Environment.NewLine;
                            break;
                        case "Steel":
                            textBlock.Text += "Стали " + System.Environment.NewLine;
                            break;
                        case "Agricultural":
                            textBlock.Text += "Сельхоз продукции " + System.Environment.NewLine;
                            break;
                        case "Wood":
                            textBlock.Text += "Древесины " + System.Environment.NewLine;
                            break;
                    }
                    textBlock.Text += "В объеме "+ _contract.Count + System.Environment.NewLine;
                    textBlock.Text += "По суммарной стоимости " + String.Format("{0:0,0}", _contract.Price) + System.Environment.NewLine;
                    textBlock.Text += "В течение " + _contract.Time + " ходов." + System.Environment.NewLine;
                    textBlock.Text += "2016";
                    break;
            }

        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            string targetName;
            if (_contract.From != CountryData.Name)
            {
                targetName = _contract.From;
            }
            else
            {
                targetName = _contract.To;
            }

            var order = new Order(CountryData.Name, targetName)
            {
                OrderNum = (short)Orders.BreakContract,
                TargetId = _contract.Id
            };

            _receiveOrder(this, order);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null);
        }
    }
}
