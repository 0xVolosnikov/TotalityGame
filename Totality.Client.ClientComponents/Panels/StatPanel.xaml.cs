using Totality.Client.ClientComponents.Dialogs;
using Totality.Client.ClientComponents.Dialogs.Military;
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
using Totality.Model;
using Totality.Client.ClientComponents.Dialogs.Inner;
using Totality.CommonClasses;
using System.Collections.ObjectModel;

namespace Totality.Client.ClientComponents.Panels
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class StatPanel : AbstractPanel, InPanel
    {
        Dialog currentDialog;
        ObservableCollection<Record> leftList = new ObservableCollection<Record>();
        ObservableCollection<Record> rightList = new ObservableCollection<Record>();
        public class Record
        {
            public string Text1 { get; set; }
            public string Text2 { get; set; }

            public Record(string text1, string text2)
            {
                Text1 = text1;
                Text2 = text2;
            }
        }

        public StatPanel()
        {
            InitializeComponent();
            leftGrid.ItemsSource = leftList;
            rightGrid.ItemsSource = rightList;
        }

        private void createDialog<T>(Dialog dialog) where T : UIElement
        {
            if (currentDialog == null)
            {
                currentDialog = dialog;
                canvas1.Children.Add((T)currentDialog);
                Canvas.SetLeft((T)currentDialog, 295);
                Canvas.SetTop((T)currentDialog, 68);
            }
        }

        public void Update()
        {
            leftList.Clear();
            leftList.Add(new Record("Уровень силы власти", CountryData.PremierLvl.ToString()));
            leftList.Add(new Record("Уровень слежки", CountryData.ShadowingLvl.ToString()));
            leftList.Add(new Record("Уровень контрразведки", CountryData.CounterSpyLvl.ToString()));
            leftList.Add(new Record("Уровень разведки", CountryData.IntelligenceLvl.ToString()));
            leftList.Add(new Record("Уровень МВД", CountryData.InnerLvl.ToString()));

            rightList.Clear();
            rightList.Add(new Record("Стоимость повышения легкой промышленности", CountryData.LightScLvlUpCost.ToString()));
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }
    }
}
