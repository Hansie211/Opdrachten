using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
using System.Windows.Threading;

namespace Opdracht9 {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private int freq, ms;
        
        private Boolean threadRunning = false; // used to stop the thread

        private const int dotLength     = 1;
        private const int dashLength    = 3;
        private const int pauseDot      = 1;
        private const int pauseLetter   = 3;
        private const int pauseWord     = 7;

        Dictionary<char, Boolean[]> morseCodes;

        // Requiered for threads ( GUI updates )
        private delegate void ClearCanvasCallback();
        private delegate void SetCurrentCharCallback( char c );
        private delegate void UpdateCanvasCallback( int i, Boolean isLong );
        private delegate void ReleaseButtonCallback();

        private void ClearCanvas() {

            canvas.Children.Clear();
        }

        private void SetCurrentChar( char c ) {

            if ( c == ' ') {
                c = '_';
            }

            lblCurrent.Content = c.ToString().ToUpper();
        }

        private void UpdateCanvas( int i, Boolean isLong ) {

            visualBeep( i, isLong );
        }

        private void ReleaseButton() {

            BtnGO.Click -= BtnStop_Click;
            BtnGO.Content = "GO";
            BtnGO.Click += BtnGO_Click;

            BtnGO.IsEnabled = true;

            lblCurrent.Content = "";
        }

        private void beepThread( object _text ) {

            string text     = ((string)_text).ToLower();

            for ( int i = 0; i < text.Length; i++ ) {

                if ( !threadRunning ) { // GUI Request stop
                    break;
                }

                beep( text[i] );

                if ( i < text.Length - 1 ) {

                    // Sleep between letters
                    Thread.Sleep( ms * pauseLetter );
                }
            }

            canvas.Dispatcher.Invoke( new ClearCanvasCallback( ClearCanvas ) );
            //canvas.Children.Clear();

            BtnGO.Dispatcher.Invoke( new ReleaseButtonCallback( ReleaseButton ) );
            //BtnGO.IsEnabled = true;
        }

        // -------------------------------------------------

        private void startThread() {

            BtnGO.Click -= BtnGO_Click;
            BtnGO.Content = "STOP";
            BtnGO.Click += BtnStop_Click;

            threadRunning = true;

            Thread thread = new Thread( new ParameterizedThreadStart(beepThread) );
            thread.IsBackground = true; // end thread with MainThread
            thread.Start( edtToMorse.Text );
        }

        private void stopThread() {

            threadRunning = false;
        }


        private void updateFrequency() {

            if ( lblHertz == null ) {
                return;
            }

            freq = Convert.ToInt32( sbFreq.Value );
            lblHertz.Content = String.Format("{0} Hz", freq);

        }

        private void updateDuration() {

            if ( lblMs == null ) {
                return;
            }

            ms = Convert.ToInt32( sbDuration.Value );
            lblMs.Content = String.Format( "{0} ms", ms );
        }

        public MainWindow() {
            InitializeComponent();

            // The morsecodes, intl. from https://en.wikipedia.org/wiki/Morse_code
            morseCodes = new Dictionary<char, Boolean[]>();

            morseCodes.Add( 'a', new Boolean[] { false, true } );
            morseCodes.Add( 'b', new Boolean[] { true, false, false, false } );
            morseCodes.Add( 'c', new Boolean[] { true, false, true, false } );
            morseCodes.Add( 'd', new Boolean[] { true, false, false } );
            morseCodes.Add( 'e', new Boolean[] { false } );
            morseCodes.Add( 'f', new Boolean[] { false, false, true, false } );
            morseCodes.Add( 'g', new Boolean[] { true, true, false } );
            morseCodes.Add( 'h', new Boolean[] { false, false, false, false } );
            morseCodes.Add( 'i', new Boolean[] { false, false } );
            morseCodes.Add( 'j', new Boolean[] { false, true, true, true } );
            morseCodes.Add( 'k', new Boolean[] { true, false, true } );
            morseCodes.Add( 'l', new Boolean[] { false, true, false, false } );
            morseCodes.Add( 'm', new Boolean[] { true, true } );
            morseCodes.Add( 'n', new Boolean[] { true, false } );
            morseCodes.Add( 'o', new Boolean[] { true, true, true } );
            morseCodes.Add( 'p', new Boolean[] { false, true, true, false } );
            morseCodes.Add( 'q', new Boolean[] { true, true, false, true } );
            morseCodes.Add( 'r', new Boolean[] { false, true, false } );
            morseCodes.Add( 's', new Boolean[] { false, false, false } );
            morseCodes.Add( 't', new Boolean[] { true } );
            morseCodes.Add( 'u', new Boolean[] { false, false, true } );
            morseCodes.Add( 'v', new Boolean[] { false, false, false, true } );
            morseCodes.Add( 'w', new Boolean[] { false, true, true } );
            morseCodes.Add( 'x', new Boolean[] { true, false, false, true } );
            morseCodes.Add( 'y', new Boolean[] { true, false, true, true } );
            morseCodes.Add( 'z', new Boolean[] { true, true, false, false } );

            morseCodes.Add( '1', new Boolean[] { false, true, true, true, true } );
            morseCodes.Add( '2', new Boolean[] { false, false, true, true, true } );
            morseCodes.Add( '3', new Boolean[] { false, false, false, true, true } );
            morseCodes.Add( '4', new Boolean[] { false, false, false, false, true } );
            morseCodes.Add( '5', new Boolean[] { false, false, false, false, false } );
            morseCodes.Add( '6', new Boolean[] { true, false, false, false, false } );
            morseCodes.Add( '7', new Boolean[] { true, true, false, false, false } );
            morseCodes.Add( '8', new Boolean[] { true, true, true, false, false } );
            morseCodes.Add( '9', new Boolean[] { true, true, true, true, false } );
            morseCodes.Add( '0', new Boolean[] { true, true, true, true, true } );


            updateFrequency();
            updateDuration();
        }

