using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace Lettertype_Chaos {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    class RandomFont {

        private static readonly RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();

        public FontFamily[]     fonts;
        public FontWeight[]     weights;
        public Brush[]          colors;
        public FontStyle[]      styles;
        public TextDecorationCollection[] decorations;

        public TextEffect[]     effects;

        public const int MAX_SIZE = 48;
        public const int MIN_SIZE = 12;

        private int randomInt( int max = 0x7FFFFFFF ) {

            byte[] b = new byte[4];
            random.GetBytes( b );

            uint result = (uint)( (b[0] << 0) | (b[1] << 8) | (b[2] << 16) | (b[3] << 24) ) % (uint)max;

            return (int)result;
        }

        private int randomIndex( Array list, byte[] weightTable = null ) {

            if ( weightTable == null ) {
                return randomInt( list.Length );
            }

            int sum = 0;
            for ( int i = 0; i < weightTable.Length; i++ ) {
                sum += weightTable[ i ];
            }

            int rand = randomInt(sum);

            sum = 0;
            for ( int i = 0; i < weightTable.Length; i++ ) {
                sum += weightTable[ i ];

                if ( rand < sum ) {
                    return i;
                }
            }

            return 3; // random, right?
        }

        public RandomFont() {


            // Init all options

            string[] fontNames = new string[15];
            fontNames[ 0 ]  = "Tahoma";
            fontNames[ 1 ]  = "Courier New";
            fontNames[ 2 ]  = "Segoe UI";
            fontNames[ 3 ]  = "Arial";
            fontNames[ 4 ]  = "Helvetica";
            fontNames[ 5 ]  = "Times New Roman";
            fontNames[ 6 ]  = "Times";
            fontNames[ 7 ]  = "Verdana";
            fontNames[ 8 ]  = "Georgia";
            fontNames[ 9 ]  = "Palatino";
            fontNames[ 10 ] = "Garamond";
            fontNames[ 11 ] = "Bookman";
            fontNames[ 12 ] = "Comic Sans MS";
            fontNames[ 13 ] = "Trebuchet MS";
            fontNames[ 14 ] = "Impact";

            fonts = new FontFamily[ fontNames.Length ];
            for ( int i = 0; i < fonts.Length; i++ ) {

                fonts[ i ] = new FontFamily( fontNames[ i ] );
            }

            weights = new FontWeight[ 3 ];
            weights[ 0 ] = FontWeights.Bold;
            weights[ 1 ] = FontWeights.Regular;
            weights[ 2 ] = FontWeights.Thin;

            colors = new Brush[ 5 ];
            colors[ 0 ] = Brushes.Black;
            colors[ 1 ] = Brushes.Red;
            colors[ 2 ] = Brushes.Blue;
            colors[ 3 ] = Brushes.Fuchsia;
            colors[ 4 ] = Brushes.Green;

            styles = new FontStyle[ 3 ];
            styles[ 0 ] = FontStyles.Normal;
            styles[ 1 ] = FontStyles.Italic;
            styles[ 2 ] = FontStyles.Oblique;

            decorations = new TextDecorationCollection[ 5 ];
            decorations[ 0 ] = null; // Regular
            decorations[ 1 ] = TextDecorations.Baseline;
            decorations[ 2 ] = TextDecorations.OverLine;
            decorations[ 3 ] = TextDecorations.Strikethrough;
            decorations[ 4 ] = TextDecorations.Underline;

            effects = new TextEffect[ 5 ];
            effects[ 0 ] = new TextEffect(); // Regular
            effects[ 1 ] = new TextEffect( new RotateTransform( 2 ), null, null, 0, 20 );
            effects[ 2 ] = new TextEffect( new RotateTransform( -2 ), null, null, 0, 20 );
            effects[ 3 ] = new TextEffect( new SkewTransform( -5, -5 ), null, null, 0, 20 );
            effects[ 4 ] = new TextEffect( new SkewTransform( 5, 5 ), null, null, 0, 20 );
        }

        public Run generate( string s ) {

            // Upper or lower case?
            switch ( randomInt( 2 ) ) {
                case 0:
                    s = s.ToUpper();
                    break;
                case 1:
                    s = s.ToLower();
                    break;
            }

            // Generate a small box for the text
            Run r = new Run(s);

            // Randomize the values
            r.FontFamily    = fonts[ randomIndex( fonts ) ];
            r.FontSize      = randomInt( MAX_SIZE - MIN_SIZE ) + MIN_SIZE;
            r.FontWeight    = weights[ randomIndex( weights ) ];
            r.Foreground    = colors[ randomIndex( colors ) ];
            r.FontStyle     = styles[ randomIndex( styles, new byte[] { 5, 1, 1 } ) ];
            r.TextDecorations = decorations[ randomIndex( decorations, new byte[] { 30, 1, 1, 1, 1 } ) ];
            r.TextEffects.Add( effects[ randomIndex( effects, new byte[] { 1, 50, 50, 5, 5 } ) ] );

            // Return the textbox
            return r;
        }
    }

    public partial class MainWindow : Window {

        RandomFont randomFont;
        Random random;
        public MainWindow() {

            InitializeComponent();
            textBox.TextChanged += TextBox_TextChanged; // add this after initialization

            randomFont  = new RandomFont();
            random      = new Random();
        }

        private void BtnConvert_Click( object sender, RoutedEventArgs e ) {

            // Get text as string
            TextRange range = new TextRange(textBox.Document.ContentStart, textBox.Document.ContentEnd);
            string text     = range.Text.Trim();

            if ( text.Length == 0 ) {
                return;
            }

            /* Het is leuker als sommige karakters opeenvolgend bij elkaar horen. Maar in de opdracht staat duidelijk dat elke letter anders moet zijn. */
            Boolean combine = false;

            // Setup an empty paragraph
            Paragraph block = new Paragraph();

            // Set a container for some chars
            string s = "";

            for ( int i = 0; i < text.Length; i++ ) {

                s += text[ i ].ToString();

                if ( (byte)text[ i ] < 33 ) {
                    continue; // none printables
                }

                if ( ( combine ) && ( random.Next( 3 ) == 0 ) ) { // randomly break blocks
                    continue;
                }

                block.Inlines.Add( randomFont.generate( s ) );
                s = "";
            }

            if ( s.Length > 0 ) { // this happens if we randomly 'break' blocks. Sometimes a block is just not yet done.
                block.Inlines.Add( randomFont.generate( s ) );
            }

            // Clear the output
            textBox.Document.Blocks.Clear();
            // Display the random result
            textBox.Document.Blocks.Add( block );
        }

        private void TextBox_TextChanged( object sender, TextChangedEventArgs e ) {

            if ( textBox.Document.ContentStart.GetOffsetToPosition( textBox.Document.ContentEnd ) == 4 ) { // 4 == text entered and removed again

                textBox.Document.Blocks.Clear();
            }

        }
    }
}
