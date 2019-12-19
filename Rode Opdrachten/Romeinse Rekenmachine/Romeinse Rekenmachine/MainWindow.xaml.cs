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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Romeinse_Rekenmachine {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    struct SumPart {
        public uint value;
        public byte op;
    }

    public class UI {
        public static BitmapSource Backspace { get; set; } = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                           ( (System.Drawing.Bitmap)Romeinse_Rekenmachine.Resources.backspace ).GetHbitmap(),
                           IntPtr.Zero,
                           System.Windows.Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions() );
    }

    public partial class MainWindow : Window {


        const int MAX_NUM = 4000;
        const int MIN_NUM = 0;

        const byte OP_PLUS  = 0;
        const byte OP_MINUS = 1;
        const byte OP_MULT  = 2;
        const byte OP_DIV   = 3;
        const byte OP_NONE  = 0xFF;

        char[] operators = { '*', '/', '+', '-' };

        Boolean newCalculation = true;

        Regex allowedChars;
        Dictionary<char, uint> numerals;

        private Boolean canSubtract( char n ) {

            switch ( n ) {
                case 'I':
                case 'X':
                case 'C':
                case 'M':
                    return true;
                default:
                    return false;
            }

        }

        private int getIndexOfLower( string num, char n ) {

            uint v = numerals[n];

            for ( int i = 0; i < num.Length; i++ ) {
                if ( numerals[ num[ i ] ] < v ) {
                    return i;
                }
            }

            return -1;
        }

        private int getWrongNumeralCount( string num, char n ) {
            int result = 0;

            for ( int i = 0; i < num.Length; i++ ) {

                if ( num[ i ] == n ) {
                    continue;
                }

                result++;
            }

            return result;
        }

        private uint numeralToInt( string num ) {

            // Easy
            if ( num.Length == 0 ) {
                return 0;
            }

            string[] breakdown = new string[numerals.Count];

            // Break down the number into chunks
            int j = numerals.Count - 1;
            while ( j > -1 ) {

                char n = numerals.ElementAt(j).Key;
                int  p = num.LastIndexOf(n);

                if ( p > -1 ) {

                    breakdown[ j ] = num.Substring( 0, p + 1 );
                    num = num.Substring( p + 1 );
                } else {
                    breakdown[ j ] = "";
                }

                j--;
            }

            // Start calculating !
            uint result = 0;

            for ( int i = 0; i < numerals.Count; i++ ) {

                char n = numerals.ElementAt(i).Key;

                // Check if a chunk contains a value lower number than itself. This is allowed, but only under special circumstances
                int l = getIndexOfLower(breakdown[i], n);
                if ( l > -1 ) {


                    if ( getWrongNumeralCount( breakdown[ i ], n ) > 1 ) {

                        // This is just wron formatting, eg: VIX -> XVI
                        throw new Exception( String.Format( "Opbouwfout bij {0}. {1}", breakdown[ i ], "De volgorde is incorrect.\n\nUitleg: Romeinse cijfers gaan van groot naar klein.\nDus XVI (16) mag maar VIX niet." ) );
                    }

                    // It should be the second last position
                    if ( l != breakdown[ i ].Length - 2 ) {

                        // Wrong place for subtraction, eg: XIXX -> XXIX
                        throw new Exception( String.Format( "Opbouwfout bij {0}. {1}", breakdown[ i ], "De getalsvolgorde is incorrect.\n\nUitleg: Een kleiner getal mag alleen voor een groter getal staan als deze de laatste in zijn reeks is.\nDus XXIX (29) mag maar XIXX niet." ) );
                    }

                    // It should _NOT_ be a 'half' value, like 5 or 50
                    if ( !canSubtract( breakdown[ i ][ l ] ) ) {

                        // Wrong subtractor, eg: VX -> V
                        throw new Exception( String.Format( "Opbouwfout bij {0}. {1}", breakdown[ i ], "Verkeerde subtractor.\n\nUitleg: Alleen I, X, C en M mogen als subtractor worden gebruikt.\nBijvoorbeeld: VX moet V (5) zijn, en VL moet XLV (45) zijn. IX (9) mag wel." ) );
                    }

                    // Check the chunk size
                    if ( ( !canSubtract( n ) ) && ( breakdown[ i ].Length > 2 ) ) {

                        // Wrong numeral choice, eg: VIV -> IX
                        throw new Exception( String.Format( "Opbouwfout bij {0}. {1}", breakdown[ i ], "Verkeerde herhaling.\n\nUitleg: Dit getal kan korter worden geschreven.\nBijvoorbeeld: VIV moet IX (9) zijn." ) );
                    }

                    if ( ( canSubtract( n ) ) && ( breakdown[ i ].Length > 5 ) ) {

                        // Wrong numeral choice, eg: XXXXVX -> VL
                        throw new Exception( String.Format( "Opbouwfout bij {0}. {1}", breakdown[ i ], "Verkeerde herhaling.\n\nUitleg: Dit getal kan korter worden geschreven.\nBijvoorbeeld: XXXXVX moet VL (45) zijn." ) );
                    }

                    // All good, at it to the result!

                    // XXIX = (3 * X) - I = 29
                    uint sub = numerals[breakdown[i][l]];

                    result += (uint)( ( breakdown[ i ].Length - 1 ) * numerals[ n ] );
                    result -= sub;
                } else {

                    // Check the chunk size
                    if ( ( !canSubtract( n ) ) && ( breakdown[ i ].Length > 1 ) ) {

                        // Wrong numeral choice, eg: VV -> X
                        throw new Exception( String.Format( "Opbouwfout bij {0}. {1}", breakdown[ i ], "Verkeerde herhaling.\n\nUitleg: Dit getal kan korter worden geschreven.\nBijvoorbeeld: VV moet X (10) zijn." ) );
                    }

                    if ( ( canSubtract( n ) ) && ( breakdown[ i ].Length > 3 ) ) {

                        // Wrong numeral choice, eg: IIII -> IV
                        throw new Exception( String.Format( "Opbouwfout bij {0}. {1}", breakdown[ i ], "Verkeerde herhaling.\n\nUitleg: Dit getal kan korter worden geschreven.\nBijvoorbeeld: IIII moet IV (4) zijn." ) );
                    }

                    // All good, at it to the result!

                    // MMM = 3 * 1000
                    result += (uint)( breakdown[ i ].Length * numerals[ n ] );
                }
            }

            return result;
        }

        private string intToNumeral( uint dec ) {

            string result = "";
            if ( dec > MAX_NUM ) {
                return ">MMMM";
            }

            for ( int i = numerals.Count - 1; i > -1; i-- ) {

                char n = numerals.ElementAt(i).Key;
                uint v = numerals[n];

                while ( dec >= v ) {

                    dec -= v;
                    result += n.ToString();
                }

                for ( int j = i - 1; j > -1; j-- ) {

                    char n2 = numerals.ElementAt(j).Key;

                    if ( !canSubtract( n2 ) ) {
                        continue;
                    }

                    uint v2 = v - numerals[n2];
                    if ( dec >= v2 ) {

                        dec -= v2;
                        result += n2.ToString() + n.ToString();

                        break;
                    }
                }

                if ( dec == 0 ) {
                    break;
                }
            }

            return result;
        }

        private void check() {
            for ( uint i = MIN_NUM; i < MAX_NUM; i++ ) {

                string num  = intToNumeral(i);
                uint dec    = numeralToInt(num);

                if ( dec != i ) {
                    throw new Exception( String.Format( "Error decoding {0} ({1}). Result is {2}", num, i, dec ) );
                }

                Console.WriteLine( "{0} -> {1} -> {2}", i, num, dec );
            }
        }

        public MainWindow() {
            InitializeComponent();

            // Setup the numerals
            numerals = new Dictionary<char, uint>();
            numerals.Add( 'I', 1 );
            numerals.Add( 'V', 5 );
            numerals.Add( 'X', 10 );
            numerals.Add( 'L', 50 );
            numerals.Add( 'C', 100 );
            numerals.Add( 'D', 500 );
            numerals.Add( 'M', 1000 );

            allowedChars = new Regex( "^[IVXLCDM+\\-*/() ]*$" );

            // check();            
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {

            if ( newCalculation ) {
                edtSum.Clear();
                newCalculation = false;
            }

            edtSum.Text += ( (Button)sender ).Content.ToString();
        }

        private Boolean isNumeral( char c ) {

            return numerals.ContainsKey( c );
        }

        private int getCharCount( char needle, string haystack ) {

            return haystack.Split( needle ).Length;
        }

        private Boolean isOperator( char c, Boolean allowBrackets = true ) {

            if ( operators.Contains( c ) ) {
                return true;
            }

            switch ( c ) {
                case '(':
                case ')':
                    return allowBrackets;
            }

            return false;
        }

        private Boolean checkSyntax( ref string haystack ) {

            if ( !allowedChars.IsMatch( haystack ) ) {
                // Illegal characters
                throw new Exception( "Sommige karakters worden niet herkent." );
            }

            // Filter double spaces
            haystack = new Regex( "[ ]{2,}" ).Replace( haystack, " " );

            if ( getCharCount( '(', haystack ) != getCharCount( ')', haystack ) ) {
                // Brackets dont match
                throw new Exception( "Haakjes matchen niet!" );
            }


            if ( !isNumeral( haystack[ 0 ] ) && ( haystack[ 0 ] != '(' ) ) {
                // Cannot start with operator
                throw new Exception( "Som kan niet beginnen met een opdracht!" );
            }

            if ( !isNumeral( haystack[ haystack.Length - 1 ] ) && ( haystack[ haystack.Length - 1 ] != ')' ) ) {
                // Cannot end with operator
                throw new Exception( "Som kan niet eindigen met een opdracht!" );
            }

            int i;

            for ( i = 1; i < haystack.Length - 1; i++ ) { // haystack should be trimmed, so first & last are never ' '

                if ( haystack[ i ] == ' ' ) {

                    if ( !isOperator( haystack[ i-1 ] ) && !isOperator( haystack[ i+1 ] ) ) {
                        // Spaces must be paired with operators somehow, cannot be paired with only numerals
                        throw new Exception( "Spatie is verkeerd geplaatst!" );
                    }
                }

            }

            // Remove all spaces
            haystack = haystack.Replace( " ", "" );

            for ( i = 1; i < haystack.Length - 1; i++ ) { // it does not start nor end with an operator

                if ( isOperator( haystack[ i ], false ) ) {

                    if ( isOperator( haystack[ i+1 ], false ) || isOperator( haystack[ i-1 ], false ) ) {
                        // Cannot have double operators (++)
                        throw new Exception( "Dubbele opdrachten zijn niet toegestaan!" );
                    }

                }

                if ( haystack[ i ] == '(' ) {

                    if ( !isOperator( haystack[ i - 1 ] ) ) {

                        if ( haystack[ i - 1 ] != '(' ) {
                            // V(V)
                            throw new Exception( "'(' moet verbonden zijn met een operator!" );
                        }
                    }

                    if ( isOperator( haystack[ i + 1 ], false ) ) {
                        // (+V)
                        throw new Exception( "'(' mag niet worden gevold door een opdracht!" );
                    }

                } else if ( haystack[ i ] == ')' ) {

                    if ( isOperator( haystack[ i - 1 ] ) ) {
                        // (V+)
                        throw new Exception( "')' mag niet zijn verbonden met een opdracht!" );
                    }

                    if ( !isOperator( haystack[ i + 1 ], false ) ) {

                        if ( haystack[ i + 1 ] != ')' ) {
                            // (V)V
                            throw new Exception( "')' moet worden gevold door een opdracht!" );
                        }
                    }

                }

            }


            return true;
        }

        private string getTextBetweenBrackets( string text, int start = 0 ) {

            int level = 0;

            for ( int i = start; i < text.Length; i++ ) {

                if ( text[ i ] == '(' ) {

                    level++;

                    for ( int j = i + 1; j < text.Length; j++ ) {

                        if ( text[ j ] == '(' ) {
                            level++;
                            continue;
                        }

                        if ( text[ j ] != ')' ) {
                            continue;
                        }

                        level--;

                        if ( level > 0 ) {
                            continue;
                        }

                        return text.Substring( i + 1, ( j - i ) - 1 );
                    }



                }
            }

            return "";
        }

        private byte toOperator( char c ) {

            switch ( c ) {
                case '+':
                    return OP_PLUS;
                case '-':
                    return OP_MINUS;
                case '*':
                    return OP_MULT;
                case '/':
                    return OP_DIV;
                default:
                    throw new Exception( String.Format( "Illegaal karakter '{0}'", c ) );
            }

        }

        private void calculateParts( ref SumPart[] parts, byte op ) {

            uint outcome;

            int i;

            // Move every part 'foreward'
            for ( i = 0; i < parts.Length - 1; i++ ) {

                if ( parts[ i ].op != op ) {
                    continue;
                }

                switch ( op ) {

                    case OP_PLUS:
                        outcome = parts[ i ].value + parts[ i + 1 ].value;
                        break;
                    case OP_MINUS:

                        if ( parts[ i ].value < parts[ i + 1 ].value ) { // cannot do less than zero
                            outcome = 0;
                            break;
                        }

                        outcome = parts[ i ].value - parts[ i + 1 ].value;
                        break;
                    case OP_MULT:
                        outcome = parts[ i ].value * parts[ i + 1 ].value;
                        break;
                    case OP_DIV:
                        outcome = parts[ i ].value / parts[ i + 1 ].value;
                        break;
                    default:
                        throw new Exception( String.Format( "Illegaal instructie '{0}'", op ) );
                }

                parts[ i ].op         = OP_NONE; // mark as 'done'
                parts[ i+1 ].value    = outcome;
            }

            // Trim the list
            i = 0;
            for ( int j = 0; j < parts.Length - 1; j++ ) {

                if ( parts[ j ].op == OP_NONE ) {
                    continue;
                }

                parts[ i ] = parts[ j ];
                i++;
            }

            // Always copy the end result
            parts[ i ] = parts[ parts.Length - 1 ];
            // Trim
            Array.Resize( ref parts, i + 1 );
        }

        private uint calculate( string sum ) {

            // Max 100 subparts
            SumPart[] parts = new SumPart[ 100 ];
            int partIndex   = -1;

            while ( sum.Length > 0 ) {

                partIndex++;

                if ( partIndex > parts.Length - 1 ) {
                    throw new Exception( "Te veel argumenten!" );
                }

                if ( sum[ 0 ] == '(' ) {

                    string bracketPart = getTextBetweenBrackets( sum );
                    int bracketPartLen = bracketPart.Length + 2; // '(' && ')'
                    parts[ partIndex ].value = calculate( bracketPart );

                    if ( bracketPartLen >= sum.Length ) { // brackets where last part
                        parts[ partIndex ].op = OP_NONE;
                        break;
                    }
                    parts[ partIndex ].op = toOperator( sum[ bracketPartLen ] );

                    sum = sum.Substring( bracketPartLen + 1 );

                    continue;
                }

                Boolean operatorFound = false;

                for ( int i = 0; i < sum.Length; i++ ) {

                    if ( !isOperator( sum[ i ] ) ) {
                        continue;
                    }

                    if ( isOperator( sum[ i ], false ) ) {

                        parts[ partIndex ].value    = numeralToInt( sum.Substring( 0, i ) );
                        parts[ partIndex ].op       = toOperator( sum[ i ] );

                        sum = sum.Substring( i + 1 );

                        operatorFound = true;
                        break;
                    }

                }

                if ( operatorFound ) {
                    continue;
                }

                // End of sum
                parts[ partIndex ].value = numeralToInt( sum );
                parts[ partIndex ].op    = OP_NONE;
                break;
            }

            // Trim
            Array.Resize( ref parts, partIndex + 1 );

            calculateParts( ref parts, OP_MULT );
            calculateParts( ref parts, OP_DIV );
            calculateParts( ref parts, OP_PLUS );
            calculateParts( ref parts, OP_MINUS );

            return parts[ 0 ].value;
        }

        private void BtnEqual_Click( object sender, RoutedEventArgs e ) {

            string currentText = edtSum.Text.Trim();

            if ( currentText.Length == 0 ) {
                return;
            }

            currentText = currentText.ToUpper();

            try {
                // Verify the sum syntax
                checkSyntax( ref currentText );

                // Verify numerals
                string[] sumNumerals    = currentText.Split( new char[] { '+', '-', '*', '/', '(', ')' } );
                for ( int i = 0; i < sumNumerals.Length; i++ ) {
                    numeralToInt( sumNumerals[ i ] );
                }

            } catch ( Exception exp ) {
                MessageBox.Show( exp.Message );
                return;
            }

            uint outcome        = calculate( currentText );
            lblOutcome.ToolTip  = outcome.ToString();
            lblOutcome.Content  = intToNumeral( outcome );

            newCalculation      = true;
        }

        private void BtnClear_Click( object sender, RoutedEventArgs e ) {

            edtSum.Clear();
        }

        private void BtnBack_Click( object sender, RoutedEventArgs e ) {

            if ( edtSum.Text.Length == 0 ) {
                return;
            }

            edtSum.Text = edtSum.Text.Substring( 0, edtSum.Text.Length - 1 );
        }
    }
}
