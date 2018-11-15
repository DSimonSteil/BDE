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

namespace BDE_MDE
{
    /// <summary>
    /// Interaction logic for Scale.xaml
    /// </summary>
    public partial class Scale : Page
    {
        #region Variables
        
        private Facilities fc;
        private SerialPort sp_scaleListening;
        #endregion
        public Scale(Facilities facility, string str_actualBox, string str_actualFacility)
        {
            try
            {
                InitializeComponent();

                tbx_actualBox.Text = @"Box: " + str_actualBox;
                tbx_actualFacility.Text = str_actualFacility;                
                fc = facility;
                ScaleListener();

            }
            catch (Exception exc)
            {
                Feedback(exc);
            }            
        }        

        private void ScaleListener()
        {
            try
            {
                sp_scaleListening = new SerialPort("COM3");

                sp_scaleListening.BaudRate = 115200;
                sp_scaleListening.Parity = Parity.None;
                sp_scaleListening.StopBits = StopBits.One;
                sp_scaleListening.DataBits = 8;
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
                Dispatcher.Invoke(new Action(delegate ()
                {
                    tbx_actualWeight.Text = String.Empty;                    
                }));

                string str_scaleOutput = String.Empty;

                string[] stra_weight;

                SerialPort sp = sender as SerialPort;
                

                //string str_weight = sp.ReadExisting().Replace("\r\n", string.Empty) + " kg";


                while (sp.BytesToRead > 0)
                {
                    str_scaleOutput = str_scaleOutput + (sp.ReadExisting());
                    System.Threading.Thread.Sleep(500);
                }

                stra_weight = str_scaleOutput.Split(';');

                Dispatcher.Invoke(new Action(delegate ()
                {
                    tbx_actualWeight.Text = tbx_actualWeight.Text + (stra_weight[8]);//+ System.Convert.ToString(charToRead);
                    btn_scale.IsEnabled = true;
                }));


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
                sp_scaleListening.Close();
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


        private void Feedback(Exception exc)
        {
            MessageBox.Show(exc.GetType().ToString() + @" @ " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name
               + System.Environment.NewLine + exc.Message + System.Environment.NewLine + System.Environment.NewLine);
        }                        
    }                
}
