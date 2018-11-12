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
    /// Interaction logic for Facilities.xaml
    /// </summary>
    public partial class Facilities : Page
    {
        #region Variables
        string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        string str_companyLocation = String.Empty;
        string str_faciltyImgPath = String.Empty;
        #endregion
        public Facilities(string str_actualFacility)
        {
            InitializeComponent();
            RenderFacility(str_actualFacility);
        }

        public void RenderFacility(string str_actualFacility)
        {
            try
            {
                GetXmlInfos(str_actualFacility);
                img_actualFacilty.Source = new BitmapImage(new Uri(str_faciltyImgPath));

                MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mw.tbx_facility.Text = @"Aktuelle Maschine: " + System.Environment.NewLine + str_actualFacility;
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
                //string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
                xml_configFile.Load(str_configFilePath);

                str_companyLocation = xml_configFile.SelectSingleNode(@"BDE.Configuration/general/Niederlassung").Attributes[@"value"].Value;
                str_faciltyImgPath = xml_configFile.SelectSingleNode(@"BDE.Configuration/" + str_companyLocation + @"/platz_luftbild/" + str_actualFacility + @"/imagePath").Attributes[@"value"].Value;
                //XmlNodeList xnList = xml_configFile.SelectNodes(@"BDE.Configuration/" + str_companyLocation + @"/platz_luftbild");                
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
