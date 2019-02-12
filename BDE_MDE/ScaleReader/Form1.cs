using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ScaleReader
{
    public partial class Form1 : Form
    {
        private SerialPort sp_scaleListening;

        public Form1()
        {
            InitializeComponent();
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            try
            {
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
                sp_scaleListening = new SerialPort(tbx_comPort.Text);

                sp_scaleListening.BaudRate = 115200;
                sp_scaleListening.Parity = Parity.None;
                sp_scaleListening.StopBits = StopBits.One;
                sp_scaleListening.DataBits = 8;
                sp_scaleListening.Handshake = Handshake.None;

                sp_scaleListening.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                sp_scaleListening.Open();

                tbx_comPort.Text = "open" + System.Environment.NewLine;
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

                string[] stra_weight;

                SerialPort sp = sender as SerialPort;
                
                while (sp.BytesToRead > 0)
                {
                    str_scaleOutput = str_scaleOutput + (sp.ReadExisting());
                    System.Threading.Thread.Sleep(500);
                    tbx_result.Text = str_scaleOutput;
                }                

                stra_weight = str_scaleOutput.Split(';');

                

                //Dispatcher.Invoke(new Action(delegate ()
                //{
                //    try
                //    {
                        
                //        //tbx_result.Text = stra_weight[8].Replace("\r\n", String.Empty) + @"t";//+ System.Convert.ToString(charToRead);                        
                //    }
                //    catch (Exception exc)
                //    {
                //        Feedback(exc);
                //    }
                //}));


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
