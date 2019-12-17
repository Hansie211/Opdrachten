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
    /// Interaction logic for window_opdr_5.xaml
    /// </summary>
    /// 

    using static Helper;

    public partial class WinOpdr5 : OpdrWindow {

        private const int ControlHeight = 25;
        private const int LineHeight = ControlHeight + 10;
        private const int EditWidth = 150;
        private const int ControlMargin = 10;

        private readonly SolidColorBrush CancelBackground = (SolidColorBrush)new BrushConverter().ConvertFromString("#e74c3c");

        public static new string Opdracht() {
            return "Kinderbijslag";
        }

        public static new OpdrWindow newInstance() {

            return new WinOpdr5();
        }

        private struct Children {

            public TextBox edtName;
            public DatePicker date;
            public Button btnDelete;
        }

        Children[] children;

        public override void reset() {

            children = new Children[0];
            newChild(); // Generate first placeholder

        }

        private void updateIndex( int index ) {

            int top = index * LineHeight;

            children[index].edtName.Margin = new Thickness( ControlMargin, top, 0, 0 );

            children[index].date.Margin = new Thickness( ControlMargin + EditWidth + ControlMargin, top, 0, 0 );

            children[index].btnDelete.Margin = new Thickness( 0, top, ControlMargin, 0 );
            children[index].btnDelete.Tag = index;

        }

        private void newChild() {

            int i = children.Length;
            Array.Resize( ref children, i + 1);

            // Create the controls
            children[i].edtName     = new TextBox();
            children[i].date        = new DatePicker();
            children[i].btnDelete   = new Button();

            int top = i * LineHeight;

            // Add the controls to the grid
            gridChildren.Children.Add( children[i].edtName );
            gridChildren.Children.Add( children[i].date );
            gridChildren.Children.Add( children[i].btnDelete );

            // Update the position and data
            updateIndex(i);

            // Set some constant properties
            children[i].edtName.HorizontalAlignment     = HorizontalAlignment.Left;
            children[i].edtName.VerticalAlignment       = VerticalAlignment.Top;
            children[i].date.HorizontalAlignment        = HorizontalAlignment.Left;
            children[i].date.VerticalAlignment          = VerticalAlignment.Top;
            children[i].btnDelete.HorizontalAlignment   = HorizontalAlignment.Right;
            children[i].btnDelete.VerticalAlignment     = VerticalAlignment.Top;

            children[i].date.SelectedDate   = DateTime.Now;
            children[i].date.DisplayDateEnd = children[i].date.SelectedDate.Value;
            children[i].date.DisplayDate    = children[i].date.SelectedDate.Value;

            children[i].btnDelete.ToolTip   = "Verwijderen";
            children[i].btnDelete.Content   = "X";
            children[i].btnDelete.Width     = ControlHeight;
            children[i].btnDelete.Height    = ControlHeight;
            children[i].btnDelete.Click += BtnDelete_Click;

            children[i].btnDelete.BorderBrush   = Brushes.Transparent;
            children[i].btnDelete.Foreground    = Brushes.White;
            children[i].btnDelete.Background    = CancelBackground;
            children[i].btnDelete.FontWeight    = FontWeights.Bold;

            children[i].edtName.Width = EditWidth;
            children[i].edtName.Height = ControlHeight;

            // Expand the grid
            gridChildren.Height += LineHeight;

            // Scroll down
            scrollChildren.ScrollToVerticalOffset( gridChildren.Height );
        }

        private void removeChild( int index ) {

            if (( index < 0 ) || ( index > children.Length - 1 )){
                return;
            }

            // Remove the event handler
            children[index].btnDelete.Click -= BtnDelete_Click;

            // Remove the controls from the grid
            gridChildren.Children.Remove( children[index].edtName );
            gridChildren.Children.Remove( children[index].date );
            gridChildren.Children.Remove( children[index].btnDelete );

            // Remove the item from the array
            splice( ref children, index );

            // Update the positions
            for( int i = 0; i < children.Length; i++ ) {

                updateIndex(i);
            }

            // Shrink the grid
            gridChildren.Height -= LineHeight;
        }

        private void BtnDelete_Click( object sender, RoutedEventArgs e ) {

            Button btn = (Button)sender;

            int i = (int)btn.Tag;
            removeChild( i );
        }

        public WinOpdr5() {

            InitializeComponent();

        }

        private void BtnNewPerson_Click( object sender, RoutedEventArgs e ) {

            newChild();
        }

        private void BtnCalc_Click( object sender, RoutedEventArgs e ) {

            int totalChildren = 0;

            int younger = 0;
            int middle  = 0;

            for( int i = 0; i < children.Length; i++ ) {

                // Is set?
                if ( ( children[i].edtName.Text.Length == 0 ) || ( children[i].date.SelectedDate == null ) ){
                    continue;
                }

                // Get age
                int _age = age( children[i].date.SelectedDate.Value, datePeildatum.SelectedDate.Value );

                // Is this a child?
                if ( _age > 18 ) {
                    continue;
                }

                totalChildren++;

                if ( _age < 12 ) {
                    younger++;
                } else {
                    middle++;
                }

            }

            if ( totalChildren == 0 ) {
                MessageBox.Show("Voer minimaal 1 kind in.");
                return;
            }

            WindowReceipt w = new WindowReceipt( Opdracht() );

            w.addToReceipt("Kind 0 - 11", younger, 150);
            w.addToReceipt("Kind 12 - 18", middle, 235);

            switch( totalChildren ) {
                case 0:
                case 1:
                case 2: 
                break; // skip these numbers

                case 3:
                case 4:
                    w.setCharge("Opslag", 0.02);
                break;

                case 5:
                    w.setCharge("Opslag", 0.03);
                break;

                default:
                    w.setCharge("Opslag", 0.035);
                break;                
            }

            w.displayReceipt();
        }
    }
}
