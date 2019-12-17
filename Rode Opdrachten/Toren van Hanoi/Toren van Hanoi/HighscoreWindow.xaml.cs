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
    /// Interaction logic for HighscoreWindow.xaml
    /// </summary>
    public partial class HighscoreWindow : Window {

        public static void Execute( int Score, Difficulty difficulty ) {

            HighscoreWindow window = new HighscoreWindow();

            window.lblScore.Content = String.Format( "Score: {0}", Score );

            Nullable<bool> save = window.ShowDialog();

            if ( save != true ) {
                return;
            }

            HighscoreManager.addScore( window.edtName.Text, Score, difficulty );
        }

        public HighscoreWindow() {
            InitializeComponent();
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {

            DialogResult = true;
            Close();
        }

        private void edtName_KeyDown( object sender, KeyEventArgs e ) {

            if ( e.Key != Key.Enter ) {

                return;
            }

            btnSave.RaiseEvent( new RoutedEventArgs(Button.ClickEvent) );
        }
    }
}
