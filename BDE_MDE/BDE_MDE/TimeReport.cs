using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BDE_MDE
{
    class TimeReport
    {
        #region Variables
        private static string str_reportPath = String.Empty;
        private static string str_reportFileName = String.Empty;
        private static string str_branchNo = String.Empty;
        private static string str_vehicle_EQ = String.Empty;        

        private static RfcDestination dest;
        private static RfcRepository rep;
        private static IRfcFunction rfc_read_table;
        private static IRfcStructure struct_bde_time;
        private static XmlDocument xml_doc;
        #endregion

        public static void CreateReport(string str_status, string str_actFacility, string str_actBox)
        {
            try
            {
                xml_doc = new XmlDocument();
                xml_doc.Load(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml");
                str_reportPath = xml_doc.SelectSingleNode(@"BDE.Configuration/General/TimeReportPath").Attributes[@"value"].Value;
                str_reportFileName = str_reportPath + ConfigClass.strEmpNo + @"_" + DateTime.Now.ToString("yyyyMMdd") + "_" + DateTime.Now.ToString("HHmmss") + @".xml";
                
                if (CheckConnection.PingSAP(xml_doc.SelectSingleNode(@"BDE.Configuration/General/SAP_IP").Attributes[@"value"].Value, @"CreateReport"))
                {
                    dest = RfcDestinationManager.GetDestination(xml_doc.SelectSingleNode(@"BDE.Configuration/General/SAP_Connection").Attributes[@"value"].Value);
                    rep = dest.Repository;

                    rfc_read_table = rep.CreateFunction("Z_PP_BDE_TIME");

                    struct_bde_time = rfc_read_table.GetStructure("IS_BDE_TIME");

                    struct_bde_time.SetValue("WERKS", xml_doc.SelectSingleNode(@"BDE.Configuration/General/Branch_NL").Attributes[@"value"].Value);
                    struct_bde_time.SetValue("PERNR", ConfigClass.strEmpNo);
                    struct_bde_time.SetValue("VEHICLE", xml_doc.SelectSingleNode(@"BDE.Configuration/General/Vehicle_EQ").Attributes[@"value"].Value);
                    struct_bde_time.SetValue("ANLAGE", str_actFacility);
                    struct_bde_time.SetValue("BOX", str_actBox);
                    struct_bde_time.SetValue("UZEIT", DateTime.Now.ToString("HHmmss"));
                    struct_bde_time.SetValue("DATUM", DateTime.Now.ToString("yyyyMMdd"));
                    struct_bde_time.SetValue("STATUS", str_status);

                    rfc_read_table.Invoke(dest);

                    var returnState = rfc_read_table.GetValue("EF_STATE");

                    if (returnState.Equals("X"))
                    {
                    }
                }
                else
                {
                    if (!File.Exists(str_reportFileName))
                    {
                        using (XmlWriter writer = XmlWriter.Create(str_reportFileName))
                        {
                            writer.WriteStartElement("TimeReport");
                            writer.WriteElementString("WERKS", xml_doc.SelectSingleNode(@"BDE.Configuration/General/Branch_NL").Attributes[@"value"].Value);
                            writer.WriteElementString("PERNR", ConfigClass.strEmpNo);
                            writer.WriteElementString("VEHICLE", xml_doc.SelectSingleNode(@"BDE.Configuration/General/Vehicle_EQ").Attributes[@"value"].Value);
                            writer.WriteElementString("ANLAGE", str_actFacility);
                            writer.WriteElementString("BOX", str_actBox);
                            writer.WriteElementString("DATUM", DateTime.Now.ToString("yyyyMMdd"));
                            writer.WriteElementString("UZEIT", DateTime.Now.ToString("HHmmss"));
                            writer.WriteElementString("STATUS", str_status);
                        }
                    }
                }                                
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        public static void EnterFacility(XmlDocument xml_doc, string str_actualFacility)
        {
            try
            {
                str_reportPath = xml_doc.SelectSingleNode(@"BDE.Configuration/General/TimeReportPath").Attributes[@"value"].Value;
                str_reportFileName = str_reportPath + ConfigClass.strEmpNo + @"_" + DateTime.Now.ToString("yyyyMMdd") + @".xml";

                if (File.Exists(str_reportFileName))
                {
                    
                    //XmlDocument xml_actualFile = new XmlDocument();
                    //xml_actualFile.Load(str_reportFileName);
                    //xml_actualFile.

                    //XmlElement e = xml_actualFile.CreateElement(str_actualFacility);
                    //xml_actualFile.DocumentElement.AppendChild(e);

                    //XmlNode node = xml_actualFile.CreateElement(str_actualFacility);
                    //XmlElement elem = xml_actualFile.CreateElement("BEGIN");
                    //elem.InnerText = DateTime.Now.ToString("yyyyMMdd");

                    //node.AppendChild(elem);

                    //xml_actualFile.Save(str_reportFileName);
                }
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        public static void LogOff(XmlDocument xml_doc)
        {
            try
            {
                TimeReport.CreateReport("P20", "", "");                
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
