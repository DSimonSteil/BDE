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

namespace BDE_MDE
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        #region Constructor
        public Login()
        {
            InitializeComponent();           
        }
        #endregion

        #region Controls
        private void btn_loginSimulation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mw.btn_facilities.IsEnabled = true;

                ChangeFacility cF = new ChangeFacility();
                this.NavigationService.Navigate(cF);
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }                    
        }

        private void btn_settings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings s = new Settings();
                this.NavigationService.Navigate(s);
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        #endregion

        #region Feedback
        private void Feedback(Exception exc)
        {
            MessageBox.Show(exc.GetType().ToString() + @" @ " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name
               + System.Environment.NewLine + exc.Message + System.Environment.NewLine + System.Environment.NewLine);
        }
        #endregion
    }
}
