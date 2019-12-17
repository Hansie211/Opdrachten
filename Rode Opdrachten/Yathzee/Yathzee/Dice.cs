using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;
using Keys = System.Windows.Forms.Keys;
using Control = System.Windows.Forms.Control;

namespace Yathzee {
    class Dice {

        public const int MIN = 1;
        public const int MAX = 6;

        protected int value = MIN;
        public bool held    = false;
        public int index;

        private Image image;
        private static readonly CustomRandom random = new CustomRandom();

        private int baseX, baseY;
        private double transformX, transformY;
        private double rotateAngle;

        public const int WIDTH  = 64;
        public const int HEIGHT = 64;
        public const int BORDER = 12;
        private const int CENTER_X = (WIDTH - BORDER) / 2;
        private const int CENTER_Y = (HEIGHT - BORDER) / 2;

        private static BitmapSource[] sprites = new BitmapSource[ MAX ];

        static Dice() {

            Bitmap baseSprite = new Bitmap( Resources.dice );

            // Size of the sprites
            const int diceW = 64;
            const int diceH = 64;

            const double scaleX = ( WIDTH - BORDER ) / (double)diceW;
            const double scaleY = ( HEIGHT - BORDER ) / (double)diceH;

            // Row determines the color
            const int row = 2;

            for ( int i = 0; i < MAX; i++ ) {

                int x = diceW * i;
                int y = diceH * row;

                // Cut out the bitmap at this x & y
                BitmapSource bmp = Imaging.CreateBitmapSourceFromHBitmap(
                    baseSprite.GetHbitmap(),
                    IntPtr.Zero,
                    new System.Windows.Int32Rect( x, y, diceW, diceH ),
                    BitmapSizeOptions.FromEmptyOptions()
                );

                // Scale this bitmap to fit
                sprites[ i ] = new TransformedBitmap( bmp, new ScaleTransform( scaleX, scaleY ) );
            }

            // Remove the base
            baseSprite.Dispose();
        }

        public Dice( int index, int x, int y ) {

            image                   = new Image();
            image.Cursor            = Cursors.Hand;
            image.RenderTransform   = new RotateTransform( 0, CENTER_X, CENTER_Y ); // setup a basic rotate

            image.MouseLeftButtonDown   += onClickEvent;

#if DEBUG
            image.MouseRightButtonDown  += onRightClickEvent;
#endif

            this.baseX = x + BORDER / 2;
            this.baseY = y + BORDER / 2;
            this.index = index;

            setValue( MIN );
            setHeld( false );
        }

        private void onClickEvent( object sender, MouseButtonEventArgs e ) {

            setHeld( !held );
        }

        private void onRightClickEvent( object sender, MouseButtonEventArgs e ) {

            if ( ( Control.ModifierKeys & Keys.Shift ) != Keys.Shift ) {

                return;
            }

            setValue( ( getValue() % MAX ) + 1 );

        }

        private void setAngle( double angle ) {
            ( (RotateTransform)image.RenderTransform ).Angle = angle;
        }

        private int getRandomInt( int min, int max ) {

            return random.Random( min, max );
        }

        public void setHeld( bool hold ) {

            held = hold;

            if ( hold ) {

                int slot = HoldManager.takeSlot( this );
                moveToHoldPos( slot );

            } else {

                HoldManager.freeSlot( this );
                moveToFieldPos();
            }
        }

        public void moveToFieldPos() {

            Canvas.SetLeft( image, this.transformX );
            Canvas.SetTop( image, this.transformY );
            setAngle( rotateAngle );
        }

        public void moveToHoldPos( int pos ) {

            Canvas.SetLeft( image, HoldManager.pos[ pos ].X + BORDER / 2 );
            Canvas.SetTop( image, HoldManager.pos[ pos ].Y + BORDER / 2 );
            setAngle( 0 );
        }

        public int getValue() {

            return this.value;
        }

        public void setValue( int x ) {

            value = x;
            image.Source = sprites[ x - 1 ]; // show the right sprite
        }

        public void roll() {

            if ( held ) {
                return;
            }

            setValue( getRandomInt( MIN, MAX ) );

            const int DELTA = 40;
            transformX  = baseX + getRandomInt( -DELTA, DELTA );
            transformY  = baseY + getRandomInt( -DELTA, DELTA );
            rotateAngle = getRandomInt( -180, 180 );

            moveToFieldPos();
        }
        public void addToCanvas( Canvas canvas ) {

            canvas.Children.Add( image );
        }
    }
}
