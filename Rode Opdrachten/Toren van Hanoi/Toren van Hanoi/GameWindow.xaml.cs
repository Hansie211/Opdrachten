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

namespace Toren_van_Hanoi {
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window {

        private VirtualDisk vDisk = null;

        public GameWindow() {

            InitializeComponent();

            vDisk = new VirtualDisk( canvas );
            vDisk.onClick += onClickVirtualDisk;

            canvas.Width    = Game.boardWidth;
            canvas.Height   = Game.boardHeight;

            labelFinish.Width  = Game.boardWidth;
            labelFinish.Height = Game.boardHeight;
        }

        public void onClickDisk( object sender, MouseButtonEventArgs e ) {

            Disk disk = (Disk)sender;

            if ( !disk.isTop() ) {

                // Cannot move 'non-top' disks
                return;
            }

            vDisk.Show( disk );
        }

        public void onClickVirtualDisk( object sender, MouseButtonEventArgs e ) {

            if ( vDisk.subject == null ) {

                return;
            }

            vDisk.subject.moveToStick( vDisk.getClickedItem() );

            // De-activate & cleanup
            vDisk.Hide();

            Game.increaseScore();


            // Did we win? ( Are all disks on the last stick? )
            if ( !Game.finished() ) {

                return;
            }

            // We did!
            Game.Win();
        }

        private void canvas_MouseRightButtonDown( object sender, MouseButtonEventArgs e ) {

            if ( vDisk.subject == null ) {
                return;
            }

            // Cancel action, deactive everything
            vDisk.Hide();
        }

        private void labelWon_MouseRightButtonDown( object sender, MouseButtonEventArgs e ) {

            Close();
        }

    }
}
