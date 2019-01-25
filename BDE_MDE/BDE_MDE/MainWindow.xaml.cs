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
using System.DirectoryServices.AccountManagement;
using System.Windows.Threading;
using System.Xml;
using System.Net.Mail;
using SAP.Middleware.Connector;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace BDE_MDE
{
    public partial class MainWindow : Window
    {
        #region Variables
        public string str_empNumber = String.Empty;
        private XmlDocument xml_configFile;
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private string str_sapIP = String.Empty;

        string str_vehicle_EQ = String.Empty;
        string str_branchNo = String.Empty;

        private RfcDestination rfc_destination;
        private RfcRepository rfc_repository;
        private IRfcFunction rfc_function;
        private IRfcTable irfcTable_returnTable;
        private IRfcStructure struct_bde_weight;
        #endregion

        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            
            xml_configFile = new XmlDocument();
            xml_configFile.Load(str_configFilePath);
            
            if (xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Fullscreen").Attributes[@"value"].Value.Equals("true"))
            {
                this.WindowStyle = WindowStyle.None;
                this.ResizeMode = ResizeMode.NoResize;
                this.Left = 0;
                this.Top = 0;
                this.Width = SystemParameters.VirtualScreenWidth;
                this.Height = SystemParameters.VirtualScreenHeight;                
            }

            DeleteLOGs(Convert.ToInt32(xml_configFile.SelectSingleNode(@"BDE.Configuration/General/LogHoldBackDays").Attributes[@"value"].Value));            

            if (CheckConnection.PingSAP(xml_configFile.SelectSingleNode(@"BDE.Configuration/General/SAP_IP").Attributes[@"value"].Value, "GetSapData"))
            {
                string xml_file = System.IO.Path.GetDirectoryName(xml_configFile.SelectSingleNode(@"BDE.Configuration/General/SapTablesPath").Attributes[@"value"].Value);
                xml_file += @"\\ET_EMPLOYEES.xml";

                FileInfo fi_xmlFile = new FileInfo(xml_file);
                if (fi_xmlFile.LastWriteTime < DateTime.Now.Date)
                {
                    GetSapData();
                }
                GetSapData();
                
            }

            MainFrame.Content = new Login();
                        
            DispatcherTimer timer = new DispatcherTimer(new TimeSpan(0, 0, 1), DispatcherPriority.Normal, delegate
            {                
                this.tbx_actualTime.Text = DateTime.Now.ToString();
            }, this.Dispatcher);

            SapTransporter.CheckXMLs();            
        }
        #endregion

        #region Controls
        private void Btn_downtimes_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame.Content = new Downtimes();
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MainFrame.Content = new Login();
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void btn_logout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] stra_preFacility = tbx_facility.Text.Split(':');
                if (!String.IsNullOrEmpty(stra_preFacility[1].Trim()))
                {
                    TimeReport.CreateReport(@"P20", stra_preFacility[1].Trim(), "");
                }

                TimeReport.LogOff(xml_configFile);
                btn_facilities.IsEnabled = false;
                tbx_actualUser.Text = "Fahrer: ";
                tbx_facility.Text = "Aktuelle Anlage: ";
                MainFrame.Content = new Login();
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void btn_facilty_Click(object sender, RoutedEventArgs e)
        {
            try
            {                
                object o = MainFrame.Content;

                MainFrame.Content = new ChangeFacility();
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        #endregion Controls

        #region Help-Methods
        private void GetSapData()
        {
            try
            {                
                xml_configFile = new XmlDocument();
                xml_configFile.Load(str_configFilePath);

                str_vehicle_EQ = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Vehicle_EQ").Attributes[@"value"].Value;
                str_branchNo = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch_NL").Attributes[@"value"].Value;

                rfc_destination = RfcDestinationManager.GetDestination(xml_configFile.SelectSingleNode(@"BDE.Configuration/General/SAP_Connection").Attributes[@"value"].Value);                

                rfc_repository = rfc_destination.Repository;

                rfc_function = rfc_repository.CreateFunction("Z_PP_BDE_STATUS");

                struct_bde_weight = rfc_function.GetStructure("IS_BDE_STATUS");

                struct_bde_weight.SetValue("WERKS", str_branchNo);
                struct_bde_weight.SetValue("VEHICLE", str_vehicle_EQ);

                rfc_function.Invoke(rfc_destination);

                irfcTable_returnTable = rfc_function.GetTable("ET_BRANCHES");

                EvaluateSapData(irfcTable_returnTable, xml_configFile, "ET_BRANCHES");
                irfcTable_returnTable = rfc_function.GetTable("ET_VEHICLES");
                EvaluateSapData(irfcTable_returnTable, xml_configFile, "ET_VEHICLES");
                irfcTable_returnTable = rfc_function.GetTable("ET_FACILITIES");
                EvaluateSapData(irfcTable_returnTable, xml_configFile, "ET_FACILITIES");
                irfcTable_returnTable = rfc_function.GetTable("ET_EMPLOYEES");
                EvaluateSapData(irfcTable_returnTable, xml_configFile, "ET_EMPLOYEES");
                irfcTable_returnTable = rfc_function.GetTable("ET_DOWNTIMES");
                EvaluateSapData(irfcTable_returnTable, xml_configFile, "ET_DOWNTIMES");
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        
        private void EvaluateSapData(IRfcTable irfc_returnTable, XmlDocument xml_doc, string str_tableName)
        {
            try
            {
                using (DataTable dt = new DataTable())
                {
                    RfcElementMetadata md, rm;

                    dt.TableName = irfc_returnTable.Metadata.Name;
                    for (int i = 0; i < irfc_returnTable.ElementCount; i++)
                    {
                        md = irfc_returnTable.GetElementMetadata(i);
                        dt.Columns.Add(md.Name);
                    }

                    foreach (IRfcStructure row in irfc_returnTable)
                    {
                        DataRow dr = dt.NewRow();

                        for (int element = 0; element < irfc_returnTable.ElementCount; element++)
                        {
                            rm = irfc_returnTable.GetElementMetadata(element);
                            dr[rm.Name] = row.GetString(rm.Name);
                        }
                        dt.Rows.Add(dr);
                    }
                    if (dt.Rows.Count > 0)
                    {                        
                        dt.WriteXml(xml_doc.SelectSingleNode(@"BDE.Configuration/General/SapTablesPath").Attributes[@"value"].Value + str_tableName + @".xml");
                    }
                }
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void DeleteLOGs(int int_daysHoldBack)
        {
            try
            {
                string[] stra_files = Directory.GetFiles(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\LOGS\");

                foreach (string file in stra_files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime <= DateTime.Now.AddDays(int_daysHoldBack))
                    {
                        fi.Delete();
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
        public void Feedback(Exception exc)
        {
            Feedback fb = new Feedback();

            fb.FeedbackHandler(exc);
        }
        #endregion Feedback        
    }    
}
