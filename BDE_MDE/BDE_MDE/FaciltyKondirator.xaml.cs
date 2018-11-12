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

namespace BDE_MDE
{
    /// <summary>
    /// Interaction logic for FaciltyKondirator.xaml
    /// </summary>
    public partial class FaciltyKondirator : Page
    {
        public FaciltyKondirator(string str_machineName)
        {
            InitializeComponent();
            MainWindow mw = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            mw.tbx_facility.Text = @"Aktuelle Maschine: " + System.Environment.NewLine + @"Kondirator";

            MessageBox.Show(str_machineName);
        }
    }
}
