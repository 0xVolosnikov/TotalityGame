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
    /// Логика взаимодействия для MinisteryButton.xaml
    /// </summary>
    public partial class Button : UserControl
    {
        public ImageSource imgUp { get; set; }
        public ImageSource imgDown { get; set; }
        public ImageSource toolTipPicture { get; set; }
        public string toolTipDescription { get; set; }
        public string toolTipCost { get; set; }
        public delegate void Click();
        public event Click click;

        private bool isDown = false;

        public Button()
        {
            this.Loaded += Button_Loaded;
            InitializeComponent();
            toolTipDescription = "";
            toolTipCost = "";
            this.image.MouseDown += Image_MouseDown;
            this.image.MouseUp += Image_MouseUp;
            this.image.MouseLeave += Image_MouseLeave;

        }


        private void Button_Loaded(object sender, RoutedEventArgs e)
        {
            if (imgUp != null)
            this.image.Source = imgUp;

            if (toolTipPicture != null)
            {
                ButtonToolTip tip = new ButtonToolTip();
                tip.image.Source = toolTipPicture;
                tip.description.Text= toolTipDescription;
                tip.cost.Text = toolTipCost;
                if (toolTipCost == "") tip.Height -= 50;
                this.ToolTip = new ToolTip();
                ((ToolTip)this.ToolTip).Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                ((ToolTip)this.ToolTip).BorderThickness = new Thickness(0);
                ((ToolTip)this.ToolTip).Content = tip;
                ToolTipService.SetInitialShowDelay(this, 1000);
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (imgDown != null)
                this.image.Source = imgDown;
            isDown = true;            
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (imgUp != null)
                this.image.Source = imgUp;
            click?.Invoke();
            isDown = false;
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isDown)
            {
                isDown = false;
                if (imgUp != null)
                    this.image.Source = imgUp;
            }
        }
    }
}
