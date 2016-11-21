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
    /// Логика взаимодействия для StatButton.xaml
    /// </summary>
    public partial class StatButton : UserControl
    {
        public delegate void Click();
        public event Click click;

        public StatButton()
        {
            InitializeComponent();
            this.label.MouseDown += Label_MouseDown;
            this.label.MouseUp += Label_MouseUp;
        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OnMouseDown(e);
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            this.canvas.Background = new SolidColorBrush(Color.FromRgb(114, 67, 13));

        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            click?.Invoke();
            this.canvas.Background = new SolidColorBrush(Color.FromRgb(0, 4, 25));
        }
    }
}
