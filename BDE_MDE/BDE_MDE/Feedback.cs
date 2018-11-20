﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace BDE_MDE
{
    public class Feedback
    {
        #region Variables
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private string str_branch = String.Empty;
        private string str_sign = String.Empty;
        private string str_from = String.Empty;
        private string str_to = String.Empty;
        private string str_mxsHost = String.Empty;
        #endregion
        public void FeedbackHandler(Exception exc)
        {
            MessageBox.Show(exc.GetType().ToString() + @" @ " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name
              + System.Environment.NewLine + exc.Message + System.Environment.NewLine + System.Environment.NewLine + "Die EDV wird durch eine automatisch generierte E-Mail informiert.");

            GetXmlInfos();

            MailMessage mail = new MailMessage(str_from, str_to);
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = str_mxsHost;
            mail.Subject = "Fehler in Betriebsdatenerfassung";
            mail.Body = "Folgender Fehler ist aufgetreten: " + System.Environment.NewLine + System.Environment.NewLine + "Standort: "
                        + str_branch + System.Environment.NewLine + "Fahrzeug: " + str_sign + System.Environment.NewLine + System.Environment.NewLine + exc.GetType().ToString() + @" @ " + exc.StackTrace
              + System.Environment.NewLine + exc.Message;
            
            client.Send(mail);
        }
        private void GetXmlInfos()
        {
            XmlDocument xml_configFile = new XmlDocument();
            xml_configFile.Load(str_configFilePath);
            str_branch = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Branch").Attributes[@"value"].Value;
            str_sign = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Sign").Attributes[@"value"].Value;
            str_from = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/ErrorMailSender").Attributes[@"value"].Value;
            str_to = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/ErrorMailReceiver").Attributes[@"value"].Value;
            str_mxsHost = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/MsxHost").Attributes[@"value"].Value;
        }
        
    }
}