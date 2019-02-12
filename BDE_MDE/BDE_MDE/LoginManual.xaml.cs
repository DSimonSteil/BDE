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
    public partial class LoginManual : Page
    {
        #region Variables
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private string str_actualUser = String.Empty;
        private MainWindow mw;
        #endregion
        public LoginManual()
        {
            InitializeComponent();
            mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mw.btn_logout.IsEnabled = false;            
        }

        #region Controls
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbx_loginNo.Text.Equals(@"Personalnummer"))
                {
                    tbx_loginNo.Text = String.Empty;
                }
                Button button = sender as Button;

                if (!button.Content.Equals("←") && !button.Content.Equals("C"))
                {
                    tbx_loginNo.Text += button.Content;
                }
                else if (button.Content.Equals("C"))
                {
                    tbx_loginNo.Text = @"Personalnummer";
                }
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void Btn_login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (AuthorizeFromXML())
                {
                    mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();                    
                    mw.tbx_actualUser.Text = "Fahrer: " + System.Environment.NewLine + str_actualUser;
                    mw.btn_logout.IsEnabled = true;
                    mw.btn_facilities.IsEnabled = false;
                    mw.btn_downtimes.IsEnabled = true;
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

                str_source = tbx_loginNo.Text;
                                
                foreach (XmlNode x in rootNode.ChildNodes)
                {
                    foreach (XmlNode y in x.SelectNodes(@"PERNR"))
                    {
                        if (y.InnerText.Equals(str_source))
                        {
                            str_actualUser = x["VORNA"].InnerText + " " + x["NACHN"].InnerText;
                            ConfigClass.strEmpName = x["VORNA"].InnerText + " " + x["NACHN"].InnerText;
                            ConfigClass.strEmpNo = x["PERNR"].InnerText;
                            //TimeReport.CreateReport("P10", "", "");
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NavigationService.GoBack();
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
            Feedback fb = new Feedback();

            fb.FeedbackHandler(exc);
        }
        #endregion        
    }
}
