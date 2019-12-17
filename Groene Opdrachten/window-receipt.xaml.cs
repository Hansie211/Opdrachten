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
using System.Windows.Shapes;

namespace GroeneOpdrachten {
    /// <summary>
    /// Interaction logic for window_receipt.xaml
    /// </summary>
    public partial class WindowReceipt : Window {

        double[] vals;
        double percent = 0;
        string percentDesc = "";

        const int linesize = 50;

        public string makeLine( string desc, string val ) {

            const int linecount = linesize - 1; // -'€'
            const int sublen    = linecount - 10;

            string result = "";
            int c = 0;

            while( (c = desc.Length + val.Length) + 5 > linecount ) {

                result += desc.Substring(0, sublen ) + "\n";
                desc = desc.Substring( sublen );
            }

            return result + desc + new string( ' ', linecount - c ) + '€' + val + "\n";
        }

        public string currToStr( double v ) {
            
            if ( v % 1 == 0 ) { // whole number?

                return v.ToString("0") + ",--";
            }

            // Always show 2 decimals (0,5 -> 0,50) and use commas, not dots
            return v.ToString("0.00").Replace('.', ',');
        }

        public double toCurrency( double val ) {
            return Math.Round( val, 2 );
        }

        public void setCharge( string desc, double val ) {
            percent     = val;
            percentDesc = desc;
        } 

        public void addToReceipt( string desc, int times, double euroval ) {

            addToReceipt( desc, (double)times, euroval);
        }

        public void addToReceipt( string desc, double times, double euroval ) {

            if ( times == 0 ) {

                return;
            }

            // It would look a bit off if 2.50 + 2.50 would equal 4.99
            euroval = toCurrency( euroval * times );

            // Push the val to the array
            int i = vals.Length;
            Array.Resize( ref vals, i + 1);
            vals[i] = euroval;

            if ( times > 1 ) {
                desc = String.Format( "{0} x{1:0.##}", desc, times);
            }

            memo.Text += makeLine( desc, currToStr( euroval ) );
        }

        public double getSum() {

            double sum = 0;
            for ( int i = 0; i < vals.Length; i++ ) {
                sum += vals[i];
            }

            return sum;
        }

        public void displayReceipt() {
            displayReceipt( getSum() );
        }

        public void displayReceipt( double sum ) {

            const string l_addition = "                                                 +\n";
            const string l_seperate = "                                             -----\n";
            const string l_empty    = "                                                  \n";

            if ( percent != 0 ) {
                double charge = toCurrency( sum * percent );

                string s;
                if ( charge > 0 ) { 
                    s = String.Format("+{0:0.00#}% ({1})", percent, percentDesc);
                } else {
                    s = String.Format("{0:0.00#}% ({1})", percent, percentDesc);
                }
                
                memo.Text += l_empty + makeLine( s, currToStr( charge ) );

                sum += charge;
            }                 

            memo.Text += l_empty + l_addition + l_seperate + makeLine("", currToStr(sum));

            ShowDialog();
        }

        public WindowReceipt(string desc) {
            InitializeComponent();

            // Init the array
            vals = new double[0];

            memo.Text = desc + "\n";
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {
            Close();
        }
    }
}
