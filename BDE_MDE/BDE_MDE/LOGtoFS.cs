using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDE_MDE
{
    class LOGtoFS
    {
        public static void CreateTxtFile(string str_fullpath)
        {
            try
            {                
                if (!File.Exists(str_fullpath))
                {
                    using (StreamWriter sw = File.CreateText(str_fullpath))
                    {
                        sw.WriteLine("Start Logging: " + DateTime.Today.ToShortDateString());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void WriteLog(string str_fullpath, string text)
        {
            try
            {                
                using (StreamWriter sw = File.AppendText(str_fullpath))
                {
                    sw.WriteLine(text);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
