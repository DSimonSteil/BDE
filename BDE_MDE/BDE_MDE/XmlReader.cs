using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace BDE_MDE
{    
    public class XmlReader
    {
        public void ReadXmlParameters()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();

                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
                string str_values;

                xmlDocument.Load(path);
                XmlNodeList xnList = xmlDocument.SelectNodes(@"BDE.Configuration/Trier/platz_luftbild");

                foreach (XmlNode xn1 in xnList)
                {
                    foreach (XmlNode xn2 in xn1)
                    {
                        str_values = xn2.FirstChild.Attributes[@"value"].Value;
                    }                                    
                }

                string test = xmlDocument.SelectSingleNode(@"BDE.Configuration/Trier/platz_luftbild/kondirator/coords").Attributes[@"value"].Value;
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.GetType().ToString() + @" @ " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name
                + System.Environment.NewLine + exc.Message + System.Environment.NewLine + System.Environment.NewLine);                
            }
            
        }

    }    
}
