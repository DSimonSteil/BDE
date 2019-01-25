using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class ScalePage : Page
    {
        #region Variables                
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private XmlDocument xml_configFile;
        private string str_xmlDestinationPath;
        private int int_cCount = 0;
        private char lc;
        private string str_weight = String.Empty;
        private MainWindow mw;
        private Facilities fac;
        #endregion

        #region Constructor
        public ScalePage(Facilities facility, string str_actualBox, string str_actualFacility)
        {
            InitializeComponent();

            tbx_actualBox.Text = @"Box: " + str_actualBox;
            tbx_actualFacility.Text = str_actualFacility;
            
            fac = facility;
            
            mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mw.btn_logout.IsEnabled = false;
        }
        #endregion

        #region Controls
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tbx_actualWeight.Text.Equals("Gewicht in Tonnen"))
                {
                    tbx_actualWeight.Text = String.Empty;
                }
                Button button = sender as Button;

                if (!button.Content.Equals("←") && !button.Content.Equals("C"))
                {
                    tbx_actualWeight.Text += button.Content;                    
                }
                else
                {
                    if (button.Content.Equals("←"))
                    {
                        str_weight = tbx_actualWeight.Text;

                        if (str_weight.Length > 1)
                        {
                            //lc = str_weight[str_weight.Length - 1];
                            str_weight = str_weight.Substring(0, str_weight.Length - 1);
                        }
                        else
                        {
                            str_weight = String.Empty;
                            tbx_actualWeight.Text = @"Gewicht in Tonnen";

                            tbx_actualWeight.Foreground = Brushes.Black;
                            btn_scale.IsEnabled = true;
                            //int_cCount = 0;
                        }
                        tbx_actualWeight.Text = str_weight;
                    }
                    else if (button.Content.Equals("C"))
                    {
                        tbx_actualWeight.Text = @"Gewicht in Tonnen";

                        tbx_actualWeight.Foreground = Brushes.Black;
                        btn_scale.IsEnabled = true;
                        //int_cCount = 0;
                    }
                }

                if ((new Regex(@"^(?:(?=0)([0],\d{1,2})|[1-9]?\d(,\d{1,2})?)$")).IsMatch(tbx_actualWeight.Text))
                {
                    tbx_actualWeight.Foreground = Brushes.Black;
                    btn_scale.IsEnabled = true;
                }
                else
                {
                    if (tbx_actualWeight.Text.Equals("Gewicht in Tonnen"))
                    {
                        tbx_actualWeight.Foreground = Brushes.Black;
                        btn_scale.IsEnabled = false;
                    }
                    else
                    {
                        tbx_actualWeight.Foreground = Brushes.Red;
                        btn_scale.IsEnabled = false;
                    }
                }

                //if (tbx_actualWeight.Text.Length > 0)
                //{
                //    btn_scale.IsEnabled = true;
                //}
                //else
                //{
                //    tbx_actualWeight.Text = @"Gewicht in Tonnen";
                //    btn_scale.IsEnabled = false;
                //}
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void Btn_scale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ScaleProcess sp = new ScaleProcess(this, tbx_actualFacility.Text, tbx_actualBox.Text, tbx_actualWeight.Text + @" to");
                this.NavigationService.Navigate(sp);
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void Btn_closeBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TimeReport.CreateReport(@"B40", tbx_actualFacility.Text, tbx_actualBox.Text.Split(':')[1].Trim());

                mw.btn_facilities.IsEnabled = true;
                this.NavigationService.Navigate(fac);

            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        private void Btn_deleteLastScale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Letzte Wiegung wirklich löschen?", "Lösch-Bestätigung", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    CreateDeleteXML();
                    btn_deleteLastScale.IsEnabled = false;
                    tbx_lastScale.Text = @"erfolgreich gelöscht";
                }
                else { }
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        #endregion

        #region Help-Methods
        private void CreateDeleteXML()
        {
            try
            {
                xml_configFile = new XmlDocument();
                xml_configFile.Load(str_configFilePath);

                str_xmlDestinationPath = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/ScalePath").Attributes[@"value"].Value;

                using (XmlWriter writer = XmlWriter.Create(str_xmlDestinationPath + "/ToDelete_" + DateTime.Now.Ticks + @".xml"))
                {
                    writer.WriteStartElement("Scale");
                    writer.WriteElementString("GUID", lbl_lastWeightGuid.Content.ToString());
                    writer.WriteElementString("WDELETE", "X");
                    writer.WriteEndElement();
                    writer.Flush();
                }
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
