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
    public partial class ScaleProcess : Page
    {
        #region Variables
        private Scale sc;
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private string str_weightGuid = String.Empty;
        private string[] stra_employee;
        #endregion

        #region Constructor
        public ScaleProcess(Scale scale, string str_facility, string str_boxID, string str_actualWeight)
        {
            InitializeComponent();

            sc = scale;
            tbx_boxID.Text = str_boxID;
            tbx_facility.Text = str_facility;
            tbx_date.Text = DateTime.Now.Date.ToShortDateString();
            tbx_time.Text = DateTime.Now.ToString("HH:mm");
            tbx_weight.Text = str_actualWeight;
            stra_employee = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault().tbx_actualUser.Text.Split(':'); ;
            tbx_employee.Text = stra_employee[1].Trim();
        }
        #endregion

        #region Controls
        private void Btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CreateScaleXML();
                           
                NavigationService.Navigate(sc);

                sc.tbx_lastScale.Text = @"Letzte Wiegung:" + System.Environment.NewLine + 
                                         tbx_boxID.Text + @" Uhrzeit: " + tbx_time.Text + System.Environment.NewLine + 
                                         @"Gewicht: " + tbx_weight.Text;
                sc.btn_deleteLastScale.IsEnabled = true;
                sc.tbx_actualWeight.Text = String.Empty;                
                sc.tbx_actualWeight.IsEnabled = false;
                sc.btn_scale.IsEnabled = false;
                sc.lbl_lastWeightGuid.Content = str_weightGuid;
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
                NavigationService.Navigate(sc);                
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        #endregion

        #region Help Methods
        private void CreateScaleXML()
        {
            try
            {
                XmlDocument xml_configFile = new XmlDocument();
                xml_configFile.Load(str_configFilePath);
                string str_xmlDestinationPath = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/ScalePath").Attributes[@"value"].Value;
                string str_branch = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch").Attributes[@"value"].Value;
                string[] stra_box = tbx_boxID.Text.Split(':');

                using (XmlWriter writer = XmlWriter.Create(str_xmlDestinationPath + "/" + DateTime.Now.Ticks + ".xml"))
                {
                    writer.WriteStartElement("Scale");
                    writer.WriteElementString("Branch", str_branch);
                    writer.WriteElementString("Employee", tbx_employee.Text);
                    writer.WriteElementString("WeightDate", DateTime.Now.ToString("yyyyMMdd"));
                    writer.WriteElementString("WeightTime", DateTime.Now.ToString("HHmmss"));
                    writer.WriteElementString("Facility", tbx_facility.Text);
                    writer.WriteElementString("Box", stra_box[1].Trim());
                    writer.WriteElementString("Weight", tbx_weight.Text);
                    str_weightGuid = Guid.NewGuid().ToString();
                    writer.WriteElementString("GUID", str_weightGuid);
                    //writer.WriteElementString("Material", tbx_weight.Text);
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
