using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Opdracht4 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    struct CharItem {

        public byte value;
        public uint count;
    }

    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        private void BtnOpenFile_Click( object sender, RoutedEventArgs e ) {

            OpenFileDialog open = new OpenFileDialog();
            if ( !(bool)open.ShowDialog() ) {
                return;
            }

            FileStream stream;
            try {
                stream = new FileStream( open.FileName, FileMode.Open, FileAccess.Read);
            } catch( Exception ex ) {

                Console.WriteLine( ex.Message );
                return;
            }

            CharItem[] chars = new CharItem[256];
            // Init the list
            for ( int i = 0; i < chars.Length; i++ ) { /* note to self: if i is a byte, 'i < chars.Length' creates and endless loop */
                chars[i].value = (byte)( i & 0xFF );
                chars[i].count = 0;
            }

            // Read in blocks, much faster than byte-by-byte
            byte[] buff = new byte[1024];

            while ( stream.Position < stream.Length ) {

                int bytesRead = stream.Read(buff, 0, buff.Length);

                for ( int i = 0; i < bytesRead; i++ ) { 

                    byte b = buff[i];

                    if ( b < 33) { // non-printable
                        continue;
                    }

                    chars[b].count++;
                }
            }

            // Close the file
            stream.Close();

            // Sort the result
            for ( int i = 0; i < chars.Length - 1; i++ ) {

                for( int j = i + 1; j < chars.Length; j++ ) {

                    if ( chars[i].count < chars[j].count ) {

                        // swap
                        CharItem tmp = chars[i];
                        chars[i] = chars[j];
                        chars[j] = tmp;
                    }
                }
            }

            // Clean previous results
            listBox.Items.Clear();

            // Display the new results
            var enc = Encoding.GetEncoding("437"); // Make sure all characters are encoded correctly
            for ( int i = 0; i < chars.Length; i++ ) {

                if ( chars[i].count == 0 ) {
                    break;
                }

                listBox.Items.Add( String.Format( "[{0:X2}] {1}   -   {2}", chars[i].value, enc.GetString( new byte[] { chars[i].value } ), chars[i].count) );
            }

        }
    }
}
