using System;
using System.Collections.Generic;
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
    public partial class Facilities : Page
    {
        #region Variables
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private string str_branch = String.Empty;
        private string str_faciltyImgPath = String.Empty;
        private string str_values = String.Empty;
        private string[] stra_coords;
        private string str_actualFac = String.Empty;
        #endregion

        #region Constructor
        public Facilities(string str_actualFacility)
        {
            InitializeComponent();
            RenderFacility(str_actualFacility);
            CreateBoxes(str_actualFacility);
            str_actualFac = str_actualFacility;
        }
        #endregion

        #region Controls
        private void btn_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;

                Scale sca = new Scale(this, btn.Content.ToString(), str_actualFac);
                this.NavigationService.Navigate(sca);
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        #endregion

        #region Help Methods
        public void RenderFacility(string str_actualFacility)
        {
            try
            {
                GetXmlInfos(str_actualFacility);
                img_actualFacilty.Source = new BitmapImage(new Uri(str_faciltyImgPath));

                MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mw.tbx_facility.Text = @"Aktuelle Anlage: " + System.Environment.NewLine + str_actualFacility;
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        public void GetXmlInfos(string str_actualFacility)
        {
            try
            {
                XmlDocument xml_configFile = new XmlDocument();                
                xml_configFile.Load(str_configFilePath);

                str_branch = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch").Attributes[@"value"].Value;
                str_faciltyImgPath = xml_configFile.SelectSingleNode(@"BDE.Configuration/" + str_branch + @"/Aerial_image/" + str_actualFacility + @"/imagePath").Attributes[@"value"].Value;                
            }
            catch (Exception exc)
            {
                Feedback(exc);                
            }
        }

        public void CreateBoxes(string str_actualFacility)
        {
            try
            {
                XmlDocument xml_configFile = new XmlDocument();
                xml_configFile.Load(str_configFilePath);                

                XmlNodeList xnList = xml_configFile.SelectNodes(@"BDE.Configuration/" + str_branch + "/Facilities/" + str_actualFacility);

                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFB0D99D"));

                foreach (XmlNode xn1 in xnList)
                {
                    foreach (XmlNode xn2 in xn1.ChildNodes)
                    {                        
                        foreach (XmlNode xn3 in xn2.ChildNodes)
                        {
                            str_values = xn3.Attributes[@"coords"].Value;
                            stra_coords = str_values.Split(';');

                            Button btn = new Button()
                            {
                                Name = xn3.Name,
                                Content = xn3.Name.Substring(1),
                                Background = mySolidColorBrush,
                                Margin = new Thickness(Convert.ToDouble(stra_coords[0]), Convert.ToDouble(stra_coords[1]), 0, 0),
                                FontWeight = FontWeights.Bold,
                                FontSize = 35,
                                Width = Convert.ToDouble(stra_coords[2]),
                                Height = Convert.ToDouble(stra_coords[3]),
                            };
                            btn.Click += new RoutedEventHandler(btn_button_Click);
                            FacilityGrid.Children.Add(btn);                                                                               
                        }                                                
                    }                    
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
