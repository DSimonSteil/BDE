using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BDE_MDE
{
    public static class CheckConnection
    {
        private static PingReply reply;

        public static bool PingSAP(string str_ip, string str_sourceClass)
        {
            try
            {
                bool pingable = false;
                Ping pinger = null;

                try
                {
                    pinger = new Ping();
                    reply = pinger.Send(str_ip, 1000);
                    pingable = reply.Status == IPStatus.Success;              
                }
                catch (PingException)
                {
                    
                }
                finally
                {
                    if (pinger != null)
                    {
                        pinger.Dispose();
                    }
                }
                if (pingable)
                {
                    
                    LOGtoFS.CreateTxtFile(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\LOGS\" + DateTime.Today.ToShortDateString() + @"_SapConnection.txt");
                    LOGtoFS.WriteLog(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\LOGS\" + DateTime.Today.ToShortDateString() + "_SapConnection.txt", DateTime.Now + " → " + str_sourceClass + ": SAP erreichbar. Antwortzeit: " + reply.RoundtripTime);
                }
                else
                {
                    LOGtoFS.CreateTxtFile(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\LOGS\" + DateTime.Today.ToShortDateString() + @"_SapPing.txt");
                    LOGtoFS.WriteLog(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\LOGS\" + DateTime.Today.ToShortDateString() + "_SapPing.txt", DateTime.Now + " → " + str_sourceClass + ": SAP NICHT erreichbar.");
                }                
                return pingable;
            }
            catch (Exception)
            {
                throw;
            }            
        }         
    }

}
