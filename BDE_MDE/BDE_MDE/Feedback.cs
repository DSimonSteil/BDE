using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Net.NetworkInformation;

namespace BDE_MDE
{
    public class Feedback
    {
        #region Variables
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private string str_branch = String.Empty;
        private string str_sign = String.Empty;
        private string str_name = String.Empty;
        private string str_from = String.Empty;
        private string str_to = String.Empty;
        private string str_mxsHost = String.Empty;
        #endregion
        public void FeedbackHandler(Exception exc)
        {
            MessageBox.Show(exc.GetType().ToString() + @" @ " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name
              + System.Environment.NewLine + exc.Message + System.Environment.NewLine + System.Environment.NewLine + "Die EDV wird durch eine automatisch generierte E-Mail informiert.");

            GetXmlInfos();

            string str_hostName = System.Net.Dns.GetHostName();
            System.Net.IPHostEntry hostInfo = System.Net.Dns.GetHostEntry(str_hostName);
            string str_ipAdress = hostInfo.AddressList[1].ToString(); 

            MailMessage mail = new MailMessage(str_from, str_to);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = str_mxsHost;
            mail.Subject = "Fehler in Betriebsdatenerfassung";
            mail.Body = "Folgender Fehler ist aufgetreten: " + System.Environment.NewLine + System.Environment.NewLine + "Standort: "
                        + str_branch + System.Environment.NewLine + "Fahrzeug: " + str_sign + " (" + str_name + ")" + System.Environment.NewLine + 
                          @"Fahrer: " + ConfigClass.strEmpName + System.Environment.NewLine + @"IP: " + str_ipAdress + System.Environment.NewLine + System.Environment.NewLine + exc.GetType().ToString() + @" @ " + exc.StackTrace
              + System.Environment.NewLine + exc.Message;

            using (Ping myPing = new Ping())
            {
                try
                {
                    PingReply reply = myPing.Send(str_mxsHost, 1000);                    
                    if (reply != null)
                    {
                        client.Send(mail);
                        LOGtoFS.CreateTxtFile(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\LOGS\" + DateTime.Today.ToShortDateString() + @"_Log.txt");
                        LOGtoFS.WriteLog(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\LOGS\" + DateTime.Today.ToShortDateString() + "_Log.txt", exc.GetType().ToString() + @" @ " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name + System.Environment.NewLine + exc.Message + System.Environment.NewLine + System.Environment.NewLine);
                    }                    
                }
                catch
                {
                    LOGtoFS.CreateTxtFile(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\LOGS\" + DateTime.Today.ToShortDateString() + @"_Log.txt");
                    LOGtoFS.WriteLog(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\LOGS\" + DateTime.Today.ToShortDateString() + "_Log.txt", exc.GetType().ToString() + @" @ " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name + System.Environment.NewLine + exc.Message + System.Environment.NewLine + System.Environment.NewLine);
                }
            }            
        }
        private void GetXmlInfos()
        {
            XmlDocument xml_configFile = new XmlDocument();
            xml_configFile.Load(str_configFilePath);
            str_branch = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch").Attributes[@"value"].Value;
            str_sign = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Vehicle_EQ").Attributes[@"value"].Value;
            str_name = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Vehicle").Attributes[@"value"].Value;
            str_from = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/ErrorMailSender").Attributes[@"value"].Value;
            str_to = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/ErrorMailReceiver").Attributes[@"value"].Value;
            str_mxsHost = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/MsxHost").Attributes[@"value"].Value;
        }
        
    }
}
