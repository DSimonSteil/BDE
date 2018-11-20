using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

namespace CSVtoDatatable
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (DataTable dt = new DataTable())
                using (StreamReader sr = new StreamReader(tbx_path.Text))
                {
                    string[] headers = sr.ReadLine().Split(';');
                    foreach (string header in headers)
                    {
                        dt.Columns.Add(header);
                    }
                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(';');
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i];
                        }
                        dt.Rows.Add(dr);
                    }
                    tbx_feedback.Focus();
                    string str_key = "Ztauswnr = " + tbx_key.Text;
                    DataRow[] foundRows;
                    
                    foundRows = dt.Select(str_key);

                    tbx_feedback.Text = foundRows[0][@"Name Mitarb./Bewerb."].ToString();

                }
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
