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
using System.DirectoryServices.AccountManagement;
using System.Windows.Threading;
using System.Xml;
using System.Net.Mail;

namespace BDE_MDE
{
    public partial class MainWindow : Window
    {
        #region Variables
        public string str_empNumber = String.Empty;
        #endregion
        
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();            
            MainFrame.Content = new Login();

            //tbx_actualUser.Text = @"Fahrer: " + System.Environment.NewLine + System.DirectoryServices.AccountManagement.UserPrincipal.Current.DisplayName;
            //tbx_facility.Text = @"Aktuelle Maschine: " + System.Environment.NewLine + @"Testmaschine";            

            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {
                this.tbx_actualTime.Text = DateTime.Now.ToString();
            }, this.Dispatcher);

            //XmlReader xr = new XmlReader();
            //xr.ReadXmlParameters();
        }
        #endregion

        #region Controls
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

        private void btn_logout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btn_facilities.IsEnabled = false;
                tbx_actualUser.Text = "Fahrer: ";
                tbx_facility.Text = "Aktuelle Anlage: ";
                MainFrame.Content = new Login();
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void btn_facilty_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                object o = MainFrame.Content;

                MainFrame.Content = new ChangeFacility();
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        #endregion Controls

        #region Feedback
        public void Feedback(Exception exc)
        {
            Feedback fb = new Feedback();

            fb.FeedbackHandler(exc);
        }        
        #endregion Feedback
    }

    
}
