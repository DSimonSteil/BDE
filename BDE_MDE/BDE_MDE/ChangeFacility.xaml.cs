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
    /// <summary>
    /// Interaction logic for ChangeFacility.xaml
    /// </summary>
    public partial class ChangeFacility : Page
    {
        #region Variables
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private string str_branch = String.Empty;        
        #endregion
        public ChangeFacility()
        {
            InitializeComponent();
            CreateFacilityButtons();
        }

        private void btn_birdview_Click(object sender, RoutedEventArgs e)
        {
            ViewArea va = new ViewArea();
            this.NavigationService.Navigate(va);
        }

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

        private void CreateFacilityButtons()
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
                        Button btn = new Button()
                        {
                            BorderBrush = System.Windows.Media.Brushes.White,
                            //BorderThickness = new Thickness(2),
                            Name = xn2.Name,
                            Content = xn2.Name,
                            Background = mySolidColorBrush,
                            Margin = new Thickness(30, 5, 0, 0),                            
                            FontWeight = FontWeights.Bold,
                            FontSize = 16,
                            Width = 400,
                            Height = 100,                            
                        };
                        wp_buttons.Children.Add(btn);

                        btn.Click += new RoutedEventHandler(btn_button_Click);
                    }
                }
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }                       
        }

        private void Feedback(Exception exc)
        {
            MessageBox.Show(exc.GetType().ToString() + @" @ " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name
               + System.Environment.NewLine + exc.Message + System.Environment.NewLine + System.Environment.NewLine);
        }
    }
}
