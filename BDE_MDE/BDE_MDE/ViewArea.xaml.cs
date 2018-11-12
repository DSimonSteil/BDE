using System;
using System.Collections.Generic;
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

namespace BDE_MDE
{
    /// <summary>
    /// Interaction logic for ViewArea.xaml
    /// </summary>
    public partial class ViewArea : Page
    {
        public ViewArea()
        {
            InitializeComponent();
            CreateButtons();
        }

        private void btn_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                
                FaciltyKondirator fk = new FaciltyKondirator(btn.Name);
                this.NavigationService.Navigate(fk);
            }
            catch (Exception exc)
            {
                MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
                mw.Feedback(exc);
            }
        }

        private void CreateButtons()
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();                
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\bdeConfig.xml";
                string str_values;
                string[] stra_coords;

                xmlDocument.Load(path);
                XmlNodeList xnList = xmlDocument.SelectNodes(@"BDE.Configuration/Trier/platz_luftbild");

                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFB0D99D"));

                foreach (XmlNode xn1 in xnList)
                {
                    foreach (XmlNode xn2 in xn1)
                    {
                        str_values = xn2.FirstChild.Attributes[@"value"].Value;
                        stra_coords = str_values.Split(';');

                        Button btn = new Button()
                        {
                            BorderBrush = System.Windows.Media.Brushes.White,
                            BorderThickness = new Thickness(2),
                            Name = xn2.Name,
                            Content = xn2.Name,
                            Background = System.Windows.Media.Brushes.Transparent,                            
                            Margin = new Thickness(Convert.ToDouble(stra_coords[0]), Convert.ToDouble(stra_coords[1]), 0, 0),
                            Foreground = mySolidColorBrush,
                            FontWeight = FontWeights.Bold,
                            FontSize = 25,
                            Width = Convert.ToDouble(stra_coords[2]),
                            Height = Convert.ToDouble(stra_coords[3]),                            
                        };                                                                          
                        AreaGrid.Children.Add(btn);

                        btn.Click += new RoutedEventHandler(btn_button_Click);                        
                    }                    
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.GetType().ToString() + @" @ " + new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name
                + System.Environment.NewLine + exc.Message + System.Environment.NewLine + System.Environment.NewLine);
            }            
        }
        
    }
}
