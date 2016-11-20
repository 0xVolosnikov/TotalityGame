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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Totality.CommonClasses;
using Totality.Model;

namespace Totality.Client.ClientComponents
{
    /// <summary>
    /// Логика взаимодействия для NukeRocketOrder.xaml
    /// </summary>
    public partial class NukeRocketOrder : UserControl
    {
        public Guid Id { get; }
        public Country CountryData { get; set; }

        public NukeRocketOrder()
        {
            InitializeComponent();
        }

        public NukeRocketOrder(NukeRocket rocket, Country countryData)
        {
            InitializeComponent();

            CountryData = countryData;
            Id = rocket.Id;
            CountLabel.Content = rocket.Count;
            AgressorLabel.Text = rocket.From;
            TargetLabel.Text = rocket.To;

            ProgressLine.Width = 315 * (rocket.LifeTime / (double)Constants.NukeRocketLifetime);
            
        }

        public void Update(NukeRocket rocket)
        {
            CountLabel.Content = rocket.Count;
            AgressorLabel.Text = rocket.From;
            TargetLabel.Text = rocket.To;
            if (rocket.LifeTime == 0)
            {
                if (rocket.To.Equals(CountryData.Name))
                {
                    Background = Brushes.Red;
                    TargetLabel.Foreground = Brushes.Orange;
                }
                else
                    Background = Brushes.YellowGreen;
            }
            if (rocket.Count == 0)
            {
                Background = Brushes.Green;
            }
            ProgressLine.Width = 315*( rocket.LifeTime / (double)Constants.NukeRocketLifetime);
        }
    }
}
