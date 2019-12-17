using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;
using Yathzee.Scores;
using Brushes = System.Windows.Media.Brushes;
using System.Windows.Input;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Yathzee {
    abstract class BaseScore {

        protected bool open = true;
        protected int value = 0;

        protected string name;

        private Border border;
        private Rectangle area;
        private TextBlock textName;
        private TextBlock textScore;

        public event MouseButtonEventHandler onClick;

        public static readonly SolidColorBrush BackgroundColor = new SolidColorBrush( Color.FromRgb( 56, 173, 169 ) );

        public BaseScore( string name ) {

            this.name = name;

            border = new Border();
            border.Width             = Scorecard.SCOREWIDTH;
            border.Height            = Scorecard.SCOREHEIGHT;
            border.BorderBrush       = BackgroundColor;
            border.BorderThickness   = new System.Windows.Thickness( 0, 0, 0, 3 );

            area  = new Rectangle();
            area.Width              = Scorecard.SCOREWIDTH;
            area.Height             = Scorecard.SCOREHEIGHT;

            textName            = new TextBlock();
            textName.Text       = getName();
            textName.FontSize   = 25;
            textName.FontWeight = System.Windows.FontWeights.Bold;

            textScore           = new TextBlock();
            textScore.Text      = getValue().ToString();
            textScore.FontSize  = 25;
            textScore.FontWeight = System.Windows.FontWeights.Bold;
            textScore.FontFamily = new FontFamily( "Bahnschrift Light" );


            border.MouseLeftButtonDown      += onClickEvent;
            area.MouseLeftButtonDown        += onClickEvent;
            textName.MouseLeftButtonDown    += onClickEvent;
            textScore.MouseLeftButtonDown   += onClickEvent;

            setOpen( true );
        }

        abstract public int potentailValue();

        private void onClickEvent( object sender, MouseButtonEventArgs e ) {

            if ( !this.isOpen() ) {

                return;
            }

            if ( this.onClick == null ) {
                return;
            }

            this.onClick.Invoke( this, e );
        }

        public void activate() {

            this.setValue( potentailValue() );
        }

        private void setOpen( bool open ) {

            this.textName.Foreground    = ( open ) ? Brushes.White : BackgroundColor;
            this.textName.Cursor        = ( open ) ? Cursors.Hand : Cursors.Arrow;

            this.textScore.Foreground   = textName.Foreground;
            this.textScore.Cursor       = textName.Cursor;

            this.area.Cursor    = textName.Cursor;
            this.border.Cursor  = textName.Cursor;

            this.open = open;
        }

        public void updatePreview() {

            if ( !open ) {
                return;
            }

            textScore.Text = potentailValue().ToString();
        }

        protected void setValue( int value ) {

            this.value      = value;
            textScore.Text  = this.value.ToString();

            setOpen( false );
        }

        public bool isOpen() {

            return this.open;
        }

        public int getValue() {
            return this.value;
        }

        public void reset() {

            setValue( 0 );
            setOpen( true );
        }

        public string getName() {

            return name;
        }

        private void addObjectToCanvas( Canvas canvas, System.Windows.UIElement element, double x, double y ) {

            Canvas.SetLeft( element, x );
            Canvas.SetTop( element, y );
            canvas.Children.Add( element );
        }

        public void addToCanvas( Canvas canvas, double left, double top ) {

            addObjectToCanvas( canvas, area, left, top );
            addObjectToCanvas( canvas, border, left, top );
            addObjectToCanvas( canvas, textName, left + 8, top + 12 );
            addObjectToCanvas( canvas, textScore, left + 250, top + 12 );

            area.Fill = canvas.Background;
        }
    }
}
