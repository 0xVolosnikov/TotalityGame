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
using Totality.Client.ClientComponents.Dialogs.Media;
using Totality.CommonClasses;

namespace Totality.Client.ClientComponents.Panels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class MediaPanel : AbstractPanel, InPanel
    {
        AbstractDialog currentDialog;
        List<News> _news = new List<News>();

        public MediaPanel()
        {
            InitializeComponent();
            PropagandaButton.click += () => createDialog(new PropagandaDialog(receiveOrder));
            NewsButton.click += () => createBigDialog(new NewsDialog(receiveOrder, _news));
        }

        public void ReceiveNews (News[] news)
        {
            _news.Clear();
            _news.AddRange(news);
        }

        public void OpenNews()
        {
            if (canvas1.Children.Contains(currentDialog))
            canvas1.Children.Remove(currentDialog);
            currentDialog = null;

            createBigDialog(new NewsDialog(receiveOrder, _news));
        }

        private void createDialog(AbstractDialog dialog) 
        {
            if (currentDialog == null)
            {
                currentDialog = dialog;
                canvas1.Children.Add(currentDialog);
                Canvas.SetLeft(currentDialog, 295);
                Canvas.SetTop(currentDialog, 68);
            }
        }

        private void createBigDialog(AbstractDialog dialog)
        {
            if (currentDialog == null)
            {
                currentDialog = dialog;
                canvas1.Children.Add(currentDialog);
                Canvas.SetLeft(currentDialog, (Width - ((UserControl)currentDialog).Width) / 2.0);
                Canvas.SetTop(currentDialog, (Height - ((UserControl)currentDialog).Height) / 2.0);
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
            if (CountryData.MinsBlocks[(short)Mins.Media] > 0 && !isBlocked)
            {
                isBlocked = true;
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Media/PropButtonDeactivated.png", UriKind.Relative);
                NewsButton.imgUp = new BitmapImage(uriSource);
                NewsButton.Update();
                NewsButton.IsEnabled = false;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Media/NewsButtonDeactivated.png", UriKind.Relative);
                PropagandaButton.imgUp = new BitmapImage(uriSource);
                PropagandaButton.Update();
                PropagandaButton.IsEnabled = false;
            }
            else if (isBlocked && CountryData.MinsBlocks[(short)Mins.Media] == 0)
            {
                isBlocked = false;
                var uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Media/PropButton.png", UriKind.Relative);
                NewsButton.imgUp = new BitmapImage(uriSource);
                NewsButton.Update();
                NewsButton.IsEnabled = true;

                uriSource = new Uri(@"/Totality.Client.ClientComponents;component/Images/Media/NewsButton.png", UriKind.Relative);
                PropagandaButton.imgUp = new BitmapImage(uriSource);
                PropagandaButton.Update();
                PropagandaButton.IsEnabled = true;
            }
        }
    }
}
