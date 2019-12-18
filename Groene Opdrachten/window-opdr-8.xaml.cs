using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for window_opdr_8.xaml
    /// </summary>
    /// 

    using static Helper;

    public partial class WinOpdr8 : OpdrWindow {

        public static new string Opdracht() {
            return "Transportbedrijf";
        }

        public static new OpdrWindow newInstance() {

            return new WinOpdr8();
        }

        public WinOpdr8() {

            InitializeComponent();
        }

        private void PreviewNumericText( object sender, TextCompositionEventArgs e ) {

            if ( !isNumeric( e.Text ) ) {

                e.Handled = true; // stop the event
            }
        }

        private void BtnCalc_Click( object sender, RoutedEventArgs e ) {

            // Validate input
            TextBox[] boxes = new TextBox[]{ textKMDomestic, textKMAbroad, textVolume, textWeight, textValue };
            for ( int i = 0; i < boxes.Length; i++ ) {

                if ( isNumeric( boxes[ i ].Text ) ) {

                    continue;
                }

                MessageBox.Show( $"Fout: { boxes[ i ].Text } is geen getal!" );
                return;
            }

            double kmdomestic   = parseDouble( textKMDomestic.Text );
            double kmabroad     = parseDouble( textKMAbroad.Text );
            double volume       = parseDouble( textVolume.Text );
            double weight       = parseDouble( textWeight.Text );
            double value        = parseDouble( textValue.Text );

            if ( kmdomestic + kmabroad == 0.0 ) {

                MessageBox.Show( "Fout: aantal KM kan niet 0 zijn!" );
                return;
            }

            bool abroad = (kmabroad > 0.0);
            bool liquid = (boxType.SelectedIndex == 0);

            const double liquidPriceVolume      = 0.80;
            const double liquidPriceWeight      = 0.55;
            const double nonliquidPriceVolume   = 1.25;
            const double nonliquidPriceWeight   = 0.45;

            double priceVolume = ( liquid ) ? liquidPriceVolume : nonliquidPriceVolume;
            double priceWeight = ( liquid ) ? liquidPriceWeight : nonliquidPriceWeight;

            WindowReceipt w = new WindowReceipt( Opdracht() );

            double pricePerKM = volume * priceVolume + weight * priceWeight;

            w.addTextToReceipt( $"Prijs per KM: { w.currToStr( pricePerKM) }");

            w.addToReceipt( "Kilometers", kmdomestic + kmabroad, pricePerKM );
            if ( abroad ) {

                //w.addToReceipt( "Kilometers buiten Nederland", kmabroad, pricePerKM );
                w.addToReceipt("Heffing KM buiten Nederland", 0.45, pricePerKM * kmabroad );

                double border = max( 0.035 * value, 45 );
                w.addToReceipt("Heffing douane", 1, border );
            }

            w.displayReceipt();


        }
    }
}
