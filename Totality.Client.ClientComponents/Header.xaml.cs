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

namespace Totality.Client.ClientComponents
{
    /// <summary>
    /// Логика взаимодействия для Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        public delegate void Clicked();
        public event Clicked StatButtonClicked;

        public Header()
        {
            InitializeComponent();
            statButton.click += () => StatButtonClicked?.Invoke();
        }


        public void UpdateMoney (long money)
        {
            this.LMoney.Content = money.ToString();
        }

        public void UpdateNukes(int count)
        {
            this.LNukes.Content = count.ToString();
        }

        public void UpdateMissiles(int count)
        {
            this.LPRO.Content = count.ToString();
        }

        public void UpdateMood(short value)
        {
            this.LMood.Content = value.ToString();
        }

    }
}
