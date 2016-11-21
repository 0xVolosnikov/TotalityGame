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

namespace Totality.Client.ClientComponents.Panels
{
    /// <summary>
    /// Логика взаимодействия для ConnectionPanel.xaml
    /// </summary>
    public partial class WaitingPanel : UserControl
    {
        DoubleAnimation SyncronizationSpinning = new DoubleAnimation(360, TimeSpan.FromSeconds(2.3));
        DoubleAnimation open = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(2));
        ColorConverter conv = new ColorConverter();

        public WaitingPanel()
        {
            InitializeComponent();

        }

        public void StartSpinning()
        {

            SyncronizationSpinning = new DoubleAnimation(360, TimeSpan.FromSeconds(2.3));
            SyncronizationSpinning.RepeatBehavior = RepeatBehavior.Forever;
            TimeImage.RenderTransform = new RotateTransform(0, TimeImage.Width / 2, TimeImage.Height / 2);
            TimeImage.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, SyncronizationSpinning);
        }

        public void Close()
        {
            var clear = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.4));
            clear.Completed += Clear_Completed;
            _canvas.BeginAnimation(Canvas.OpacityProperty, clear);

            var slide = new DoubleAnimation(1000, TimeSpan.FromSeconds(0.5));
            slide.AccelerationRatio = 0.4;
            _border.BeginAnimation(Canvas.TopProperty, slide);

        }

        private void Clear_Completed(object sender, EventArgs e)
        {
            SyncronizationSpinning.RepeatBehavior = new RepeatBehavior(0);
            TimeImage.RenderTransform = new RotateTransform(0, TimeImage.Width / 2, TimeImage.Height / 2);
            TimeImage.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, SyncronizationSpinning);
            this.Visibility = Visibility.Hidden;
        }

        public void Open()
        {
            StartSpinning();
            this.Visibility = Visibility.Visible;
            var slide = new DoubleAnimation(28, TimeSpan.FromSeconds(0.5));
            slide.AccelerationRatio = 0.4;
            _border.BeginAnimation(Canvas.TopProperty, slide);
            var open = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.4));
            open.AccelerationRatio = 0.4;
            _canvas.BeginAnimation(Canvas.OpacityProperty, open);
        }
    }
}
