using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;

namespace BDE_MDE
{
    public class PortLogger
    {
        XmlDocument xml_configFile;
        private SerialPort sp_scaleListening;
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        int int_sleepTime;

        public void StartLog()
        {
            try
            {
                xml_configFile = new XmlDocument();
                xml_configFile.Load(str_configFilePath);
                string str_serialPort = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Serial-Port").Attributes[@"value"].Value;
                string str_baudRate = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/Baudrate").Attributes[@"value"].Value;
                string str_dataBits = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/DataBits").Attributes[@"value"].Value;

                int_sleepTime = Convert.ToInt32(xml_configFile.SelectSingleNode(@"BDE.Configuration/General/SleepScale").Attributes[@"value"].Value);

                sp_scaleListening = new SerialPort(str_serialPort);

                sp_scaleListening.BaudRate = Convert.ToInt32(str_baudRate);
                sp_scaleListening.Parity = Parity.None;
                sp_scaleListening.StopBits = StopBits.One;
                sp_scaleListening.DataBits = Convert.ToInt32(str_dataBits);
                sp_scaleListening.Handshake = Handshake.None;

                sp_scaleListening.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                sp_scaleListening.Open();
                LOGtoFS.CreateTxtFile(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\PortLogger\" + DateTime.Today.ToShortDateString() + @"_Log.txt");
                LOGtoFS.WriteLog(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\PortLogger\" + DateTime.Today.ToShortDateString() + "_Log.txt", "Logging gestartet" + System.Environment.NewLine);
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string str_scaleOutput = String.Empty;
                
                SerialPort sp = sender as SerialPort;               

                while (sp.BytesToRead > 0)
                {
                    str_scaleOutput = str_scaleOutput + (sp.ReadExisting());
                    System.Threading.Thread.Sleep(int_sleepTime);
                }

                LOGtoFS.CreateTxtFile(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\PortLogger\" + DateTime.Today.ToShortDateString() + @"_Log.txt");
                LOGtoFS.WriteLog(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\PortLogger\" + DateTime.Today.ToShortDateString() + "_Log.txt", str_scaleOutput + System.Environment.NewLine); 


            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        #region Feedback
        private void Feedback(Exception exc)
        {
            Feedback fb = new Feedback();

            fb.FeedbackHandler(exc);
        }
        #endregion

    }
}
