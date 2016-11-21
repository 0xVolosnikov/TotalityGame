using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Totality.Model;

namespace Totality.Client.ClientComponents.Panels
{
    public class AbstractPanel : UserControl
    {
        public static Country CountryData { get; set; }
        public static OrdersTable Table;
        protected bool isBlocked;

        protected void activateButton(Button button, string path)
        {
            var uriSource = new Uri(@path, UriKind.Relative);
            button.imgUp = new BitmapImage(uriSource);
            button.Update();
            button.IsEnabled = true;
        }

        protected void deActivateButton(Button button, string path)
        {
            var uriSource = new Uri(@path, UriKind.Relative);
            button.imgUp = new BitmapImage(uriSource);
            button.Update();
            button.IsEnabled = false;
        }
    }
}
