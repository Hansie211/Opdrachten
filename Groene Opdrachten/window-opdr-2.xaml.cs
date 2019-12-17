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
    /// Interaction logic for window_opdr_2.xaml
    /// </summary>
    /// 

    using static Helper;

    public partial class WinOpdr2 : OpdrWindow {

        public static new string Opdracht() {
            return "Ouderbijdrage school";
        }

        public static new OpdrWindow newInstance() {

            return new WinOpdr2();
        }

        public void resetBirthdate() {
            dateBox.SelectedDate = new DateTime( DateTime.Now.Year - 10, 1, 1 );
            dateBox.DisplayDate = dateBox.SelectedDate.Value;
        }

        public override void reset() {
            
            resetBirthdate();

            children = new Child[0];
        }

        public struct Child {

            public string name;
            public DateTime date;
        }

        Child[] children;

        public WinOpdr2() {

            InitializeComponent();
        }

        private void removeChild( int idx ) {

            if ( !splice( ref children, idx ) ) {
                return;
            }

            listBox.Items.Remove( listBox.Items[idx] );
        }

        private int addChild( string name, DateTime date ) {

            int i = children.Length;
            Array.Resize( ref children, i + 1);

            children[i].name = name;
            children[i].date = date;

            listBox.Items.Add( name );
            //listBox.SelectedIndex = listBox.Items.Count - 1;

            return i;
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {

            if ( textBox.Text.Length == 0 ) {
                
                MessageBox.Show("De naam ontbreekt.");
                return;
            }

            if ( dateBox.SelectedDate == null ) {

                MessageBox.Show( "De datum ontbreekt." );
                return;
            }

            addChild( textBox.Text, dateBox.SelectedDate.Value );

            resetBirthdate();
            textBox.Clear();

            textBox.Focus();
        }

        private void Button1_Click( object sender, RoutedEventArgs e ) {

            removeChild( listBox.SelectedIndex );
        }

        private void ButtonCalc_Click( object sender, RoutedEventArgs e ) {

            if ( children.Length == 0 ) {

                MessageBox.Show("Voer minimaal 1 kind in!");
                return;
            }

            WindowReceipt w = new WindowReceipt(Opdracht());

            int[] young, old;
            int[] ages = new int[children.Length];

            DateTime peilDatum = ( boxPeil.SelectedDate == null ) ? DateTime.Now : boxPeil.SelectedDate.Value;

            // Get all ages
            for( int i = 0; i < children.Length; i++ ) {

                ages[i] = age( children[i].date, peilDatum );
            }

            split( out young, out old, 10, ages );

            w.addToReceipt( "Basisbedrag", 1, 50 );
            w.addToReceipt( "Kind jonger dan 10", min( young.Length, 3 ), 25 );
            w.addToReceipt( "Kind ouder dan 10", min( old.Length, 2 ), 37 );

            double discount = w.getSum() - 150;
            if ( discount > 0 ) {

                w.addToReceipt("Max. €150,-", 1, -1 * discount);
            }

            if ( checkBox.IsChecked == true ) {
                w.setCharge( "Éénoudergezin reductie", -0.25);
            }

            w.displayReceipt();

        }
    }
}
