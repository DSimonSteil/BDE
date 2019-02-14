using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
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
        private ScaleManual sm;
        private Scale scmo;
        private ScalePage sPage;
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private string str_weightGuid = String.Empty;
        private string[] stra_employee;

        private XmlDocument xml_configFile;
        private string str_xmlDestinationPath = String.Empty;
        private string str_branch = String.Empty;
        private string str_branchNo = String.Empty;
        private string str_vehicle = String.Empty;
        private string str_vehicle_EQ = String.Empty;
        private string[] stra_box;
        private string[] stra_weight;
        private string str_sapIP = String.Empty;

        private RfcDestination dest;
        private RfcRepository rep;
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

        public ScaleProcess(ScaleManual scaleManual, Scale scm, string str_facility, string str_boxID, string str_actualWeight)
        {
            InitializeComponent();

            sm = scaleManual;
            scmo = scm;

            tbx_boxID.Text = str_boxID;
            tbx_facility.Text = str_facility;
            tbx_date.Text = DateTime.Now.Date.ToShortDateString();
            tbx_time.Text = DateTime.Now.ToString("HH:mm");
            tbx_weight.Text = str_actualWeight;
            stra_employee = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault().tbx_actualUser.Text.Split(':'); ;
            tbx_employee.Text = stra_employee[1].Trim();
        }

        public ScaleProcess(ScalePage scalePage, string str_facility, string str_boxID, string str_actualWeight)
        {
            InitializeComponent();

            sPage = scalePage;
            
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

                if (xml_configFile.SelectSingleNode(@"BDE.Configuration/General/ManualWeight").Attributes[@"value"].Value.Equals(@"yes"))
                {
                    NavigationService.Navigate(sPage);

                    sPage.tbx_lastScale.Text = @"Letzte Wiegung:" + System.Environment.NewLine +
                                            tbx_boxID.Text + @" Uhrzeit: " + tbx_time.Text + System.Environment.NewLine +
                                            @"Gewicht: " + tbx_weight.Text;
                    sPage.btn_deleteLastScale.IsEnabled = true;
                    sPage.tbx_actualWeight.Text = String.Empty;
                    sPage.tbx_actualWeight.IsEnabled = false;
                    sPage.btn_scale.IsEnabled = false;
                    sPage.tbx_actualWeight.ClearValue(TextBox.BackgroundProperty);
                    sPage.lbl_lastWeightGuid.Content = str_weightGuid;
                    sPage.tbx_actualWeight.Text = "Gewicht in Tonnen";
                }
                else
                {
                    if (sc != null)
                    {
                        NavigationService.Navigate(sc);

                        sc.tbx_lastScale.Text = @"Letzte Wiegung:" + System.Environment.NewLine +
                                                tbx_boxID.Text + @" Uhrzeit: " + tbx_time.Text + System.Environment.NewLine +
                                                @"Gewicht: " + tbx_weight.Text;
                        sc.btn_deleteLastScale.IsEnabled = true;
                        sc.tbx_actualWeight.Text = String.Empty;
                        sc.tbx_actualWeight.IsEnabled = false;
                        sc.btn_scale.IsEnabled = false;
                        sc.tbx_actualWeight.ClearValue(TextBox.BackgroundProperty);
                        sc.lbl_lastWeightGuid.Content = str_weightGuid;
                    }
                    else
                    {
                        NavigationService.Navigate(scmo);

                        scmo.tbx_lastScale.Text = @"Letzte Wiegung:" + System.Environment.NewLine +
                                                tbx_boxID.Text + @" Uhrzeit: " + tbx_time.Text + System.Environment.NewLine +
                                                @"Gewicht: " + tbx_weight.Text;
                        scmo.btn_deleteLastScale.IsEnabled = true;
                        scmo.tbx_actualWeight.Text = String.Empty;
                        scmo.tbx_actualWeight.IsEnabled = false;
                        scmo.btn_scale.IsEnabled = false;
                        scmo.tbx_actualWeight.ClearValue(TextBox.BackgroundProperty);
                        scmo.lbl_lastWeightGuid.Content = str_weightGuid;
                    }
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
                if (sm == null)
                {
                    if (sPage == null)
                    {
                        NavigationService.Navigate(sc);
                    }
                    else
                    {
                        NavigationService.Navigate(sPage);
                    }
                }
                else
                {
                    NavigationService.Navigate(sm);
                }                
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
                xml_configFile = new XmlDocument();
                xml_configFile.Load(str_configFilePath);

                str_xmlDestinationPath = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/ScalePath").Attributes[@"value"].Value;
                str_branch = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch").Attributes[@"value"].Value;
                str_branchNo = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch_NL").Attributes[@"value"].Value;
                str_vehicle = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Vehicle").Attributes[@"value"].Value;
                str_vehicle_EQ = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Vehicle_EQ").Attributes[@"value"].Value;
                stra_box = tbx_boxID.Text.Split(':');
                str_sapIP = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/SAP_IP").Attributes[@"value"].Value;
                str_weightGuid = Guid.NewGuid().ToString();

                if (CheckConnection.PingSAP(str_sapIP, @"CreateScaleXML"))
                {                                                            
                    dest = RfcDestinationManager.GetDestination(xml_configFile.SelectSingleNode(@"BDE.Configuration/General/SAP_Connection").Attributes[@"value"].Value);                    
                    rep = dest.Repository;

                    var rfc_read_table = rep.CreateFunction("Z_PP_BDE_IMPORT");

                    IRfcStructure struct_bde_weight = rfc_read_table.GetStructure("IS_BDE_WEIGHT");

                    struct_bde_weight.SetValue("WERKS", str_branchNo);
                    //struct_bde_weight.SetValue("ANLAGE", tbx_facility.Text);
                    struct_bde_weight.SetValue("ANLAGE", xml_configFile.SelectSingleNode(@"BDE.Configuration/" + str_branch + "/Aerial_image/" + tbx_facility.Text + "/EQ").Attributes[@"value"].Value);
                    struct_bde_weight.SetValue("VEHICLE", str_vehicle_EQ);
                    struct_bde_weight.SetValue("BOX", stra_box[1].Trim());
                    struct_bde_weight.SetValue("GUID", str_weightGuid);
                    struct_bde_weight.SetValue("PERNR", ConfigClass.strEmpNo);

                    stra_weight = tbx_weight.Text.Split(' ');

                    struct_bde_weight.SetValue("GEWICHT", stra_weight[0]);
                    struct_bde_weight.SetValue("MEINS", stra_weight[1]);
                    struct_bde_weight.SetValue("DATUM", DateTime.Now.ToString("yyyyMMdd"));
                    struct_bde_weight.SetValue("UZEIT", DateTime.Now.ToString("HHmmss"));
                    struct_bde_weight.SetValue("WDELETE", "");
                    rfc_read_table.Invoke(dest);

                    var returnState = rfc_read_table.GetValue("EF_STATE");                    

                    if (!returnState.Equals("X"))
                    {
                        using (XmlWriter writer = XmlWriter.Create(str_xmlDestinationPath + "/" + DateTime.Now.Ticks + ".xml"))
                        {
                            writer.WriteStartElement("Scale");                            
                            writer.WriteElementString("BranchNo", str_branchNo);                            
                            writer.WriteElementString("VehicleEQ", str_vehicle_EQ);                            
                            writer.WriteElementString("EmployeeNo", ConfigClass.strEmpNo);
                            writer.WriteElementString("WeightDate", DateTime.Now.ToString("yyyyMMdd"));
                            writer.WriteElementString("WeightTime", DateTime.Now.ToString("HHmmss"));
                            //writer.WriteElementString("Facility", tbx_facility.Text);
                            struct_bde_weight.SetValue("Facility", xml_configFile.SelectSingleNode(@"BDE.Configuration/" + str_branch + "/Aerial_image/" + tbx_facility.Text + "/EQ").Attributes[@"value"].Value);
                            writer.WriteElementString("Box", stra_box[1].Trim());

                            string[] stra_weightXML = tbx_weight.Text.Split(' ');

                            writer.WriteElementString("Weight", stra_weight[0]);
                            writer.WriteElementString("Einheit", stra_weight[1]);
                            writer.WriteElementString("GUID", str_weightGuid);
                            writer.WriteElementString("WDELETE", "");
                            writer.WriteEndElement();
                            writer.Flush();
                        }
                    }                    
                }
                else
                {
                    using (XmlWriter writer = XmlWriter.Create(str_xmlDestinationPath + "/" + DateTime.Now.Ticks + ".xml"))
                    {
                        writer.WriteStartElement("Scale");
                        writer.WriteElementString("BranchNo", str_branchNo);
                        writer.WriteElementString("VehicleEQ", str_vehicle_EQ);
                        writer.WriteElementString("EmployeeNo", ConfigClass.strEmpNo);
                        writer.WriteElementString("WeightDate", DateTime.Now.ToString("yyyyMMdd"));
                        writer.WriteElementString("WeightTime", DateTime.Now.ToString("HHmmss"));
                        //writer.WriteElementString("Facility", tbx_facility.Text);
                        writer.WriteElementString("Facility", xml_configFile.SelectSingleNode(@"BDE.Configuration/" + str_branch + "/Aerial_image/" + tbx_facility.Text + "/EQ").Attributes[@"value"].Value);                        
                        writer.WriteElementString("Box", stra_box[1].Trim());

                        string[] stra_weightXML = tbx_weight.Text.Split(' ');

                        writer.WriteElementString("Weight", stra_weightXML[0]);
                        writer.WriteElementString("Einheit", stra_weightXML[1]);
                        writer.WriteElementString("GUID", str_weightGuid);
                        writer.WriteElementString("WDELETE", "");
                        writer.WriteEndElement();
                        writer.Flush();
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
