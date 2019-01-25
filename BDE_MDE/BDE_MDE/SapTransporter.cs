using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;
using System.IO;
using System.Net.NetworkInformation;
using SAP.Middleware.Connector;

namespace BDE_MDE
{
    class SapTransporter
    {
        #region Variables        
        private static XmlDocument xml_configFile;
        private static string str_configFilePath;
        private static string str_fileName;
        private static string str_sapIP;
        private static XmlDocument actual;
        private static string str_scalePath;

        private static RfcDestination dest;
        private static RfcRepository rep;
        private static IRfcStructure struct_bde_weight;        
        #endregion

        public static void CheckXMLs()
        {
            try
            {                
                Timer t = new Timer(TimeSpan.FromMinutes(1).TotalMilliseconds); // Set the time (5 mins in this case)
                t.AutoReset = true;
                t.Elapsed += new System.Timers.ElapsedEventHandler(Start);
                t.Start();                
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private static void Start(object sender, ElapsedEventArgs e)
        {            
            xml_configFile = new XmlDocument();
            str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
            str_fileName = String.Empty;
            xml_configFile.Load(str_configFilePath);
            str_sapIP = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/SAP_IP").Attributes[@"value"].Value;

            if (CheckConnection.PingSAP(str_sapIP, @"SapTransporter"))
            {
                actual = new XmlDocument();

                str_scalePath = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/ScalePath").Attributes[@"value"].Value;

                dest = RfcDestinationManager.GetDestination(xml_configFile.SelectSingleNode(@"BDE.Configuration/General/SAP_Connection").Attributes[@"value"].Value);
                rep = dest.Repository;
                var rfc_read_table = rep.CreateFunction("Z_PP_BDE_IMPORT");                

                foreach (string str_file in Directory.EnumerateFiles(str_scalePath, "*.xml"))
                {
                    CreateWeightXML(rfc_read_table, str_file);
                }
            }                                    
        }

        private static void CreateWeightXML(IRfcFunction rfc_read_table, string str_file)
        {
            try
            {
                actual.Load(str_file);
                struct_bde_weight = rfc_read_table.GetStructure("IS_BDE_WEIGHT");

                if (actual.SelectSingleNode(@"Scale/WDELETE").InnerText.Equals("X"))
                {
                    struct_bde_weight.SetValue("GUID", actual.SelectSingleNode(@"Scale/GUID").InnerText);
                    struct_bde_weight.SetValue("WDELETE", actual.SelectSingleNode(@"Scale/WDELETE").InnerText);
                }
                else
                {
                    struct_bde_weight.SetValue("WERKS", actual.SelectSingleNode(@"Scale/BranchNo").InnerText);
                    struct_bde_weight.SetValue("ANLAGE", actual.SelectSingleNode(@"Scale/Facility").InnerText);
                    struct_bde_weight.SetValue("VEHICLE", actual.SelectSingleNode(@"Scale/VehicleEQ").InnerText);
                    struct_bde_weight.SetValue("BOX", actual.SelectSingleNode(@"Scale/Box").InnerText);
                    struct_bde_weight.SetValue("GUID", actual.SelectSingleNode(@"Scale/GUID").InnerText);
                    struct_bde_weight.SetValue("PERNR", actual.SelectSingleNode(@"Scale/EmployeeNo").InnerText);
                    struct_bde_weight.SetValue("GEWICHT", actual.SelectSingleNode(@"Scale/Weight").InnerText);
                    struct_bde_weight.SetValue("MEINS", actual.SelectSingleNode(@"Scale/Einheit").InnerText);
                    struct_bde_weight.SetValue("DATUM", actual.SelectSingleNode(@"Scale/WeightDate").InnerText);
                    struct_bde_weight.SetValue("UZEIT", actual.SelectSingleNode(@"Scale/WeightTime").InnerText);
                    struct_bde_weight.SetValue("WDELETE", actual.SelectSingleNode(@"Scale/WDELETE").InnerText);
                }

                rfc_read_table.Invoke(dest);

                var irfc_returnState = rfc_read_table.GetValue("EF_STATE");

                if (irfc_returnState.Equals("X"))
                {
                    str_fileName = System.IO.Path.GetFileName(str_file);

                    File.Move(str_file, @"C:\BDE\scale\Archiv\" + str_fileName);
                }
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }                       
        }

        #region Feedback
        private static void Feedback(Exception exc)
        {
            Feedback fb = new Feedback();

            fb.FeedbackHandler(exc);
        }
        #endregion
    }
}
