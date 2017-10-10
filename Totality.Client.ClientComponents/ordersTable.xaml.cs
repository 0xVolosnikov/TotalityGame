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
using Totality.Model;

namespace Totality.Client.ClientComponents
{
    /// <summary>
    /// Логика взаимодействия для ordersTable.xaml
    /// </summary>
    public partial class OrdersTable : UserControl, InPanel
    {
        private ObservableCollection<OrderRecord> orderRecords = new ObservableCollection<OrderRecord>();
        public int startingLines { get; set; }
        private long _money = 1000000;

        public OrdersTable()
        {
            InitializeComponent();
            this.Loaded += OrdersTable_Loaded;
            this.dataGrid.MouseDoubleClick += DataGrid_MouseDoubleClick;
            MenuItem deleteAllButton = new MenuItem();
            deleteAllButton.Header = "Очистить все";
            deleteAllButton.Icon = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri(@"/Totality.Client.ClientComponents;component/Images/remove.png", UriKind.Relative))    
            };
            deleteAllButton.Click += DeleteAllButton_Click;
            this.ContextMenu.Items.Add(deleteAllButton);
            
        }

        private void OrdersTable_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < startingLines; i++) orderRecords.Add(new Totality.Client.ClientComponents.OrderRecord("", ""));
            this.dataGrid.ItemsSource = orderRecords;
            dataGrid.Columns[0].Width = this.Width * 0.8;
           dataGrid.Columns[1].Width = this.Width * 0.2 + 1;
        }

        public void changeMoney(long money)
        {
            _money = money;
            long cost = 0;
            foreach (var ord in orderRecords)
            {
                if (!ord.Text.Equals(""))
                    cost += long.Parse(ord.Cost);
            }

            sum.Content = String.Format("{0:N0}", cost).Replace(' ', '.') + " / " + String.Format("{0:N0}", money).Replace(' ', '.');
            if (cost > _money) sum.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFA80000"));
            else sum.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF00A83E"));
        }

        public void addOrder(OrderRecord newRecord)
        {
            orderRecords.Add(newRecord);
            if (newRecord.Text != "")
            {
                reorginizeList();
                if (orderRecords.Count > startingLines)
                {
                    this.dataGrid.Height += this.dataGrid.RowHeight ;
                    this.Height = this.dataGrid.Height + this.image.Height;
                }
            }            
        }

        public List<Order> GetOrders()
        {
            List<Order> orders = new List<Order>();

            for (int i = 0; i < orderRecords.Count; i++)
            {
                if (orderRecords[i].Order == null) break;

                orders.Add(orderRecords[i].Order);
            }

            return orders;
        }

        public void Clear()
        {
            orderRecords.Clear();
            for (int i = 0; i < startingLines; i++) orderRecords.Add(new Totality.Client.ClientComponents.OrderRecord("", ""));
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            orderRecords.Remove(dataGrid.CurrentItem as OrderRecord);
            if (orderRecords.Count < startingLines)
            {
                orderRecords.Add(new OrderRecord("", ""));
            }
                reorginizeList();          
        }

        private void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            orderRecords.Clear();
            for (int i = 0; i < startingLines; i++) orderRecords.Add(new Totality.Client.ClientComponents.OrderRecord("", ""));
        }

        private void reorginizeList()
        {
            for (int i = orderRecords.Count - 1; i > 0; i--)
            {
                if( !orderRecords[i].Text.Equals("") )
                {
                    int k = i;
                    while (k > 0 && orderRecords[k-1].Text.Equals("")){ k--; }
                    if (i != k)
                    {
                        orderRecords.RemoveAt(k);
                        if (i - 1 != k)
                            orderRecords.Move(i - 1, k);
                    }
                    break;

                }
            }

            changeMoney(_money);
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
