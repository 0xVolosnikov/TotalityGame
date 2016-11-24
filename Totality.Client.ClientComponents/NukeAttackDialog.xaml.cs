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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Totality.Client.ClientComponents.Dialogs;
using Totality.Model;

namespace Totality.Client.ClientComponents
{
    /// <summary>
    /// Логика взаимодействия для NukeAttackDialog.xaml
    /// </summary>
    public partial class NukeAttackDialog : AbstractDialog
    {
        private DoubleAnimation Spinning;
        private ObservableCollection<NukeRocketOrder> _rockets = new ObservableCollection<NukeRocketOrder>();

        public delegate void ShootDown(Guid Id);
        public event ShootDown TryToShootDown;
        public int Count { get; set; }

        public NukeAttackDialog()
        {
            InitializeComponent();
            RocketBox.ItemsSource = _rockets;

            Spinning = new DoubleAnimation(360, TimeSpan.FromSeconds(1.5));
            Spinning.RepeatBehavior = RepeatBehavior.Forever;
            RadioSign.RenderTransform = new RotateTransform(0, RadioSign.Width / 2, RadioSign.Height / 2);
            RadioSign.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, Spinning);
        }

        public void UpdateRockets(NukeRocket[] rockets)
        {
            if (rockets.Count() == 0) Visibility = Visibility.Collapsed;

            for (int i = 0; i < rockets.Count(); i++)
            {
                if (_rockets.Any(x => x.Id == rockets[i].Id))
                {
                    //NukeRocketOrder currentOrder = _rockets.First(x => x.Id == rockets[i].Id);
                    _rockets.First(x => x.Id == rockets[i].Id).Update(rockets[i]);

                 //   if (_rockets.First(x => x.Id == rockets[i].Id).CountLabel.Content.Equals("0"))
                  //      _rockets.Remove(_rockets.First(x => x.Id == rockets[i].Id));
                }
                else
                    _rockets.Add(new NukeRocketOrder(rockets[i], CountryData));
            }        
        }

        private void shootDownButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void ShootDownButton_Click(object sender, RoutedEventArgs e)
        {
           TryToShootDown?.Invoke(_rockets[RocketBox.SelectedIndex].Id);
        }
    }
}
