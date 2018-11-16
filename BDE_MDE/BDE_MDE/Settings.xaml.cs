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
using System.Xml;

namespace BDE_MDE
{
    public partial class Settings : Page
    {
        #region Variables
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private XmlDocument xml_configFile;
        #endregion

        #region Constructor
        public Settings()
        {
            InitializeComponent();
        }
        #endregion

        #region Controls
        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Login login = new Login();
                this.NavigationService.Navigate(login);
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void Btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (pwb_password.Password.Equals(@"steil"))
                {
                    tbx_loader.IsEnabled = true;
                    cbx_branch.IsEnabled = true;
                    btn_transfer.IsEnabled = true;
                    FillControls();
                }
                else { }
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void Btn_transfer_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                xml_configFile.Load(str_configFilePath);
                xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Sign").Attributes[@"value"].Value = tbx_loader.Text;
                xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch").Attributes[@"value"].Value = cbx_branch.Text;                
                xml_configFile.Save(str_configFilePath);

                MessageBox.Show("Parameter erfolgreich geändert.");

                Login login = new Login();
                this.NavigationService.Navigate(login);
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        #endregion

        #region Help Methods
        private void FillControls()
        {
            try
            {
                xml_configFile = new XmlDocument();
                xml_configFile.Load(str_configFilePath);
                tbx_loader.Text = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Sign").Attributes[@"value"].Value;
                cbx_branch.Text = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch").Attributes[@"value"].Value;                
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
