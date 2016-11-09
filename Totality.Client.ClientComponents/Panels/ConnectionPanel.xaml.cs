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
    public partial class ConnectionPanel : UserControl
    {
        DoubleAnimation SyncronizationSpinning;
        DoubleAnimation slide;
        DoubleAnimation clear;

        public ConnectionPanel()
        {
            InitializeComponent();
        }

        public void StartSpinning()
        {
            SyncronizationSpinning = new DoubleAnimation(360, TimeSpan.FromSeconds(1.5));
            SyncronizationSpinning.RepeatBehavior = RepeatBehavior.Forever;
            _synchronizationImage.RenderTransform = new RotateTransform(0, _synchronizationImage.Width/2, _synchronizationImage.Height / 2);
            _synchronizationImage.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, SyncronizationSpinning);
        }

        public void Close()
        {
            SyncronizationSpinning.RepeatBehavior = new RepeatBehavior(0);
            _synchronizationImage.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, SyncronizationSpinning);
            slide = new DoubleAnimation(1000, TimeSpan.FromSeconds(0.5));
            slide.AccelerationRatio = 0.4;
            _canvas.BeginAnimation(Canvas.TopProperty, slide);
            clear = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
            clear.Completed += Clear_Completed;
            _grid.Background.BeginAnimation(Brush.OpacityProperty, clear);
        }

        private void Clear_Completed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        public void Open()
        {
            this.Visibility = Visibility.Visible;
            StartSpinning();
            slide = new DoubleAnimation(28, TimeSpan.FromSeconds(0.5));
            slide.AccelerationRatio = 0.4;
            _canvas.BeginAnimation(Canvas.TopProperty, slide);
            clear = new DoubleAnimation(100, TimeSpan.FromSeconds(0.5));
            _grid.Background.BeginAnimation(Brush.OpacityProperty, clear);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
