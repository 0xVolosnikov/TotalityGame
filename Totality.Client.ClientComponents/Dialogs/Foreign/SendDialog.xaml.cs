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
        public delegate void SendDipMessage(object sender, DipMsg msg);
        SendDipMessage _sendMessage;

        public SendDialog(SendDipMessage sendMessage)
        {
            _sendMessage = sendMessage;
            InitializeComponent();

            List<string> types = new List<string>();
            types.Add("Торговля");
            types.Add("Мирный договор");
            types.Add("Военный альянс");
            types.Add("Валютный союз");
            types.Add("Военные учения");
            types.Add("Другое");

            CountriesBox.ItemsSource = Countries;
            CountriesBox.SelectedIndex = 0;
            TypesBox.ItemsSource = types;
            TypesBox.SelectedIndex = 0;


        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            _sendMessage(this, new DipMsg(CountryData.Name, (string)CountriesBox.SelectedItem)); // {Type}
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _sendMessage(this, null);
        }
    }
}
