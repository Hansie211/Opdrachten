using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Toren_van_Hanoi {
    public class Disk {

        public int size;
        public Rectangle shape = null;
        public TextBlock text = null;

        private int currentStick = -1;
        private int currentPlace = -1;

        public static readonly int height = 30;

        public event MouseButtonEventHandler onClick;

        public static int getSize( int size ) {

            return 80 + size * 20;
        }

        public static Point getPosition( Disk subject, int stickIndex, int place = -1) {

            Point result = new Point();

            if ( place < 0 ) {
                place = subject.currentPlace;
            }

            result.X    = Game.sticks[stickIndex].x - ( subject.shape.Width / 2 );
            result.Y    = Game.baselineHeight - ( place + 1 ) * (subject.shape.Height - 1);

            return result;
        }

        public Disk( int size ) {

            this.size   = size;

            this.shape  = new Rectangle();

            shape.Width = getSize( size );
            shape.Height            = height;
            shape.Fill              = Brushes.Gray;
            shape.Stroke            = Brushes.Black;
            shape.StrokeThickness = 3;

            shape.MouseLeftButtonDown += onClickEvent;

            this.text = new TextBlock();
            text.TextAlignment = System.Windows.TextAlignment.Center;
            text.Text = String.Format( "{0}kg", this.size + 1 );
            text.Width = shape.Width;
            text.Height = height - 4;
            text.FontSize = 16;
            text.Foreground = Brushes.Black;
            text.MouseLeftButtonDown += onClickEvent;
        }

        public void addToCanvas( Canvas canvas ) {

            canvas.Children.Add( shape );
            canvas.Children.Add( text );
        }

        private void onClickEvent( object sender, MouseButtonEventArgs e ) {

            if ( this.onClick == null ) {
                return;
            }

            this.onClick.Invoke( this, e);
        }

        public int getCurrentStick() {

            return currentStick;
        }

        public void moveToStick( int stickIndex ) {

            int placeIndex = Game.sticks[ stickIndex ].getLowestPos();
            if ( placeIndex < 0 ) {
                throw new Exception( "Cannot place disk." );
            }

            Game.sticks[stickIndex].disks[placeIndex] = size;

            if ( ( currentPlace > -1 ) && ( currentStick > -1 ) ) {

                Game.sticks[currentStick].disks[currentPlace] = -1;
            }

            currentPlace = placeIndex;
            currentStick = stickIndex;

            Point p = getPosition( this, stickIndex );

            Canvas.SetTop( this.shape,  p.Y );
            Canvas.SetLeft( this.shape, p.X );

            Canvas.SetTop( this.text, p.Y + 2 );
            Canvas.SetLeft( this.text, p.X );
        }

        public bool isTop() {

            if (( currentPlace < 0 ) || ( currentStick < 0 )) {

                throw new Exception( "Disk is not yet initialized." );
            }

            Stick stick = Game.sticks[ currentStick ];

            for( int i = currentPlace + 1; i < Game.diskCount; i++ ) {

                if ( stick.disks[i] > -1 ) {

                    return false;
                }
            }

            return true;
        }
    }
}
