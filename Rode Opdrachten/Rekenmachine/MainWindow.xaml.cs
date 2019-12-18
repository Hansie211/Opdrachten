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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Button = System.Windows.Controls.Button;
using Control = System.Windows.Forms.Control;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

// Upgrade buttons, add a PerformClick
namespace System.Windows.Controls {
    public static class MyExt {
        public static void PerformClick( this Button btn ) {
            btn.RaiseEvent( new RoutedEventArgs( Button.ClickEvent ) );
        }
    }
}

namespace Opdracht1 {

    struct Stack {

        public Double value;
        public Boolean isSet;
        public Byte op;
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        private Stack stack;
        private Double memory;

        private Boolean newCalculation = true;
        private String previousText;

        private const int OP_PLUS   = 1;
        private const int OP_MINUS  = 2;
        private const int OP_MULT   = 3;
        private const int OP_DIV    = 4;

        public MainWindow() {
            InitializeComponent();

            stack.isSet = false;
            previousText = edtMain.Text;
            edtMain.TextChanged += EdtMain_TextChanged;

            //edtMain.Focus();
        }

        private void clearBox() {

            if ( newCalculation ) {

                edtMain.Clear();
                newCalculation = false;
            }

        }

        private void clearStack() {

            stack.isSet         = false;
            labelStack.Content  = "";
            edtMain.Clear();
        }

        private Boolean isShiftDown() {
            return ( Control.ModifierKeys == Keys.Shift );
        }

        private Double getCurrentValue() {

            Double r;
            if ( !Double.TryParse(edtMain.Text, out r) ) {
                return 0;
                //throw new Exception( String.Format("'{0}' is geen getal!", edtMain.Text));
            }

            return r;
        }

        private void BtnKeypad_Click(object Sender, RoutedEventArgs e) {

            clearBox();

            edtMain.Text += ((Button)Sender).Tag.ToString();
        }

        private Boolean isNumeric( Key k ) {

            if ( isShiftDown() ) {
                return false;
            }

            if ( (k == Key.Decimal) || (k == Key.OemPeriod) ) {

                return true;
                //return edtMain.Text.IndexOf(".") == -1;
            }

            if ( (k >= Key.D0) && (k <= Key.D9) ) {
                return true;
            }

            if ( (k >= Key.NumPad0) && (k <= Key.NumPad9) ) {
                return true;
            }


            return false;
        }

        private Boolean handleSymbolKeyPress( Key k) {

            Boolean shiftDown = isShiftDown();

            // Check for '+'
            if ( (k == Key.OemPlus && shiftDown) || (k == Key.Add) ) {

                btnPlus.PerformClick();
                return true;
            }

            // Check for '='
            if ( (k == Key.OemPlus && !shiftDown ) || (k == Key.Return) ) {

                btnEqual.PerformClick();
                return true;
            }

            // Check for '-'
            if ( (k == Key.OemMinus && !shiftDown) || (k == Key.Subtract) ) {

                btnMinus.PerformClick();
                return true;
            }

            // Check for '/'
            if ( (k == Key.OemQuestion && shiftDown) || (k == Key.Divide) ) {

                btnDivide.PerformClick();
                return true;
            }

            // Check for '*'
            if ( (k == Key.Multiply) || (k == Key.D8 && shiftDown) ) {

                btnMult.PerformClick();
                return true;
            }

            // Check for 'Del'
            if ( k == Key.Delete ) {

                btnClearCurrent.PerformClick();
                return true;
            }

            // Check for 'C'
            if ( k == Key.C ) {

                btnClearAll.PerformClick();
                return true;
            }

            // No matches
            return false;
        }

        private void setOperator( Byte op ) {
            if ( stack.isSet ) {
                btnEqual.PerformClick();
            }

            stack.isSet = true;
            stack.op    = op;
            stack.value = getCurrentValue();

            labelStack.Content = stack.value.ToString();

            switch ( op ) {
                default:
                case OP_PLUS:
                    labelStack.Content += " + "; break;
                case OP_MINUS:
                    labelStack.Content += " - "; break;
                case OP_MULT:
                    labelStack.Content += " * "; break;
                case OP_DIV:
                    labelStack.Content += " / "; break;
            }

            edtMain.Clear();
        }

        private void BtnPlus_Click( object sender, RoutedEventArgs e ) {

            setOperator( OP_PLUS );
        }

        private void BtnMinus_Click( object sender, RoutedEventArgs e ) {

            setOperator( OP_MINUS );
        }

