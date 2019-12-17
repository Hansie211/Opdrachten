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
    /// Interaction logic for window_opdr_3.xaml
    /// </summary>
    /// 

    using static Helper;

    public partial class WinOpdr3 : OpdrWindow {

        public static new string Opdracht() {
            return "Autoverhuur";
        }

        public static new OpdrWindow newInstance() {

            return new WinOpdr3();
        }

        public override void reset() {

            updateCalendar();
        }

        private int km = 0;

        public WinOpdr3() {

            InitializeComponent();
        }

        private void TextBox_TextChanged( object sender, TextChangedEventArgs e ) {

            if ( textBox.Text.Length == 0 ) {
                km = 0;

                return;
            }

            int tmp;
            if ( !int.TryParse( textBox.Text, out tmp ) ) {

                textBox.Text = km.ToString();
                textBox.CaretIndex = textBox.Text.Length;
                return;
            }

            km = tmp;
        }

        private void ButtonCalc_Click( object sender, RoutedEventArgs e ) {
            const int consumptionSmall  = 5; // 5L / 100km
            const int consumptionLarge  = 8; // 8L / 100km

            const double literPrice = 1.39;

            WindowReceipt w = new WindowReceipt("Autoverhuur");

            if ( dateStart.SelectedDate == null ) {

                MessageBox.Show( "Begindatum ontbreekt!" );
                return;
            }

            if ( dateEnd.SelectedDate == null ) {

                MessageBox.Show( "Einddatum ontbreekt!" );
                return;
            }

            if ( km <= 0 ) {

                MessageBox.Show("Aantal km ontbreekt!");
                return;
            }

            int days = ( dateEnd.SelectedDate.Value - dateStart.SelectedDate.Value ).Days;

            if ( days < 1 ) {

                MessageBox.Show( "Verhuur minimaal 1 dag!" );
                return;
            }

            int c;

            if ( rbSmall.IsChecked == true ) {

                // Set the car consumption
                c = consumptionSmall;

                // Set the basic cost
                w.addToReceipt( "Personenauto per dag", days, 50 );

                // 100km each day free
                int kmPrice = max( km - (days*100), 0 );
                w.addToReceipt( "Kilometer heffing", kmPrice, 0.20 );
            }
            else {
                
                // Set the car consumption
                c = consumptionLarge;

                // Set the basic cost
                w.addToReceipt( "Personenbusje per dag", days, 95 );

                // No free km's
                w.addToReceipt( "Kilometer heffing", km, 0.30 );
            }

            double gasUsage = Math.Round( ( (double)km / 100 ) * c, 2 );
            w.addToReceipt("Benzine (L)", gasUsage, literPrice );

            w.displayReceipt();


        }

        private void DateStart_CalendarClosed( object sender, RoutedEventArgs e ) {

            updateCalendar();
        }

        private void updateCalendar() {

            if ( dateStart.SelectedDate == null ) {
                return;
            }

            DateTime tmp = dateStart.SelectedDate.Value;
            tmp = tmp.AddDays( 1 );

            dateEnd.DisplayDate         = tmp;
            dateEnd.SelectedDate        = tmp;
            dateEnd.DisplayDateStart    = tmp;
        }
    }
}
