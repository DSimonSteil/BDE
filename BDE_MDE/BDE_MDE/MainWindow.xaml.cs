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

namespace BDE_MDE
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame.Content = new Login();
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void btn_radlader_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame.Content = new ChangeFacility();
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }


        private void Feedback(Exception exc)
        {
            tbx_feedback.Visibility = Visibility.Visible;
            tbx_feedback.Text += exc.GetType().ToString() + @" @ " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name 
                + System.Environment.NewLine + exc.Message + System.Environment.NewLine + System.Environment.NewLine;
        }        
    }
}