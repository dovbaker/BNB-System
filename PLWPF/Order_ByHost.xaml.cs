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
using System.Windows.Shapes;
using System.Threading;
using System.ComponentModel;

namespace PLWPF
{
    /// <summary>
    /// Interaction logic for Order_ByHost.xaml
    /// </summary>
    public partial class Order_ByHost : Window
    {
         BE.Order order1;
        BackgroundWorker workerThread;
        BL.IBL bl;
        BE.Order order;
        public Order_ByHost()
        {
            InitializeComponent();

            order = null;
            bl = BL.Factory.GetBL();
            try
            {
                this.OrderCB.ItemsSource = bl.getListOfOrdersByOwnerKey(FindHostWindow.host.HostKey).ToList();
                this.OrderCB.DisplayMemberPath = "OrderKey";
                this.OrderCB.SelectedValuePath = "OrderKey";

                this.statusComboBox.ItemsSource = Enum.GetValues(typeof(BE.Status));
            }
            catch(Exception exp) { exp.ToString(); }
            }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{

        //    System.Windows.Data.CollectionViewSource orderViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("orderViewSource")));
        //    // Load data by setting the CollectionViewSource.Source property:
        //    // orderViewSource.Source = [generic data source]
        //}

        private void OrderCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.OrderCB.SelectedItem is BE.Order)
            {
                this.order = ((BE.Order)this.OrderCB.SelectedItem).GetCopy();
                this.DataContext = order;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                double Commission;
                if (MainWindow.IsEmpty(guestRequestKeyTextBox.Text)) 
                if (MainWindow.IsEmpty(hostingUnitKeyTextBox.Text)) 

                bl = BL.Factory.GetBL();
                bl.CheakDatesAreFree(bl.GetHostingUnitFromOrder(order), bl.GetGuestRequestFromOrder(order).EntryDate, bl.GetGuestRequestFromOrder(order).EndDate);

                Commission = bl.updateStatusOfOrder(order,(int)this.statusComboBox.SelectedItem);
                MessageBox.Show("your details update successfully ." +
                    "Commission is:"+ Commission);
            }
            catch (Exception E) { MessageBox.Show(E.ToString()); }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // a thred that activates sending email to guest
                workerThread = new BackgroundWorker();
                workerThread.DoWork += new DoWorkEventHandler(workerThread_DoWork);
                workerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(workerThread_RunWorkerCompleted);
                workerThread.RunWorkerAsync();
                order1 = order;
               
            }
            catch(Exception E) { MessageBox.Show(E.ToString()); }
        }
        void workerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                bl.sendEmailToCancell(order1);
                bl.DeleteOrder(order);
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        void workerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            

                if (e.Error != null)
                
                    MessageBox.Show("Error: " + e.Error.Message);
                
                else
         
                     MessageBox.Show("email sent.");

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Title != "MainWindow")
                    window.Close();
            }
        }
    }
}

