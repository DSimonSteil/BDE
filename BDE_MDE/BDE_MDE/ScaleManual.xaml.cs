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
    public partial class ScaleManual : Page
    {
        #region Variables        
        private Scale sc;
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
        public ScaleManual(Facilities facility, Scale scale, string str_actualBox, string str_actualFacility)
        {
            try
            {
                InitializeComponent();

                tbx_actualBox.Text = @"Box: " + str_actualBox;
                tbx_actualFacility.Text = str_actualFacility;
                sc = scale;
                fac = facility;

                mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mw.btn_logout.IsEnabled = false;
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        #endregion

        #region Controls
        private void Btn_autoWeight_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                NavigationService.Navigate(sc);
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
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

                    //if ((new Regex(@"^[0],\d\d?$|^[1]?\d,\d?\d?$|^[1]?\d$")).IsMatch(tbx_actualWeight.Text))
                    //{
                    //    tbx_actualWeight.Foreground = Brushes.Black;
                    //    btn_scale.IsEnabled = true;
                    //}
                    //else
                    //{
                    //    tbx_actualWeight.Foreground = Brushes.Red;
                    //    btn_scale.IsEnabled = false;
                    //}
                    //if (button.Content.Equals(","))
                    //{
                    //    if (int_cCount == 0 && !String.IsNullOrEmpty(tbx_actualWeight.Text))
                    //    {
                    //        tbx_actualWeight.Text += button.Content;
                    //        int_cCount = 1;
                    //    }
                    //    else { }
                    //}                                             
                    //else
                    //{
                    //    if (tbx_actualWeight.Text.Contains(","))
                    //    {
                    //        str_weight = tbx_actualWeight.Text.Substring(tbx_actualWeight.Text.IndexOf(','), tbx_actualWeight.Text.Length - 1);

                    //        if (str_weight.Length < 3)
                    //        {
                    //            tbx_actualWeight.Text += button.Content;
                    //        }                            
                    //    }
                    //    else
                    //    {
                    //        tbx_actualWeight.Text += button.Content;
                    //    }                        
                    //}
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
                ScaleProcess sp = new ScaleProcess(this, sc, tbx_actualFacility.Text, tbx_actualBox.Text, tbx_actualWeight.Text + @" TO");
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
                mw.btn_facilities.IsEnabled = true;
                mw.btn_logout.IsEnabled = true;
                this.NavigationService.Navigate(fac);
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
