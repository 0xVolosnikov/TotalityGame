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
        DoubleAnimation open = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(2));
        DoubleAnimation clear = new DoubleAnimation(100, 0, TimeSpan.FromSeconds(1));
        public delegate void NameHandler(string name);
        public event NameHandler NameReceived;
        private Uri _video;

        public ConnectionPanel()
        {
            InitializeComponent();
        }

        public void StartSpinning()
        {
           
            DoubleAnimationUsingKeyFrames g = new DoubleAnimationUsingKeyFrames();
            SyncronizationSpinning = new DoubleAnimation(360, TimeSpan.FromSeconds(1.5));
            SyncronizationSpinning.RepeatBehavior = RepeatBehavior.Forever;
            _synchronizationImage.RenderTransform = new RotateTransform(0, _synchronizationImage.Width/2, _synchronizationImage.Height / 2);
            _synchronizationImage.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, SyncronizationSpinning);
        }

        public void Close()
        {
            

          //  slide = new DoubleAnimation(1000, TimeSpan.FromSeconds(0.5));
          //  slide.AccelerationRatio = 0.4;
          //  _canvas.BeginAnimation(Canvas.TopProperty, slide);
            var clear = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
            //clear.AccelerationRatio = 0.4;
            //_canvas.Background.BeginAnimation(Brush.OpacityProperty, clear);
            clear.Completed += Clear_Completed;
            BeginAnimation(UIElement.OpacityProperty, clear);


        }

        public void Video (Uri video)
        {
            _mediaElement.Source = video;
            _mediaElement.BeginInit();
            _mediaElement.Play();
        }

        private void Clear_Completed(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            SyncronizationSpinning.RepeatBehavior = new RepeatBehavior(0);
            _synchronizationImage.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, SyncronizationSpinning);
            _mediaElement.Pause();
        }

        public void Open()
        {
            StartSpinning();
            _mediaElement.Play();
            this.Visibility = Visibility.Visible;
            //slide = new DoubleAnimation(28, TimeSpan.FromSeconds(0.5));
            //slide.AccelerationRatio = 0.4;
           // _canvas.BeginAnimation(Canvas.TopProperty, slide);
            var open = new DoubleAnimation(0, 100, TimeSpan.FromSeconds(2));
            open.AccelerationRatio = 0.4;
            //_grid.Background.BeginAnimation(Brush.OpacityProperty, clear);
            this.BeginAnimation(UIElement.OpacityProperty, open);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void approveName_Click(object sender, RoutedEventArgs e)
        {
            StartSpinning();
            NameReceived.Invoke(nameInput.Text);
            _nameCanvas.Visibility = Visibility.Hidden;
        }
    }
}
