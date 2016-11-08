﻿using System;
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

namespace Totality.Client.ClientComponents.Dialogs.Military
{
    /// <summary>
    /// Логика взаимодействия для NukeStrikeCountDialog.xaml
    /// </summary>
    public partial class PROcountDialog : UserControl, Dialog
    {
        public delegate void ReceiveOrder(Object sender, Order order);
        ReceiveOrder receiveOrder;

        public PROcountDialog(ReceiveOrder receiveOrder)
        {
            this.receiveOrder = receiveOrder;
            InitializeComponent();
        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
