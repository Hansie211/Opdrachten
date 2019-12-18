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

namespace Morse {
    /// <summary>
    /// Interaction logic for WindowExport.xaml
    /// </summary>
    public partial class WindowExport : Window {
        public WindowExport() {
            InitializeComponent();
        }

        private char findMatch( string s ) {

            Boolean[] code = new Boolean[ s.Length ];

            // String to code
            for ( int i = 0; i < s.Length; i++ ) {

                code[ i ] = ( s[ i ] == '-' );
            }

            return ( (MainWindow)Application.Current.MainWindow ).findChar( code );
        }

        private void BtnImport_Click( object sender, RoutedEventArgs e ) {

            // Filter illegal characters
            string text = Regex.Replace(textBlock.Text, @"[^\.\- |]", "").Trim();


            string result = "";


            // Split the line into words
            string[] words = text.Split(' ');

            for ( int i = 0; i < words.Length; i++ ) {

                if ( words[ i ].Length == 0 ) {

                    continue;
                }

                // Split the word into letters
                string[] letters = words[i].Split('|');

                // Find a matching char for each letter
                for ( int j = 0; j < letters.Length; j++ ) {

                    if ( letters[ j ].Length == 0 ) {

                        continue;
                    }

                    char c = findMatch( letters[j] );

                    if ( (byte)c == 0 ) { //  char not found

                        continue;
                    }

                    result += c;
                }

                // Insert a space each word
                result += ' ';
            }

            // Remove double spaces
            result = Regex.Replace( result.Trim(), "[ ]{2,}", " " );

            // Update the MainWindow
            ( (MainWindow)Application.Current.MainWindow ).edtToMorse.Text = result.ToUpper();

            // Close this window
            Close();
        }
    }
}
