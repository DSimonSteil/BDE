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
        private string str_actualuserID = String.Empty;
        #endregion
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
                if (AuthorizeUser())
                {
                    MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                    mw.btn_facilities.IsEnabled = true;
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

        #region Help Methods
        private bool AuthorizeUser()
        {
            try
            {
                XmlDocument xml_configFile = new XmlDocument();
                xml_configFile.Load(str_configFilePath);

                string str_authorizePath = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Userdata").Attributes[@"value"].Value;
                using (DataTable dt = new DataTable())
                using (StreamReader sr = new StreamReader(str_authorizePath))
                {
                    string[] headers = sr.ReadLine().Split(';');
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header);
                    }
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(';');
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        dt.Rows.Add(dr);
                    }

                    string str_key = "Ztauswnr = " + tbx_rfidNr.Text;

                    DataRow[] foundRows;

                    foundRows = dt.Select(str_key);

                    if (foundRows.Count() > 0)
                    {
                        str_actualUser = foundRows[0][6].ToString();                        
                        Application.Current.Windows.OfType<MainWindow>().FirstOrDefault().lbl_employeeID.Content = str_actualUser[5].ToString();
                        return true;
                    }
                    else
                    {
                        return false;
                    }                    
                }
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
