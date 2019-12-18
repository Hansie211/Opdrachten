using System;
using System.Collections.Generic;
using System.Globalization;
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

        const int linesize = 50;

        public static readonly CultureInfo CurrencyFormatProvider = CultureInfo.GetCultureInfo( "nl-NL" );

        private static readonly string l_addition   = "+".PadLeft(linesize) +  "\n";
        private static readonly string l_seperate   = "-----".PadLeft(linesize) + "\n";
        private static readonly string l_empty      = new string(' ', linesize) + "\n";


        public double[] vals;
        public double percent = 0;
        public string percentDesc = "";

        private Run content = new Run();

        private List<string> chunkSplit( string text, int size ) {

            if ( text.Length == 0 ) { // return empty list
                return new List<string>( new string[] { "" } );
            }

            string[] words = text.Split(null);

            List<string> result = new List<string>();

            string line = "*";
            for( int i = 0; i < words.Length; i++ ) {

                int currentlen = line.Length;
                int nextlen = currentlen + words[i].Length + 1;

                if ( nextlen <= size ) {

                    line += $" {words[ i ]}";
                } else {

                    result.Add( line );
                    line = words[i];
                }
            }

            if ( line.Length > 0 ) { 
                // Add the last line
                result.Add( line );
            }

            //int len = text.Length;
            //int pos;

            //for ( pos = 0; pos + size < len; pos += size ) {

            //    result.Add( text.Substring( pos, size ) + "-" );
            //}

            //if ( pos < text.Length ) {

            //    result.Add( text.Substring( pos, text.Length - pos ) );
            //}

            return result;
        }

        public Run addLine( string text ) {
            Run line = new Run( text );
            memo.Inlines.Add( line );

            return line;
        }

        public string makeLine( string desc, string val ) {

            const int whitespace = 15;

            List<string> chunks = chunkSplit( desc, linesize - whitespace);

            string result = String.Format("{0}{1}\n", chunks[0].PadRight(linesize - whitespace), val.PadLeft(whitespace) );
            for ( int i = 1; i < chunks.Count; i++ ) {

                result += $"{chunks[ i ]}\n";
            }

            return result;
        }

        public string currToStr( double v ) {

            v = Math.Round( v, 2 );

            return '€' + v.ToString( "N", CurrencyFormatProvider );
        }

        public double toCurrency( double val ) {
            return Math.Round( val, 2 );
        }

        public void setCharge( string desc, double val ) {
            percent     = val;
            percentDesc = desc;
        }

        public void addTextToReceipt( string text ) {

            List<string> chunks = chunkSplit( text, linesize - 10 );
            addLine( String.Join( "\n", chunks.ToList() ) + "\n" );
        }

        public void addToReceipt( string desc, int times, double euroval ) {

            addToReceipt( desc, (double)times, euroval );
        }

        public void addToReceipt( string desc, double times, double euroval, bool forceShow = false ) {

            if (( times == 0 ) && ( !forceShow )) {

                return;
            }

            // Force round to 2 decimals, it would look a bit off if 2.50 + 2.50 would equal 4.99
            euroval = toCurrency( euroval * times );

            // Push the val to the array
            int i = vals.Length;
            Array.Resize( ref vals, i + 1 );
            vals[ i ] = euroval;

            if ( times != 1 ) {
                desc = String.Format( "{0} x{1:0.##}", desc, times );
            }

            addLine( makeLine( desc, currToStr( euroval ) ) );
        }

        public double getSum() {

            double sum = 0;
            for ( int i = 0; i < vals.Length; i++ ) {
                sum += vals[ i ];
            }

            return sum;
        }

        public void displayReceipt() {
            displayReceipt( getSum() );
        }

        public void displayReceipt( double sum ) {

            if ( percent != 0 ) {
                double charge = toCurrency( sum * percent );

                string s;
                if ( charge > 0 ) {
                    s = String.Format( "+{0:0.00#}% ({1})", percent, percentDesc );
                } else {
                    s = String.Format( "{0:0.00#}% ({1})", percent, percentDesc );
                }

                addLine( l_empty + makeLine( s, currToStr( charge ) ) );
                sum += charge;
            }

            addLine( l_empty + l_addition + l_seperate );
            Run sumLine = addLine( currToStr( sum ).PadLeft( linesize ) );

            sumLine.FontWeight  = FontWeights.Bold;

            ShowDialog();
        }

        public WindowReceipt( string desc ) {
            InitializeComponent();

            // Init the array
            vals = new double[ 0 ];
            memo.Inlines.Clear();

            // Set the title
            Run title = addLine( desc + "\n\n");

            title.FontSize      = 18;
            title.FontFamily    = this.FontFamily;
            title.FontWeight    = FontWeights.Bold;
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {
            Close();
        }
    }
}
