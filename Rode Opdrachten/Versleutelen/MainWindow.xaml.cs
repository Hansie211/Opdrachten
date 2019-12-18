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
using Path = System.IO.Path;

namespace Versleutelen {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window {

        private byte bitmode = 8;

        public MainWindow() {
            InitializeComponent();

            CipherManager.init();

            for ( int i = 0; i < CipherManager.ciphers.Length; i++ ) {
                boxCipher.Items.Add( CipherManager.ciphers[ i ].getName() );
            }
        }

        private void setFile( string filepath ) {

            lblFilename.Content = Path.GetFileName( filepath );
            lblFilename.Tag     = filepath;
            lblFilename.ToolTip = lblFilename.Content;
        }

        private void BtnOpenFile_Click( object sender, RoutedEventArgs e ) {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Tekst bestanden (*.txt)|*.txt|Versleutelde bestanden (*.enc)|*.enc|Alle bestanden(*.*)|*.*";

            if ( !(bool)open.ShowDialog() ) {
                return;
            }

            setFile( open.FileName );
            gridControls.IsEnabled = true;
        }

        private Boolean SaveFile( byte[] data, ref string proposedName ) {

            SaveFileDialog save = new SaveFileDialog();
            save.Filter     = "All bestanden (*.*)|*.*";
            save.FileName   = Path.GetFileName( proposedName );

            if ( !(bool)save.ShowDialog() ) {
                return false;
            }

            FileStream stream;
            try {
                stream = new FileStream( save.FileName, FileMode.Create, FileAccess.Write );
            } catch {
                MessageBox.Show( String.Format( "Kan bestand '{0}' niet aanmaken!", save.FileName ) );
                return false;
            }

            stream.Write( data, 0, data.Length );
            stream.Close();

            proposedName = save.FileName;

            return true;

        }

        private void BtnEncrypt_Click( object sender, RoutedEventArgs e ) {

            if ( lblFilename.Tag == null ) {
                MessageBox.Show( "Selecteer eerst een bestand!" );
                return;
            }

            byte[] data;

            string password     = edtPassword.Password;
            string inputFile    = lblFilename.Tag.ToString();
            string outputFile   = inputFile + ".enc"; // append '.enc'

            if ( password.Length == 0 ) {
                MessageBox.Show( "Een wachtwoord is verplicht!" );
                return;
            }

            if ( !CipherManager.encrypt( bitmode, boxCipher.SelectedIndex, password, inputFile, out data ) ) {

                MessageBox.Show( String.Format( "Het is niet gelukt bestand '{0}' te versleutelen.", inputFile ) );
                return;
            }

            if ( !SaveFile( data, ref outputFile ) ) {
                return;
            }

            setFile( outputFile );
        }

        private void BtnDecrypt_Click( object sender, RoutedEventArgs e ) {

            if ( lblFilename.Tag == null ) {
                MessageBox.Show( "Selecteer eerst een bestand!" );
                return;
            }

            byte[] data;

            string password     = edtPassword.Password;
            string inputFile    = lblFilename.Tag.ToString();
            string outputFile   = inputFile.Substring(0, inputFile.Length - 4); // remove '.enc'

            if ( password.Length == 0 ) {
                MessageBox.Show( "Een wachtwoord is verplicht!" );
                return;
            }

            if ( !CipherManager.decrypt( bitmode, boxCipher.SelectedIndex, password, inputFile, out data ) ) {

                MessageBox.Show( String.Format( "Het is niet gelukt bestand '{0}' te ontsleutelen. Controleer de methode, de bitmodus en het wachtwoord.", inputFile ) );
                return;
            }

            if ( !SaveFile( data, ref outputFile ) ) {
                return;
            }

            setFile( outputFile );
        }

        private void RbBitmode_Checked( object sender, RoutedEventArgs e ) {

            if ( ( (RadioButton)sender ).Tag == null ) {
                return;
            }

            Byte.TryParse( ( (RadioButton)sender ).Tag.ToString(), out bitmode );
        }

    }
}
