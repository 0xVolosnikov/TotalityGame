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

namespace Totality.Client.ClientComponents
{
    /// <summary>
    /// Логика взаимодействия для NukeAttackDialog.xaml
    /// </summary>
    public partial class NukeAttackDialog : UserControl
    {
        DoubleAnimation Spinning;

        public NukeAttackDialog()
        {
            InitializeComponent();
            List<NukeRocketOrder> a = new List<NukeRocketOrder>();
            a.Add(new NukeRocketOrder());
            a.Add(new NukeRocketOrder());
            a.Add(new NukeRocketOrder());
            a.Add(new NukeRocketOrder());
            RocketsView.ItemsSource = a;

            Spinning = new DoubleAnimation(360, TimeSpan.FromSeconds(1.5));
            Spinning.RepeatBehavior = RepeatBehavior.Forever;
            RadioSign.RenderTransform = new RotateTransform(0, RadioSign.Width / 2, RadioSign.Height / 2);
            RadioSign.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, Spinning);
        }
    }
}
