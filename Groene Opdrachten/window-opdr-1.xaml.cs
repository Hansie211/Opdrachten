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
    /// Interaction logic for Window1.xaml
    /// </summary>
    /// 

    using static Helper;

    public partial class WinOpdr1 : OpdrWindow {

        public static new string Opdracht() {
            return "Taxikosten";
        }

        public static new OpdrWindow newInstance() {

            return new WinOpdr1();
        }

        public override void reset() {

            boxTime.SetTime( 17, 00 );
            boxDuration.SelectedIndex = 1;
            boxDistance.SelectedIndex = 1;
        }

        public WinOpdr1() {

            InitializeComponent();

            // fill duration

            // boxDuration.Items.Add("<5 min");
            for ( int i = 5; i < MINUTES_PER_DAY; i += 5 ) {

                boxDuration.Items.Add( durationAsString( i ) );
            }

            boxDuration.SelectedIndex = 0;


            // fill distance
            for ( int i = 5; i <= 300; i += 5 ) {

                boxDistance.Items.Add( String.Format( "{0} km", i ) );
            }

            boxDistance.SelectedIndex = 0;
        }

        private void ScrollBar_Scroll( object sender, System.Windows.Controls.Primitives.ScrollEventArgs e ) {
            
            boxTime.SelectedIndex = clamp( boxTime.SelectedIndex + barTime.direction *-1, 0, boxTime.Items.Count );
        }

        private string durationAsString( int i ) {

            int hours   = i / MINUTES_PER_HOUR;
            int mins    = i - ( hours * MINUTES_PER_HOUR );

            if ( hours == 0 ) {

                return String.Format("{0} min", mins);
            }

            if ( mins == 0 ) {

                return String.Format("{0} uur", hours);
            }

            return String.Format("{0} uur en {1} min", hours, mins);
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {

            WindowReceipt w = new WindowReceipt( Opdracht() );
            w.addToReceipt( "Kilometerheffing", ( boxDistance.SelectedIndex + 1 ) * 5, 1 );


            int startMin    = boxTime.SelectedIndex * 5;
            int durMin      = (boxDuration.SelectedIndex+1) * 5;
            int endMin      = startMin + durMin;

            if ( endMin > MINUTES_PER_DAY ) {
                MessageBox.Show("De rit eindigt niet op de dag zelf!");
                return;
            }

            const int ExpEnd    = 8  * MINUTES_PER_HOUR;
            const int ExpStart  = 18 * MINUTES_PER_HOUR;
            
            // 8:00 - start
            int ExpensiveMinutes = max(ExpEnd - startMin, 0);
            // Never more than the total ride
            ExpensiveMinutes = min( durMin, ExpensiveMinutes );

            w.addToReceipt( "Minuten voor 8:00", ExpensiveMinutes, 0.45 );

            durMin -= ExpensiveMinutes;

            // (18:00 - start) - ( 8:00 - start )
            int CheapMinutes = max( ExpStart - startMin, 0) - ExpensiveMinutes;
            CheapMinutes = min( durMin, CheapMinutes);
            w.addToReceipt( "Minuten tussen 8:00 - 18:00", CheapMinutes, 0.25 );

            durMin -= CheapMinutes;


            w.addToReceipt( "Minuten na 18:00 ", durMin, 0.45);

            if ( 
                ( boxDate.SelectedDate.Value.DayOfWeek == DayOfWeek.Saturday )  || 
                ( boxDate.SelectedDate.Value.DayOfWeek == DayOfWeek.Sunday ) || 
                ( ( boxDate.SelectedDate.Value.DayOfWeek == DayOfWeek.Friday ) && ( startMin >= 22 * MINUTES_PER_HOUR ) ) || 
                ( ( boxDate.SelectedDate.Value.DayOfWeek == DayOfWeek.Monday ) && ( startMin < 7 * MINUTES_PER_HOUR ) ) 
            ) {
                w.setCharge( "Weekendtoeslag", 0.15 );
            }
            
            w.displayReceipt();

        }
    }
}