        private void BtnMult_Click( object sender, RoutedEventArgs e ) {

            setOperator( OP_MULT );
        }

        private void BtnDivide_Click( object sender, RoutedEventArgs e ) {

            setOperator( OP_DIV );
        }

        private void BtnEqual_Click( object sender, RoutedEventArgs e ) {

            if ( !stack.isSet ) {
                return;
            }

            Double current = getCurrentValue();
            Double outcome;

            switch( stack.op ) {

                default:
                case OP_PLUS:
                    outcome = stack.value + current; break;

                case OP_MINUS:
                    outcome = stack.value - current; break;

                case OP_MULT:
                    outcome = stack.value * current; break;

                case OP_DIV:
                    outcome = stack.value / current; break;

            }

            labelStack.Content += current.ToString();
            edtMain.Text        = outcome.ToString();

            stack.isSet     = false;
            newCalculation  = true;
        }

        private void BtnClearAll_Click( object sender, RoutedEventArgs e ) {
            
            clearStack();
        }

        private void BtnClearCurrent_Click( object sender, RoutedEventArgs e ) {
            
            edtMain.Clear();
        }

        private void BtnPercent_Click( object sender, RoutedEventArgs e ) {
            
            edtMain.Text = (getCurrentValue() / 100).ToString();
        }

        private void BtnPosNeg_Click( object sender, RoutedEventArgs e ) {

            edtMain.Text = (getCurrentValue() * -1).ToString();
        }

        private void BtnMemSet_Click( object sender, RoutedEventArgs e ) {

            memory = getCurrentValue();
            btnMemRecall.IsEnabled = true;
            btnMemClear.IsEnabled = true;
        }

        private void BtnMemRecall_Click( object sender, RoutedEventArgs e ) {

            edtMain.Text = memory.ToString();
        }

        private void BtnMemClear_Click( object sender, RoutedEventArgs e ) {

            btnMemRecall.IsEnabled  = false;
            btnMemClear.IsEnabled   = false;
        }

        private void BtnExponent_Click( object sender, RoutedEventArgs e ) {

            edtMain.Text    = (getCurrentValue() * getCurrentValue()).ToString();
            newCalculation  = true;
        }

        private void Window_PreviewKeyDown( object sender, KeyEventArgs e ) {

            e.Handled = true;

            // Check for symbol presses
            if ( handleSymbolKeyPress( e.Key ) ) {

                return;
            }

            // Check for numeric values
            if ( isNumeric( e.Key ) ) {

                char c;
                clearBox();

                switch ( e.Key ) {

                    default:
                    case Key.D0:
                    case Key.NumPad0:
                        c = '0';
                        break;

                    case Key.D1:
                    case Key.NumPad1:
                        c = '1';
                        break;

                    case Key.D2:
                    case Key.NumPad2:
                        c = '2';
                        break;

                    case Key.D3:
                    case Key.NumPad3:
                        c = '3';
                        break;

                    case Key.D4:
                    case Key.NumPad4:
                        c = '4';
                        break;

                    case Key.D5:
                    case Key.NumPad5:
                        c = '5';
                        break;

                    case Key.D6:
                    case Key.NumPad6:
                        c = '6';
                        break;

                    case Key.D7:
                    case Key.NumPad7:
                        c = '7';
                        break;

                    case Key.D8:
                    case Key.NumPad8:
                        c = '8';
                        break;

                    case Key.D9:
                    case Key.NumPad9:
                        c = '9';
                        break;

                    case Key.OemPeriod:
                    case Key.Decimal:
                        c = '.';
                        break;
                }

                edtMain.Text += c;
                return;
            }

            // Check for backspace
            if (e.Key == Key.Back ) {

                if ( edtMain.Text.Length > 0 ) { 
                    edtMain.Text = edtMain.Text.Substring(0, edtMain.Text.Length - 1);
                }

                return;
            }

            e.Handled = false;
        }

        private void BtnDecimal_Click( object sender, RoutedEventArgs e ) {

            edtMain.Text = edtMain.Text + ".";
        }

        private void EdtMain_TextChanged( object sender, TextChangedEventArgs e ) {

            
            if ( edtMain.Text.Length == 0 ) {
                return;
            }

            if (edtMain.Text == "." ) {

                edtMain.Text = "0.";
                return;
            }

            Double dummy;
            if ( !Double.TryParse( edtMain.Text, out dummy ) ) {

                edtMain.Text = previousText;
            } else {

                previousText = edtMain.Text;
            }
            
            edtMain.CaretIndex  = edtMain.Text.Length;
            newCalculation = false;

        }

    }
}
