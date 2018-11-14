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

namespace BDE_MDE
{
    /// <summary>
    /// Interaction logic for Scale.xaml
    /// </summary>
    public partial class Scale : Page
    {
        #region Variables
        int int_box;
        #endregion
        public Scale(string str_actualBox, string str_actualFacility)
        {
            try
            {
                InitializeComponent();

                tbx_actualBox.Text = @"Box: " + str_actualBox;
                tbx_actualFacility.Text = str_actualFacility;
                int_box = Convert.ToInt32(str_actualBox);
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

        private void btn_scale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch (Exception exc)
            {
                Feedback(exc);
            }
        }        
    }                
}
