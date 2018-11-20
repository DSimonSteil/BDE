using System;
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
using System.IO.Ports;
using System.Xml;
using System.Net.Mail;

namespace BDE_MDE
{    
    public partial class Scale : Page
    {
        #region Variables        
        private Facilities fc;
        private SerialPort sp_scaleListening;
        private string str_configFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
        private XmlDocument xml_configFile;
        private string str_xmlDestinationPath;
        private int int_sleepTime = 0;
        private MainWindow mw;

        //private string str_branch;
        //private string str_from;
        //private string str_to;
        //private string str_sign;
        //private string str_mxsHost;
        #endregion

        #region Constructor
        public Scale(Facilities facility, string str_actualBox, string str_actualFacility)
        {
            try
            {
                InitializeComponent();

                tbx_actualBox.Text = @"Box: " + str_actualBox;
                tbx_actualFacility.Text = str_actualFacility;                
                fc = facility;
                ScaleListener();

                mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mw.btn_facilities.IsEnabled = false;

            }
            catch (Exception exc)
            {
                Feedback(exc);
            }            
        }
        #endregion

        #region DataListener/DataReceiver
        private void ScaleListener()
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
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void DataReceivedHandler(object sender,SerialDataReceivedEventArgs e)
        {
            try
            {
                string str_scaleOutput = String.Empty;

                string[] stra_weight;

                SerialPort sp = sender as SerialPort;

                Dispatcher.Invoke(new Action(delegate ()
                {
                    tbx_actualWeight.Text = String.Empty;                    
                }));                                

                while (sp.BytesToRead > 0)
                {
                    str_scaleOutput = str_scaleOutput + (sp.ReadExisting());
                    System.Threading.Thread.Sleep(int_sleepTime);
                }

                stra_weight = str_scaleOutput.Split(';');

                Dispatcher.Invoke(new Action(delegate ()
                {
                    try
                    {
                        tbx_actualWeight.Text = stra_weight[8].Replace("\r\n", String.Empty) + @"kg";//+ System.Convert.ToString(charToRead);
                        btn_scale.IsEnabled = true;
                    }
                    catch (Exception exc)
                    {
                        Feedback(exc);
                    }                    
                }));


            }
            catch (Exception exc)
            {
                Feedback(exc);
            }                                    
        }
        #endregion

        #region Controls
        private void Btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sp_scaleListening.Close();
                mw.btn_facilities.IsEnabled = true;
                NavigationService.Navigate(fc);
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void btn_scale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ScaleProcess sp = new ScaleProcess(this, tbx_actualFacility.Text, tbx_actualBox.Text, tbx_actualWeight.Text);
                this.NavigationService.Navigate(sp);
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }

        private void Btn_deleteLastScale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Letzte Wiegung wirklich löschen?", "Lösch-Bestätigung", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    CreateDeleteXML();
                    btn_deleteLastScale.IsEnabled = false;
                    tbx_lastScale.Text = @"erfolgreich gelöscht";
                }
                else { }
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }
        #endregion

        #region Help Methods
        private void CreateDeleteXML()
        {
            try
            {                            
                str_xmlDestinationPath = xml_configFile.SelectSingleNode(@"BDE.Configuration/General/ScalePath").Attributes[@"value"].Value;
                                
                using (XmlWriter writer = XmlWriter.Create(str_xmlDestinationPath + "/ToDelete_" + DateTime.Now.Ticks + ".xml"))
                {                                        
                    writer.WriteStartElement("ToDelete");
                    writer.WriteElementString("GUID", lbl_lastWeightGuid.Content.ToString());
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
