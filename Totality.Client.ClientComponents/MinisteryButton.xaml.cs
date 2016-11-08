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
    public partial class MinisteryButton : UserControl
    {
        bool isDown;
        public ImageSource imgUp { get; set; }
        public ImageSource imgDown { get; set; }
        public delegate void Click();
        public event Click click;

        public MinisteryButton()
        {
            this.Loaded += MinisteryButton_Loaded;
            InitializeComponent();
            this.image.MouseDown += Image_MouseDown;
            
        }

        private void MinisteryButton_Loaded(object sender, RoutedEventArgs e)
        {
            if (imgUp != null)
            this.image.Source = imgUp;
        }

        public void connectToAnotherButton(MinisteryButton anotherButton)
        {
            anotherButton.MouseDown += release;
        }

        public void connectToButtons(List<MinisteryButton> buttons)
        {
            foreach(MinisteryButton curButton in buttons)
            {
                if (curButton.Equals(this)) continue;

                this.connectToAnotherButton(curButton);
            }
        }

        public void release(object sender, EventArgs e)
        {
            if (isDown)
            {
                isDown = false;
                if (imgUp != null)
                    this.image.Source = imgUp;
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
         if (!isDown)
            {
                if (imgDown != null)
                    this.image.Source = imgDown;
                isDown = true;
                click?.Invoke();
            }   
        }
    }
}
