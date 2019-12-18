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
    /// Interaction logic for window_opdr_10.xaml
    /// </summary>
    /// 

    using static Helper;


    public partial class WinOpdr10 : OpdrWindow {

        public static new string Opdracht() {
            return "Glashandel";
        }

        public static new OpdrWindow newInstance() {

            return new WinOpdr10();
        }

        private struct Tarrif {

            public string name;
            public int cutPrice;
            public int areaPrice;
        }

        Tarrif[] tarrifs = new Tarrif[]{

            new Tarrif() {
                name        = "Gewoon glas",
                cutPrice    = 10,
                areaPrice   = 30,
            },
            new Tarrif() {
                name        = "Speciaal glas",
                cutPrice    = 25,
                areaPrice   = 55,
            },
        };


        public WinOpdr10() {
            InitializeComponent();

            for ( int i = 0; i < tarrifs.Length; i++ ) {

                boxType.Items.Add( new ComboBoxItem() { Content = tarrifs[ i ].name } );
            }

            boxType.SelectedIndex = 0;
        }

        private void PreviewNumericText( object sender, TextCompositionEventArgs e ) {

            if ( !isNumeric( e.Text ) ) {

                e.Handled = true; // stop the event
            }
        }

        private void BtnCalc_Click( object sender, RoutedEventArgs e ) {

            if ( !isNumeric( textArea.Text ) ) {

                MessageBox.Show( $"Fout: {textArea.Text} is geen getal!" );
                return;
            }

            double area     = Math.Round( parseDouble( textArea.Text ), 2);
            Tarrif tarrif   = tarrifs[ boxType.SelectedIndex ];
            bool rests      = (cbRest.IsChecked == true);

            if ( area == 0 ) {

                MessageBox.Show($"Fout: Oppervlakte kan niet kleiner zijn dan 0.01 m²" );
                return;
            }

            if ( !rests ) {

                area = Math.Ceiling( area );
            }

            double price    = area * tarrif.areaPrice;
            double cutprice = ( price < 145 ) ? tarrif.cutPrice : 0;

            WindowReceipt w = new WindowReceipt( Opdracht() );

            w.addToReceipt("Glasprijs", area, tarrif.areaPrice );
            w.addToReceipt("Snijkosten", 1, cutprice );

            if ( price > 250 ) {

                w.setCharge("Korting boven €250,-", -0.05);
            }

            w.displayReceipt();
        }
    }
}
