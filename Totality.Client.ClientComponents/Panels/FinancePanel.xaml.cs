﻿using Totality.Client.ClientComponents.Dialogs;
using Totality.Client.ClientComponents.Dialogs.Finance;
using Totality.Client.ClientComponents.Dialogs.Common;
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
using Totality.CommonClasses;
using Totality.Client.ClientComponents.Panels;

namespace Totality.Client.ClientComponents
{
    /// <summary>
    /// Логика взаимодействия для MilitaryPanel.xaml
    /// </summary>
    public partial class FinancePanel : UserControl, InPanel
    {
        private Dialog _currentDialog;

        public FinancePanel()
        {
            InitializeComponent();
            /*
            this.InvestButton.SnapsToDevicePixels = true;
            this.LVLupButton.click += () => createDialog<LvlUpDialog>(new LvlUpDialog(receiveOrder, Mins.Finance));
            this.InvestButton.click += () => createDialog<InvestDialog>(new InvestDialog(receiveOrder, Mins.Finance));
            this.ExchangeButton.click += () => createDialog<ExchangeDialog>(new ExchangeDialog(receiveOrder, Mins.Finance));
            this.TransButton.click += () => createDialog<TransDialog>(new TransDialog(receiveOrder, Mins.Finance));*/
        }

        private void createDialog<T>(Dialog dialog) where T : UIElement
        {
            if (_currentDialog == null)
            {
                _currentDialog = dialog;
                canvas1.Children.Add((T)_currentDialog);
                Canvas.SetLeft((T)_currentDialog, 295);
                Canvas.SetTop((T)_currentDialog, 68);
            }
        }

        public void receiveOrder(object sender, Order order)
        {
            canvas1.Children.Remove((UIElement)sender);
            _currentDialog = null;     
        }
    }
}
