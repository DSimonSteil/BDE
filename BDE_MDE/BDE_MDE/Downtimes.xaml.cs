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
    public partial class Downtimes : Page
    {
        #region Variables
        private MainWindow mw;
        #endregion
        public Downtimes()
        {
            InitializeComponent();
            CreateDownTimeButtons();
            mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mw.btn_facilities.IsEnabled = true;
            mw.btn_logout.IsEnabled = true;
        }

        #region Help-Methods
        private void CreateDownTimeButtons()
        {
            try
            {
                XmlDocument xml_configFile = new XmlDocument();
                XmlDocument xml_downtimeFile = new XmlDocument();
                string str_source = String.Empty;

                xml_configFile.Load(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml");

                string str_downtimePath = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Downtimes").Attributes[@"value"].Value;

                xml_downtimeFile.Load(str_downtimePath);

                XmlElement rootNode = xml_downtimeFile.DocumentElement;
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFB0D99D"));

                foreach (XmlNode x in rootNode.ChildNodes)
                {
                    foreach (XmlNode y in x.SelectNodes("REASON"))
                    {
                        Button btn = new Button()
                        {
                            BorderBrush = System.Windows.Media.Brushes.White,
                            //Name = y.InnerText,
                            Content = y.InnerText,
                            Background = mySolidColorBrush,
                            Margin = new Thickness(30, 5, 0, 0),
                            FontWeight = FontWeights.Bold,
                            FontSize = 16,
                            Width = 400,
                            Height = 100,
                        };
                        wp_buttons.Children.Add(btn);                        
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
