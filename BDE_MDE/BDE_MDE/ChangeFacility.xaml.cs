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
    /// Interaction logic for ChangeFacility.xaml
    /// </summary>
    public partial class ChangeFacility : Page
    {
        public ChangeFacility()
        {
            InitializeComponent();            
        }

        private void btn_birdview_Click(object sender, RoutedEventArgs e)
        {
            ViewArea va = new ViewArea();
            this.NavigationService.Navigate(va);
        }

        private void btn_kondirator_Click(object sender, RoutedEventArgs e)
        {
            Facilities fac = new Facilities(btn_kondirator.Content.ToString());
            this.NavigationService.Navigate(fac);
        }
    }
}
