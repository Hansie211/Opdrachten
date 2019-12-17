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
    /// Interaction logic for window_opdr_6.xaml
    /// </summary>
    /// 

    using static Helper;

    public partial class WinOpdr6 : OpdrWindow {

        public static new string Opdracht() {
            return "Camping";
        }

        public static new OpdrWindow newInstance() {

            return new WinOpdr6();
        }


        public override void reset() {

            boxSize.SelectedIndex       = 7;
            boxPersons.SelectedIndex    = 1;
            boxDogs.SelectedIndex       = 0;

            boxCar.IsChecked = true;

            dateStart.SelectedDate  = DateTime.Now;
            dateStart.DisplayDate   = dateStart.SelectedDate.Value;

            updateCalendar();
        }


        public WinOpdr6() {
            InitializeComponent();

            for( int i = 3; i <= 20; i++ ) {

                boxSize.Items.Add( String.Format("{0}m", i));
            }

            for( int i = 1; i <= 8; i++ ) {

                boxPersons.Items.Add( i.ToString() );
            }

            boxDogs.Items.Add("Geen");
            for( int i = 1; i < 3; i++ ) {

                boxDogs.Items.Add( i.ToString() );
            }

        }

        private void updateCalendar() {

            if ( dateStart.SelectedDate == null ) {
                return;
            }

            if ( dateStart.SelectedDate.Value < dateEnd.SelectedDate.Value ) {
                return;
            }

            DateTime tmp = dateStart.SelectedDate.Value;
            tmp = tmp.AddDays( 1 );

            dateEnd.DisplayDate = tmp;
            dateEnd.SelectedDate = tmp;
            dateEnd.DisplayDateStart = tmp;
        }


        private void BtnCalc_Click( object sender, RoutedEventArgs e ) {

            int daysLow     = 0;
            int daysHigh    = 0;

            DateTime startHigh  = new DateTime( dateStart.SelectedDate.Value.Year, 7, 11);
            DateTime endHigh    = new DateTime( dateStart.SelectedDate.Value.Year, 8, 15);

            int daysTotal   = ( dateEnd.SelectedDate.Value - dateStart.SelectedDate.Value).Days;

            if ( daysTotal < 1 ) {

                MessageBox.Show("Minimaal verblijf is 1 dag.");
                return;
            }

            if ( daysTotal > 90 ) {

                MessageBox.Show("Maximaal verblijf is 90 dagen.");
                return;
            }

            WindowReceipt w = new WindowReceipt( Opdracht() );


            // Count days before startHigh
            // Add days after endHigh

            if ( dateStart.SelectedDate.Value < startHigh ) {

                // Days until either start of 'High' or endDate
                daysLow += ( min( dateEnd.SelectedDate.Value, startHigh ) - dateStart.SelectedDate.Value ).Days;
            }

            if ( dateEnd.SelectedDate.Value > endHigh ) {

                // Days since either end of 'High' or dateStart
                daysLow += ( dateEnd.SelectedDate.Value - max( dateStart.SelectedDate.Value, endHigh ) ).Days;
            }

            // Days in 'High' is days in TotalDays - LowDays ( with a min. of 0 )
            daysHigh = max(daysTotal - daysLow, 0);

            int size = boxSize.SelectedIndex + 3;
            int sizePrice = max(0, size - 10 ) * 3 + max(0, 10 - size) * -2;

            int priceHigh   = 30 + sizePrice;
            int priceLow    = 25 + sizePrice;

            w.addToReceipt("Dagen in hoogseizoen", daysHigh, priceHigh );
            w.addToReceipt("Dagen buiten hoogseizoen", daysLow, priceLow );

            w.addToReceipt( "Personen", boxPersons.SelectedIndex + 1, 5 );
            w.addToReceipt( "Honden", boxDogs.SelectedIndex, 4 );

            if ( boxCar.IsChecked == true) {
                w.addToReceipt("Autoplaats", 1, 6);
            }

            w.displayReceipt();
        }

        private void DateStart_CalendarClosed( object sender, RoutedEventArgs e ) {
            updateCalendar();
        }
    }
}
