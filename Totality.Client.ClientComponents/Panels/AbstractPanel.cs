using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Totality.Model;

namespace Totality.Client.ClientComponents.Panels
{
    public class AbstractPanel : UserControl
    {
        public static Country CountryData { get; set; }
        public static OrdersTable Table;
    }
}
