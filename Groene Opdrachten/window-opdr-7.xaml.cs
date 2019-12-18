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
    /// Interaction logic for window_opdr_7.xaml
    /// </summary>
    /// 

    using static Helper;

    public partial class WinOpdr7 : OpdrWindow {

        public static new string Opdracht() {
            return "Containerverhuur";
        }

        public static new OpdrWindow newInstance() {

            return new WinOpdr7();
        }

        private struct Month {

            public string name;
            public int days;
        }

        Month[] months;

        private void updateDays( int month, ComboBox box ) {

            // Prevent event loop
            box.SelectionChanged -= BoxMonthStart_SelectionChanged;

            // Keep a copy of the selected index
            int sel = box.SelectedIndex;

            box.Items.Clear();

            // Add all the months
            for(  int i = 0; i < months[month].days; i++ ) {

                box.Items.Add( (i+1).ToString() );
            }

            // Check if the day is valid
            if ( sel > months[month].days - 1 ) {
                sel = months[month].days - 1;
            }

            box.SelectedIndex = sel;

            // Restore event
            box.SelectionChanged += BoxMonthStart_SelectionChanged;
        }

        private void updateDays( ComboBox month, ComboBox box ) {

            updateDays( month.SelectedIndex, box );
        }

        private int toDays( int month, int day ) {

            int result = day;
            for( int i = 0; i < month; i++ ) {

                result += months[i].days;
            }

            return result;
        }

        private int daysBetween( int monthStart, int dayStart, int monthEnd, int dayEnd ) {

            return toDays( monthEnd, dayEnd ) - toDays( monthStart, dayStart );
        }

        public WinOpdr7() {

            InitializeComponent();

            months = new Month[12];

            months[0] = new Month { name = "Januari",   days = 31, };
            months[1] = new Month { name = "Februari",  days = 28, };
            months[2] = new Month { name = "Maart",     days = 31, };
            months[3] = new Month { name = "April",     days = 30, };
            months[4] = new Month { name = "Mei",       days = 31, };
            months[5] = new Month { name = "Juni",      days = 30, };
            months[6] = new Month { name = "Juli",      days = 31, };
            months[7] = new Month { name = "Augustus",  days = 31, };
            months[8] = new Month { name = "September", days = 30, };
            months[9] = new Month { name = "Oktober",   days = 31, };
            months[10] = new Month { name = "November", days = 30, };
            months[11] = new Month { name = "December", days = 31, };

            for( int i = 0; i < months.Length; i++ ) {

                boxMonthStart.Items.Add( months[i].name );
                boxMonthEnd.Items.Add( months[i].name );
            }

            for ( int i = 0; i < 8; i += 2 ) {

                boxSize.Items.Add( String.Format( "{0} m3", i + 2 ) );
            }
            boxSize.SelectedIndex = 0;

            for ( int i = 0; i < 100; i ++ ) {

                boxAmount.Items.Add( String.Format( "{0} m3", i + 1 ) );
            }
            boxAmount.SelectedIndex = 0;

            boxMonthStart.SelectedIndex = 6;
            boxMonthEnd.SelectedIndex = 7;

            boxDayStart.SelectedIndex = 0;
            boxDayEnd.SelectedIndex = 0;

            updateDays( boxMonthStart, boxDayStart );
            updateDays( boxMonthEnd, boxDayEnd );
        }

        private void BoxMonthStart_SelectionChanged( object sender, SelectionChangedEventArgs e ) {

            int lastMonth = months.Length - 1;

            // Cannot select last day of last month for start
            if ( ( boxMonthStart.SelectedIndex == lastMonth ) && ( boxDayStart.SelectedIndex == months[lastMonth].days - 1 ) ) {

                boxDayStart.SelectedIndex--;
            }

            updateDays( boxMonthStart, boxDayStart );

            // Cannot 'start' after 'end'
            if ( boxMonthStart.SelectedIndex > boxMonthEnd.SelectedIndex ) {

                boxMonthEnd.SelectedIndex = boxMonthStart.SelectedIndex;
            }

            // Same goes for days
            if ( boxMonthStart.SelectedIndex == boxMonthEnd.SelectedIndex ) {

                if ( boxDayStart.SelectedIndex == boxDayStart.Items.Count - 1 ) { // last day of the month

                    // Increase month
                    boxMonthEnd.SelectedIndex = boxMonthStart.SelectedIndex + 1;
                    boxDayEnd.SelectedIndex = 0;

                } else if ( boxDayStart.SelectedIndex >= boxDayEnd.SelectedIndex ) {

                    // Increase day
                    boxDayEnd.SelectedIndex = boxDayStart.SelectedIndex + 1;
                }

                updateDays( boxMonthEnd, boxDayEnd );
            }

        }

        private void BtnCalc_Click( object sender, RoutedEventArgs e ) {

            int days = daysBetween( boxMonthStart.SelectedIndex, boxDayStart.SelectedIndex, boxMonthEnd.SelectedIndex, boxDayEnd.SelectedIndex );

            int size    = ( (boxSize.SelectedIndex + 1) * 2);
            int amount  = (boxAmount.SelectedIndex + 1);

            int containerCount = amount / size;

            if ( containerCount * size < amount ) { // rest
                containerCount++;
            }

            WindowReceipt w = new WindowReceipt( Opdracht() );

            w.addToReceipt("Dagen", days, 40);

            int cost = ( size <= 2 ) ? 60 : 125;

            w.addToReceipt("Afvoeren afval", containerCount, cost);

            if ( checkBox.IsChecked == true ) {
                w.setCharge("Vaste klanten korting", -0.15);
            }

            w.displayReceipt();
        }
    }
}
