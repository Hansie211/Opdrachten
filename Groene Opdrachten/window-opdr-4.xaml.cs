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
    /// Interaction logic for window_opdr_4.xaml
    /// </summary>
    /// 

    using static Helper;

    public partial class WinOpdr4 : OpdrWindow {

        public static new string Opdracht() {
            return "Dierenpark";
        }

        public static new OpdrWindow newInstance() {

            return new WinOpdr4();
        }


        public override void reset() {

            listBox.Items.Clear();
            abbos = new Abbonement[0];

            tabControl.SelectedIndex = 0;
        }

        public struct Abbonement {

            public string mainName;
            public DateTime mainDate;

            public Boolean hasSpouce;
            public string spouceName;
            public DateTime spouceDate;

            public int childCount;
        }

        Abbonement[] abbos;

        private void validatePerson( string name, DateTime date, string desc ) {

            if ( name.Length == 0 ) {
                throw new Exception( String.Format("Naam is verplicht voor '{0}'.", desc) );
            }

            if ( age( date, datePeildatum.SelectedDate.GetValueOrDefault() ) < 18 ) {
                throw new Exception( String.Format("Minimale leeftijd is 18 jaar voor '{0}'.", desc) );
            }
        }

        private void addAbbo( string mainName, DateTime mainDate, Boolean hasSpouce, string spouceName, DateTime spouceDate, int childCount ) {

            // Validate
            validatePerson( mainName, mainDate, "aanvrager" );
            if ( hasSpouce ) {
                validatePerson( spouceName, spouceDate, "partner");
            }

            // Allocate space
            int i = abbos.Length;
            Array.Resize( ref abbos, i + 1);

            // Copy the data
            abbos[i].mainName   = mainName;
            abbos[i].mainDate   = mainDate;
            abbos[i].hasSpouce  = hasSpouce;
            abbos[i].spouceName = spouceName;
            abbos[i].spouceDate = spouceDate;
            abbos[i].childCount = childCount;

            // Update the list
            string info = mainName;
            if ( hasSpouce ) {
                info += String.Format( " & {0}", spouceName );
            }

            if ( childCount > 0) {

                info += String.Format( " + {0} kind(eren)", childCount );
            }

            listBox.Items.Add( info );
        }

        private void removeAbbo( int index ) {

            if (!splice( ref abbos, index ) ) {

                return;
            }

            listBox.Items.Remove( listBox.Items[index] );
        }

        public WinOpdr4() {
            InitializeComponent();

            dateMain.SelectedDate       = DateTime.Now.AddYears( -18 );
            dateMain.DisplayDateEnd     = dateMain.SelectedDate.Value;
            dateSpouce.SelectedDate     = dateMain.SelectedDate.Value;
            dateSpouce.DisplayDateEnd   = dateMain.SelectedDate.Value;

            for( int i = 0; i < tabControl.Items.Count; i++ ) {

                (( TabItem )tabControl.Items[i]).Height = 0;
            }

            for( int i = 0; i < 10; i++ ) {

                boxChildren.Items.Add( i.ToString() );
            }
        }

        private void BtnAddPerson_Click( object sender, RoutedEventArgs e ) {

            try {
                addAbbo( edtNameMain.Text, dateMain.SelectedDate.GetValueOrDefault(), checkBox.IsChecked == true, edtNameSpouce.Text, dateSpouce.SelectedDate.GetValueOrDefault(), boxChildren.SelectedIndex );
            }
            catch ( Exception E ) {

                MessageBox.Show( E.Message );
                return;
            }

            // Return page
            tabControl.SelectedIndex = 0;
        }

        private void BtnNewPerson_Click( object sender, RoutedEventArgs e ) {

            // Prepare
            edtNameMain.Clear();
            edtNameSpouce.Clear();

            checkBox.IsChecked = false;
            groupBox.IsEnabled = false;

            dateMain.SelectedDate = dateMain.DisplayDateEnd.Value;
            dateMain.DisplayDate = dateMain.DisplayDateEnd.Value;

            dateSpouce.SelectedDate = dateMain.DisplayDateEnd.Value;
            dateSpouce.DisplayDate = dateMain.DisplayDateEnd.Value;

            boxChildren.SelectedIndex = 0;

            // Display new page
            tabControl.SelectedIndex = 1;

        }

        private void BtnRemovePerson_Click( object sender, RoutedEventArgs e ) {

            removeAbbo( listBox.SelectedIndex );
        }

        private void BtnCalc_Click( object sender, RoutedEventArgs e ) {

            WindowReceipt w = new WindowReceipt( Opdracht() );

            for( int i = 0; i < abbos.Length; i++ ) {

                int _age;

                _age = age( abbos[i].mainDate, datePeildatum.SelectedDate.GetValueOrDefault() );

                if ( _age < 18 ) {

                    MessageBox.Show( String.Format("Hoofdaanvrager '{0}' is jonger dan 18 op de peildatum", abbos[i].mainName) );
                    continue;
                }

                Boolean old = _age > 65;
                if ( abbos[i].hasSpouce ) {

                    _age = age( abbos[i].spouceDate, datePeildatum.SelectedDate.GetValueOrDefault() );

                    if ( _age < 18 ) {

                        MessageBox.Show( String.Format("Partner '{0}' is jonger dan 18 op de peildatum", abbos[i].spouceName) );
                        continue;
                    }


                    old = (old) && ( _age > 65 );
                }

                switch( abbos[i].hasSpouce ) {

                    case true:

                        switch( old ) {

                            // 65+ couple
                            case true:
                                w.addToReceipt( "Echtpaar 65+", 1, 65 );
                                w.addToReceipt( "Kind los", abbos[i].childCount, 11 );
                            break;

                            // 65- couple
                            case false:
                                
                                switch ( abbos[i].childCount ) {

                                    case 0: // Echtpaar zonder kinderen
                                        w.addToReceipt( "Echtpaar", 1, 61 ); 
                                    break;

                                    case 1: // Echtpaar met 1 kind
                                        w.addToReceipt( "Echtpaar + 1 kind", 1, 71 );
                                    break;

                                    default: // Echtpaar met 2 of meer kinderen
                                        w.addToReceipt( "Echtpaar + 2 kinderen", 1, 82 );
                                        w.addToReceipt( "Kind los", max( abbos[i].childCount - 2, 0 ), 11 );
                                    break;
                                }

                            break;
                        }

                    break;

                    case false:

                        switch( old ) {

                            // 65+ single
                            case true:
                                w.addToReceipt("Los 65+", 1, 26);
                            break;

                            // 65- single
                            case false:
                                w.addToReceipt("Volwassen los", 1, 30);
                            break;
                        }

                        w.addToReceipt("Kind los", abbos[i].childCount, 11 );

                    break;
                }

            }


            if ( w.getSum() == 0 ) {
                w.Close();
                return;
            }

            w.displayReceipt();
        }

        private void CheckBox_Click( object sender, RoutedEventArgs e ) {
            groupBox.IsEnabled = (checkBox.IsChecked == true);
        }

        private void BtnCancel_Click( object sender, RoutedEventArgs e ) {

            tabControl.SelectedIndex = 0;
        }
    }
}
