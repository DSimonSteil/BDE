using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
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
    public partial class Login : Page
    {
        #region Variables
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private string str_actualUser = String.Empty;
        private MainWindow mw;
        #endregion

        #region Constructor
        public Login()
        {
            InitializeComponent();
            mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mw.btn_logout.IsEnabled = false;
            tbx_rfidNr.Focus();
        }
        #endregion

        #region Controls
        private void btn_loginSimulation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AuthorizeFromXML())
                {                    
                    mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    mw.btn_facilities.IsEnabled = true;
                    mw.btn_downtimes.IsEnabled = true;
                    mw.tbx_actualUser.Text = "Fahrer: " + System.Environment.NewLine + str_actualUser;
                    mw.btn_logout.IsEnabled = true;
                    ChangeFacility cF = new ChangeFacility();
                    this.NavigationService.Navigate(cF);
                    cF.btn_cancel.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("Keinen Benutzer gefunden");
                }                
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }                    
        }

        private void Btn_manualLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                LoginManual lm = new LoginManual();
                this.NavigationService.Navigate(lm);
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

        private void Tbx_rfidNr_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (AuthorizeFromXML())
                    {
                        MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                        mw.btn_facilities.IsEnabled = false;
                        mw.btn_downtimes.IsEnabled = true;
                        mw.tbx_actualUser.Text = "Fahrer: " + System.Environment.NewLine + str_actualUser;
                        ChangeFacility cF = new ChangeFacility();
                        this.NavigationService.Navigate(cF);
                        cF.btn_cancel.IsEnabled = false;                        
                    }
                    else
                    {
                        MessageBox.Show("Keinen Benutzer gefunden");
                    }
                }
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        private void Tbx_rfidNr_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.Equals(tbx_rfidNr.Text.Length -1, System.Environment.NewLine))
                {
                    if (AuthorizeFromXML())
                    {
                        MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                        mw.btn_facilities.IsEnabled = true;
                        mw.btn_downtimes.IsEnabled = true;
                        mw.tbx_actualUser.Text = "Fahrer: " + System.Environment.NewLine + str_actualUser;
                        ChangeFacility cF = new ChangeFacility();
                        this.NavigationService.Navigate(cF);
                        cF.btn_cancel.IsEnabled = false;
                    }
                    else
                    {
                        MessageBox.Show("Keinen Benutzer gefunden");
                    }
                }
                
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        #endregion

        #region Help Methods        
        private bool AuthorizeFromXML()
        {
            try
            {
                XmlDocument xml_configFile = new XmlDocument();
                XmlDocument xml_authFile = new XmlDocument();
                string str_source = String.Empty;
                

                xml_configFile.Load(str_configFilePath);

                string str_authorizePath = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Userdata").Attributes[@"value"].Value;

                xml_authFile.Load(str_authorizePath);
                
                XmlElement rootNode = xml_authFile.DocumentElement;

                if (tbx_rfidManuell.IsVisible && !String.IsNullOrEmpty(tbx_rfidManuell.Text))
                {
                    str_source = tbx_rfidManuell.Text;
                }
                else
                {
                    str_source = tbx_rfidNr.Text;
                }

                foreach (XmlNode x in rootNode.ChildNodes)
                {
                    foreach (XmlNode y in x.SelectNodes("ZAUSW"))
                    {
                        if (y.InnerText.Equals(str_source))
                        {
                            str_actualUser = x["VORNA"].InnerText + " " + x["NACHN"].InnerText;
                            ConfigClass.strEmpName = x["VORNA"].InnerText + " " + x["NACHN"].InnerText;
                            ConfigClass.strEmpNo = x["PERNR"].InnerText;

                            TimeReport.CreateReport("P10", "", "");
                            return true;
                        }                        
                    }
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Feedback
        private void Feedback(Exception exc)
        {
            Feedback fb = new Feedback();

            fb.FeedbackHandler(exc);
        }
        #endregion        
    }
}
