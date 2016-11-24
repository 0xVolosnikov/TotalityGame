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

namespace Totality.Client.ClientComponents.Dialogs.Media
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeDialog.xaml
    /// </summary>
    public partial class NewsDialog :  AbstractDialog, Dialog
    {
        public delegate void ReceiveOrder(object sender, Order order, string text, long price);
        ReceiveOrder _receiveOrder;
        List<News> _news;
        List<TextBlock> leftList = new List<TextBlock>();
        List<TextBlock> rightList = new List<TextBlock>();

        public NewsDialog(ReceiveOrder receiveOrder, List<News> news)
        {
            _news = news;
            _receiveOrder = receiveOrder;
            InitializeComponent();

            stepLabel.Content = CurrentStep;

            foreach (News n in news.Where(x => x.IsOur))
            {
                var record = new System.Windows.Controls.TextBlock();
                record.Width = 486;
                record.FontSize = 21;
                var c = new FontFamilyConverter();
                record.FontFamily = (FontFamily)c.ConvertFrom("Aparajita");
                record.Text = n.text;
                record.TextWrapping = TextWrapping.Wrap;
                leftList.Add(record);
                Panel1.Children.Add(leftList.Last());
            }

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            _receiveOrder(this, null, null, 0);
        }
    }
}
