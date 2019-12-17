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
    class VirtualDisk {

        public Rectangle[] shapes;
        public Disk subject;

        public event MouseButtonEventHandler onClick;

        private int clickedItem;

        public VirtualDisk( Canvas canvas ) {

            SolidColorBrush ForegroundColor = new SolidColorBrush( Colors.Green );
            ForegroundColor.Opacity = 0.5;

            shapes = new Rectangle[Game.stickCount];
            for( int i = 0; i < shapes.Length; i++ ) {

                shapes[i] = new Rectangle();

                shapes[i].Visibility = Visibility.Hidden;
                shapes[i].Fill      = ForegroundColor;
                shapes[i].Stroke    = Brushes.Black;
                Canvas.SetZIndex( shapes[i], 10 );

                shapes[i].Tag = i;

                shapes[i].MouseLeftButtonDown += onClickEvent;

                canvas.Children.Add( shapes[i]);
            }
        }

        private void onClickEvent( object sender, MouseButtonEventArgs e ) {

            if ( this.onClick == null ) {
                return;
            }

            clickedItem = (int)( (Rectangle)sender ).Tag;

            this.onClick.Invoke( this , e );
        }

        public int getClickedItem() {

            return clickedItem;
        }

        public void Show( Disk subject ) {

            this.subject = subject;

            for ( int i = 0; i < Game.stickCount; i++ ) {

                if ( i == this.subject.getCurrentStick() ) {

                    continue;
                }

                if ( !Game.sticks[i].canAdd( this.subject.size ) ) {

                    continue;
                }

                shapes[i].Width = this.subject.shape.Width;
                shapes[i].Height = this.subject.shape.Height;

                Point p = Disk.getPosition( this.subject, i, Game.sticks[i].getLowestPos() );

                Canvas.SetLeft( shapes[i], p.X );
                Canvas.SetTop( shapes[i], p.Y );

                shapes[i].Visibility = Visibility.Visible;
            }

        }

        public void Hide() {

            subject = null;

            for( int i = 0; i < shapes.Length; i++ ) {

                shapes[i].Visibility = Visibility.Hidden;
            }
        }
    }
}
