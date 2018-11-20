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
    public partial class ViewArea : Page
    {
        #region Variables
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private string str_branch = String.Empty;
        private string str_values = String.Empty;
        private string[] stra_coords;
        #endregion

        #region Constructor
        public ViewArea()
        {
            InitializeComponent();
            CreateFaciliteButtons();
        }
        #endregion

        #region Controls
        private void btn_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                
                Facilities fac = new Facilities(btn.Content.ToString());
                this.NavigationService.Navigate(fac);
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void CreateFaciliteButtons()
        {
            try
            {
                XmlDocument xml_configFile = new XmlDocument();                                                
                xml_configFile.Load(str_configFilePath);
                str_branch = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch").Attributes[@"value"].Value;

                XmlNodeList xnList = xml_configFile.SelectNodes(@"BDE.Configuration/" + str_branch + "/Aerial_image");

                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFB0D99D"));

                foreach (XmlNode xn1 in xnList)
                {
                    foreach (XmlNode xn2 in xn1)
                    {
                        str_values = xn2.FirstChild.Attributes[@"value"].Value;
                        stra_coords = str_values.Split(';');

                        Button btn = new Button()
                        {
                            BorderBrush = System.Windows.Media.Brushes.White,
                            BorderThickness = new Thickness(2),
                            Name = xn2.Name,
                            Content = xn2.Name,
                            Background = System.Windows.Media.Brushes.Transparent,
                            Margin = new Thickness(Convert.ToDouble(stra_coords[0]), Convert.ToDouble(stra_coords[1]), 0, 0),
                            Foreground = mySolidColorBrush,
                            FontWeight = FontWeights.Bold,
                            FontSize = 25,
                            Width = Convert.ToDouble(stra_coords[2]),
                            Height = Convert.ToDouble(stra_coords[3]),
                        };
                        AreaGrid.Children.Add(btn);

                        btn.Click += new RoutedEventHandler(btn_button_Click);
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