        public char findChar( Boolean[] code ) {

            // For all keys
            for( int i = 0; i < morseCodes.Keys.Count - 1; i++ ) {

                Boolean correct = true;

                // Get the key and the code
                char c = morseCodes.Keys.ElementAt(i);
                Boolean[] subcode = morseCodes[c];

                // Easy picking
                if ( code.Length != subcode.Length ) {
                    continue;
                }

                // Compare the codes
                for ( int j = 0; j < code.Length; j++ ) {

                    if ( code[j] != subcode[j] ) {

                        correct = false;
                        break;
                    }
                }

                if ( correct ) {

                    // Same code? Return the char
                    return c;
                }

            }

            return (char)0;
        }

        private void visualBeep( int i, Boolean isLong ) {

            Line line = new Line();

            line.Stroke = Brushes.Black;
            line.StrokeThickness = 3;

            const int longLineSize  = 15;
            const int shortLineSize = 8;
            const int space         = 7;

            line.X1 = 3 + i * ( longLineSize + space );
            line.X2 = line.X1 + ((isLong) ? longLineSize : shortLineSize );
            line.Y1 = 7;
            line.Y2 = line.Y1;

            // Give the line a rounded edge
            line.StrokeStartLineCap = PenLineCap.Round;
            line.StrokeEndLineCap = PenLineCap.Round;

            canvas.Children.Add( line );
        }

        private void beep( Boolean isLong ) {

            int _ms = ( isLong ) ? ms * dashLength : ms * dotLength;

            Console.Beep( freq, _ms );
        }

        private void beep( char c ) {

            lblCurrent.Dispatcher.Invoke( new SetCurrentCharCallback(SetCurrentChar), new object[] { c } );

            canvas.Dispatcher.Invoke( new ClearCanvasCallback( ClearCanvas ) );
            //canvas.Children.Clear();

            // Special
            if ( c == ' ' ) {
                // Sleep between words
                Thread.Sleep( ms * pauseWord );
                return;
            }

            if ( !morseCodes.ContainsKey( c ) ) {
                return;
            }

            Boolean[] code = morseCodes[c];
            for ( int i = 0; i < code.Length; i++ ) {

                canvas.Dispatcher.Invoke( new UpdateCanvasCallback(UpdateCanvas), new object[] { i, code[i] } );
                //visualBeep( i , code[i]);

                beep( code[i] );

                if ( i < code.Length - 1 ) {
                    // Sleep between dots
                    Thread.Sleep( ms * pauseDot );
                }
            }

        }

        private void ScrollBar_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e ) {

            updateFrequency();
        }

        private void SbDuration_ValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e ) {

            updateDuration();
        }

        private void BtnTestLong_Click( object sender, RoutedEventArgs e ) {

            beep(true);
        }

        private void BtnTestShort_Click( object sender, RoutedEventArgs e ) {

            beep(false);
        }

        private void BtnGO_Click( object sender, RoutedEventArgs e ) {

            if ( edtToMorse.Text.Length == 0 ) {
                return;
            }

            startThread();
        }
        private void BtnStop_Click( object sender, RoutedEventArgs e ) {

            BtnGO.IsEnabled = false;
            stopThread();
        }

        private string morseToText( Boolean[] code ) {

            string result = "";

            for( int i = 0; i < code.Length; i++ ) {

                result += (code[i]) ? '-' : '.';
            }

            return result;
        }

        private void BtnExport_Click( object sender, RoutedEventArgs e ) {

            string output = "";
            string text = edtToMorse.Text.ToLower().Trim();

            if ( text.Length == 0 ) {

                goto displayForm;
            }

            // Remove double spaces
            text = Regex.Replace( text, "[ ]{2,}", " " );

            for ( int i = 0; i < text.Length; i++ ) {

                if ( text[i] == ' ' ) {

                    output += ' ';
                    continue;
                }

                if ( !morseCodes.ContainsKey(text[i]) ) {
                    continue;
                }

                output += morseToText( morseCodes[text[i]] ) + '|';
            }

        displayForm:
            WindowExport w = new WindowExport();

            // Remove letter-seperator at the end of a word
            output = output.Replace("| ", " ").TrimEnd( '|' );

            w.textBlock.Text = output;

            w.ShowDialog();
        }

        private void CheckBox_Click( object sender, RoutedEventArgs e ) {

            sbFreq.Maximum = ( ( (CheckBox)sender ).IsChecked == true ) ? 32767 : 1200;
            sbFreq.Minimum = ( ( (CheckBox)sender ).IsChecked == true ) ? 37 : 100;
        }
    }
}
