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

        public void RiotOn()
        {
            _canvas.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC00000"));
        }

        public void RiotOff()
        {
            _canvas.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF1A203D"));
        }

        public void UpdateMoney (long money)
        {
            this.LMoney.Content = String.Format("{0:0,0}", money);
        }

        public void UpdateProfit(long FinalLightIndustry, double TaxesLvl, double InflationCoeff)
        {
            if (InflationCoeff > 1)
                profitLabel.Content = "+" + ((long)(FinalLightIndustry * Constants.LightPowerProfit * (TaxesLvl / 15.0)) * Math.Sqrt(InflationCoeff)).ToString("N0");
            else
                profitLabel.Content = "+" + ((long)(FinalLightIndustry * Constants.LightPowerProfit * (TaxesLvl / 15.0)) * Math.Pow(InflationCoeff, 1.2)).ToString("N0");

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
