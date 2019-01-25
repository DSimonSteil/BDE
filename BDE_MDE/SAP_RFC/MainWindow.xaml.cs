using SAP.Middleware.Connector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

namespace SAP_RFC
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";

                XmlDocument xml_configFile = new XmlDocument();
                xml_configFile.Load(str_configFilePath);

                string str_vehicle_EQ = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Vehicle_EQ").Attributes[@"value"].Value;
                string str_branchNo = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch_NL").Attributes[@"value"].Value;

                RfcDestination rfc_destination = RfcDestinationManager.GetDestination("mi38");
                RfcRepository rfc_repository = rfc_destination.Repository;

                IRfcFunction rfc_function = rfc_repository.CreateFunction("Z_PP_BDE_STATUS");

                //IRfcStructure struct_bde_weight = rfc_function.GetStructure("IS_BDE_STATUS");

                //struct_bde_weight.SetValue("WERKS", str_branchNo);
                //struct_bde_weight.SetValue("VEHICLE", str_vehicle_EQ);

                rfc_function.Invoke(rfc_destination);

                //IRfcTable returnTable;
                
                IRfcTable irfcTable_returnTable = rfc_function.GetTable("ET_BRANCHES");

                EvaluateSapData(irfcTable_returnTable, xml_configFile, "ET_BRANCHES");
                irfcTable_returnTable = rfc_function.GetTable("ET_VEHICLES");
                EvaluateSapData(irfcTable_returnTable, xml_configFile, "ET_VEHICLES");
                irfcTable_returnTable = rfc_function.GetTable("ET_FACILITIES");
                EvaluateSapData(irfcTable_returnTable, xml_configFile, "ET_FACILITIES");
                irfcTable_returnTable = rfc_function.GetTable("ET_EMPLOYEES");
                EvaluateSapData(irfcTable_returnTable, xml_configFile, "ET_EMPLOYEES");


                // Loop jede Structur in ReturnTable??
                //foreach (IRfcTable t in irfcTable_returnTable)
                //{

                //}

                //DataTable dt = new DataTable();

                //for (int i = 0; i < irfcTable_returnTable.ElementCount; i++)
                //{
                //    RfcElementMetadata md = irfcTable_returnTable.GetElementMetadata(i);
                //    dt.Columns.Add(md.Name);
                //}

                //foreach (IRfcStructure row in irfcTable_returnTable)
                //{
                //    DataRow dr = dt.NewRow();

                //    for (int element = 0; element < irfcTable_returnTable.ElementCount; element++)
                //    {
                //        RfcElementMetadata rm = irfcTable_returnTable.GetElementMetadata(element);
                //        dr[rm.Name] = row.GetString(rm.Name);
                //    }
                //    dt.Rows.Add(dr);
                //}                                
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
                    dt.TableName = irfc_returnTable.Metadata.Name;
                    for (int i = 0; i < irfc_returnTable.ElementCount; i++)
                    {
                        RfcElementMetadata md = irfc_returnTable.GetElementMetadata(i);
                        dt.Columns.Add(md.Name);
                    }

                    foreach (IRfcStructure row in irfc_returnTable)
                    {
                        DataRow dr = dt.NewRow();

                        for (int element = 0; element < irfc_returnTable.ElementCount; element++)
                        {
                            RfcElementMetadata rm = irfc_returnTable.GetElementMetadata(element);
                            dr[rm.Name] = row.GetString(rm.Name);
                        }
                        dt.Rows.Add(dr);
                    }
                    if (dt.Rows.Count > 0)
                    {                        
                        dt.WriteXml(@"C:\temp\" + str_tableName + @".xml");
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
            tbx_feedback.Text += exc.GetType().ToString() + @" @ " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name + System.Environment.NewLine + exc.Message + System.Environment.NewLine + System.Environment.NewLine;
        }
    }
}
