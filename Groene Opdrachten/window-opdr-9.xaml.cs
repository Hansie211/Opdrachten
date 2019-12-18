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
    /// Interaction logic for window_opdr_9.xaml
    /// </summary>
    /// 

    using static Helper;


    public partial class WinOpdr9 : OpdrWindow {

        public static new string Opdracht() {
            return "Waterverbruik";
        }

        public static new OpdrWindow newInstance() {

            return new WinOpdr9();
        }

        private struct Tarrif {

            public double basePrice;
            public double usagePrice;
        }

        private readonly Tarrif[] tarrifs = new Tarrif[]{ 
            new Tarrif{
                basePrice = 100, 
                usagePrice = 0.25,
            }, 
            new Tarrif{
                basePrice = 75, 
                usagePrice = 0.38,
            },
        };

        public WinOpdr9() {
            InitializeComponent();

            for( int i = 0; i < tarrifs.Length; i++ ) {

                comboBox.Items.Add( new ComboBoxItem() { Content = $"Tarief {i + 1}" } );
            }

            comboBox.Items.Add( new ComboBoxItem() { Content = $"Goedkoopste tarief" } );

            comboBox.SelectedIndex = 0;
        }

        private void PreviewNumericText( object sender, TextCompositionEventArgs e ) {

            if ( !isNumeric( e.Text ) ) {

                e.Handled = true; // stop the event
            }
        }

        private Tarrif getLowest( double m3 ) {

            Tarrif result   = tarrifs[0];
            double lowest   = double.MaxValue;

            for ( int i = 0; i < tarrifs.Length; i++ ) {

                double temp = tarrifs[i].basePrice + tarrifs[i].usagePrice * m3;

                if ( temp < lowest ) {
                    lowest  = temp;
                    result  = tarrifs[ i ];
                }
            }

            return result;
        }

        private void BtnCalc_Click( object sender, RoutedEventArgs e ) {

            if ( !isNumeric( textVolume.Text ) ) {

                MessageBox.Show( $"Fout: {textVolume.Text} is geen getal!" );
                return;
            }

            double m3 = parseDouble( textVolume.Text );

            WindowReceipt w = new WindowReceipt( Opdracht() );

            Tarrif tarrif;

            if ( comboBox.SelectedIndex < tarrifs.Length ) {

                tarrif = tarrifs[ comboBox.SelectedIndex ];
            } else {
                tarrif = getLowest( m3 );
            }

            w.addToReceipt( "Vastrecht", 1, tarrif.basePrice );
            w.addToReceipt( "Verbruikskosten", m3, tarrif.usagePrice );

            w.displayReceipt();
        }
    }
}
