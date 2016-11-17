using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Totality.Model;

namespace Totality.Client.ClientComponents.Dialogs
{
    public class AbstractDialog : UserControl
    {
        public static Country CountryData;
        public static List<string> Countries = new List<string>();
        public static List<string> Ministers = new List<string>();
    }
}
