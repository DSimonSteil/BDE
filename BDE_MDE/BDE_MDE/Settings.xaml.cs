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
            pwb_password.Focus();
        }
        #endregion

        #region Controls

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                Button button = sender as Button;

                if (!button.Content.Equals("←") && !button.Content.Equals("C"))
                {
                    pwb_password.Password += button.Content;                    
                }
                else if (button.Content.Equals("C"))
                {
                    pwb_password.Password = String.Empty;                                            
                }                
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
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
                if (pwb_password.Password.Equals(@"13372018"))
                {                    
                    cbx_branch.IsEnabled = true;
                    cbx_vehicle.IsEnabled = true;
                    btn_transfer.IsEnabled = true;
                    btn_exit.IsEnabled = true;
                    FillControls();
                }
                else { }
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void Btn_exit_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                System.Windows.Application.Current.Shutdown();
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
                xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch").Attributes[@"value"].Value = cbx_branch.Text;
                xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch_NL").Attributes[@"value"].Value = cbx_branch.SelectedValue.ToString();
                xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Vehicle").Attributes[@"value"].Value = cbx_vehicle.Text;
                xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Vehicle_EQ").Attributes[@"value"].Value = cbx_vehicle.SelectedValue.ToString();
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
        private void Cbx_branch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cbx_vehicle.Items.Clear();

                KeyValuePair<int, string> ss = (KeyValuePair<int, string>)cbx_branch.SelectedItem;

                XmlNodeList xnList = xml_configFile.SelectNodes(@"BDE.Configuration/Vehicles/" + ss.Value);
                cbx_vehicle.SelectedValuePath = "Key";
                cbx_vehicle.DisplayMemberPath = "Value";

                foreach (XmlNode xn1 in xnList)
                {
                    foreach (XmlNode xn2 in xn1.ChildNodes)
                    {
                        cbx_vehicle.Items.Add(new KeyValuePair<int, string>(Convert.ToInt32(xn2.Attributes["EQ"].Value), xn2.Attributes["Sign"].Value));
                    }                    
                }
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void Btn_teamviewer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                xml_configFile = new XmlDocument();
                xml_configFile.Load(str_configFilePath);
                string str_tv = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Teamviewer_Path").Attributes[@"value"].Value;
                System.Diagnostics.Process.Start(str_tv);
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
                                
                XmlNodeList xnList = xml_configFile.SelectNodes(@"BDE.Configuration/Branches");
                cbx_branch.SelectedValuePath = "Key";
                cbx_branch.DisplayMemberPath = "Value";

                foreach (XmlNode xn1 in xnList)
                {
                    foreach (XmlNode xn2 in xn1.ChildNodes)
                    {                                              
                        cbx_branch.Items.Add(new KeyValuePair<int, string>(Convert.ToInt32(xn2.Attributes["NL"].Value), xn2.Name));                      
                    }
                }
                cbx_branch.Text = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch").Attributes[@"value"].Value;

                xnList = xml_configFile.SelectNodes(@"BDE.Configuration/Vehicles/" + cbx_branch.Text);
                cbx_vehicle.SelectedValuePath = "Key";
                cbx_vehicle.DisplayMemberPath = "Value";

                foreach (XmlNode xn1 in xnList)
                {
                    foreach (XmlNode xn2 in xn1.ChildNodes)
                    {
                        cbx_vehicle.Items.Add(new KeyValuePair<int, string>(Convert.ToInt32(xn2.Attributes["EQ"].Value), xn2.Attributes["Sign"].Value));
                    }
                }
                cbx_vehicle.Text = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Vehicle").Attributes[@"value"].Value;
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
