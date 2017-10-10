using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Totality.Client.ClientComponents.ServiceReference1;
using Totality.CommonClasses;
using Totality.Model;
using Totality.Model.Diplomatical;
using static Totality.Model.Country;

namespace Totality.Client.ClientComponents.Dialogs.Security
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class NetsDialog : AbstractDialog, Dialog
    {
        private enum Orders { ImproveNetwork, AddAgents, OrderToAgent, Purge, CounterSpyLvlUp, ShadowingUp, IntelligenceUp, Sabotage }
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        ReceiveOrder _receiveOrder;
        Country _ourCountry;
        List<string> _countries;
        TransmitterServiceClient _client;
        List<Totality.Client.ClientComponents.Button> buttons = new List<Button>();
        private Dictionary<string, DipContract[]> _secretBase;

        public NetsDialog(ReceiveOrder receiveOrder, TransmitterServiceClient client)
        {
            _receiveOrder = receiveOrder;
            _client = client;

            InitializeComponent();

            levelButton.click += LevelButton_click;

            buttons.Add(b1);
            buttons.Add(b2);
            buttons.Add(b3);
            buttons.Add(b4);
            buttons.Add(b5);
            buttons.Add(b6);
            buttons.Add(b7);
            buttons.Add(b8);
            buttons.Add(b9);

            CountriesBox.ItemsSource = Countries;
            if (Countries.Count > 0)
                CountriesBox.SelectedIndex = 0;
        }

        private void LevelButton_click()
        {
            var order = new Order(CountryData.Name, (string)CountriesBox.SelectedValue);
            order.Ministery = (short)Mins.Security;
            order.OrderNum = (short)Orders.ImproveNetwork;
            _receiveOrder(this, order, "Повышение уровня разведсети: " + (string)CountriesBox.SelectedValue, 0);
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null, null, 0);
        }

        private void CountriesBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CountryData.SpyNetworks.ContainsKey((string)CountriesBox.SelectedValue) && CountryData.SpyNetworks[(string)CountriesBox.SelectedValue].NetLvl >= 3)
            {
                levelLabel.Text = CountryData.SpyNetworks[(string)CountriesBox.SelectedValue].NetLvl.ToString();
                //_wrap.IsEnabled = true;

                for (int i = 0; i < buttons.Count; i++)
                {
                    if (CountryData.SpyNetworks[(string)CountriesBox.SelectedValue].Recruit[i])
                    {
                        var buf = buttons[i].imgUp;
                        buttons[i].imgUp = buttons[i].imgDown;
                        buttons[i].imgDown = buf;

                        var min = i;
                        buttons[i].click += () =>
                        {
                            _ourCountry = CountryData;
                            SecretPanels.SecretAbstractPanel.CountryData = _client.GetCountryData((string)CountriesBox.SelectedValue);
                            SecretPanels.SecretAbstractPanel panel = null;

                            switch (min)
                            {
                                case 0:
                                    CountryData = SecretPanels.SecretAbstractPanel.CountryData;
                                    panel = new SecretPanels.SecretIndustryPanel(getOrder);
                                    ((SecretPanels.SecretIndustryPanel)panel).Update();
                                    break;
                                case 1:
                                    _countries = Countries.ToList();
                                    Countries.Add(CountryData.Name);
                                    Countries.Remove((string)CountriesBox.SelectedValue);
                                    CountryData = SecretPanels.SecretAbstractPanel.CountryData;
                                    panel = new SecretPanels.SecretFinancePanel(getOrder);
                                    ((SecretPanels.SecretFinancePanel)panel).Update();
                                    break;
                                case 2:
                                    CountryData = SecretPanels.SecretAbstractPanel.CountryData;
                                    panel = new SecretPanels.SecretMilitaryPanel(getOrder);
                                    ((SecretPanels.SecretMilitaryPanel)panel).Update();
                                    break;
                                case 3:
                                    CountryData = SecretPanels.SecretAbstractPanel.CountryData;
                                    panel = new SecretPanels.SecretForeignPanel(getOrder, _client.AskContracts(_ourCountry.Name, CountryData.Name));
                                    ((SecretPanels.SecretForeignPanel)panel).Update();
                                    break;
                                case 4:
                                    CountryData = SecretPanels.SecretAbstractPanel.CountryData;
                                    panel = new SecretPanels.SecretMediaPanel(getOrder);
                                    ((SecretPanels.SecretMediaPanel)panel).Update();
                                    break;
                                case 5:
                                    CountryData = SecretPanels.SecretAbstractPanel.CountryData;
                                    panel = new SecretPanels.SecretInnerPanel(getOrder);
                                    ((SecretPanels.SecretInnerPanel)panel).Update();
                                    break;
                                case 6:
                                    CountryData = SecretPanels.SecretAbstractPanel.CountryData;
                                    panel = new SecretPanels.SecretSecurityPanel(getOrder);
                                    ((SecretPanels.SecretSecurityPanel)panel).Update();
                                    break;
                                case 7:
                                    CountryData = SecretPanels.SecretAbstractPanel.CountryData;
                                    panel = new SecretPanels.SecretSciencePanel(getOrder);
                                    ((SecretPanels.SecretSciencePanel)panel).Update();
                                    break;
                                case 8:
                                    CountryData = SecretPanels.SecretAbstractPanel.CountryData;
                                    panel = new SecretPanels.SecretPremierPanel(getOrder);
                                    ((SecretPanels.SecretPremierPanel)panel).Update();
                                    break;
                            }
                            canvas.Children.Add(panel);
                            Canvas.SetLeft(panel, (Width - panel.Width)/2);
                            Canvas.SetTop(panel, (Height - panel.Height)/2 - 35 );
                        };
                    }
                    else
                    {
                        var min = i;
                        buttons[i].click += () =>
                        {
                            var recDil = new RecruitDialog(getInnerOrder, min);
                            canvas.Children.Add(recDil);
                            Canvas.SetLeft(recDil, (Width - recDil.Width) / 2);
                            Canvas.SetTop(recDil, (Height - recDil.Height) / 2);
                        };
                    }
                }
            }
            else
            {
                if (CountryData.SpyNetworks.ContainsKey((string)CountriesBox.SelectedValue))
                    levelLabel.Text = CountryData.SpyNetworks[(string)CountriesBox.SelectedValue].NetLvl.ToString();
                else
                    levelLabel.Text = "0";
            }

            this.UpdateLayout();
        }


        private void getInnerOrder(object sender, Order order)
        {
            if (order != null)
            {
                order.TargetCountryName = (string) CountriesBox.SelectedValue;
                var agentCost = Constants.InitialAgentCost;

                int count = 0;
                foreach (KeyValuePair<string, SpyNetwork> net in CountryData.SpyNetworks)
                {
                    var a = Array.FindAll(net.Value.Recruit, x => x == true);
                    count += a.Length;
                }

                agentCost = (long) (agentCost*Math.Pow(Constants.AgentCostRatio, count));
                _receiveOrder(this, order,
                    "Внедрение агентов в страну " + (string) CountriesBox.SelectedValue + ", " +
                    Ministers[order.TargetMinistery], agentCost);
            }
            else
            {
                _receiveOrder(this, null, null, 0);
            }
        }


        private void getOrder(object sender, Order order)
        {
            if (_ourCountry != null)
            {
                AbstractDialog.CountryData = _ourCountry;
                if (_countries != null)
                    AbstractDialog.Countries = _countries;
            }


            canvas.Children.Remove((UIElement)sender);
            if (order != null)
            {
                {
                    var newOrder = new Order(CountryData.Name);
                    newOrder.TargetCountryName = order.CountryName;
                    newOrder.Ministery = (short)Mins.Secret;
                    newOrder.OrderNum = order.OrderNum;
                    newOrder.TargetCountryName2 = order.TargetCountryName;
                    newOrder.TargetMinistery = order.Ministery;
                    newOrder.Count = order.Count;
                    newOrder.Value = order.Value;
                    newOrder.Value2 = order.Value2;
                    _receiveOrder(this, newOrder, "Приказ агенту: " + (string)CountriesBox.SelectedValue + ", " + Ministers[order.Ministery], 0);
                }
            }


        }
    }
}
