using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Toren_van_Hanoi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public static MainWindow instance = (MainWindow)Application.Current.MainWindow;

        public MainWindow() {

            InitializeComponent();            

            // Bind the highscores to the GUI
            lvHighscore.ItemsSource = HighscoreManager.highscores;

            // Setup groups and sorting
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvHighscore.ItemsSource);

            view.GroupDescriptions.Add( new PropertyGroupDescription( "Difficulty" ) );
            view.SortDescriptions.Add( new SortDescription( "Difficulty", ListSortDirection.Ascending ) );
            view.SortDescriptions.Add( new SortDescription( "Score", ListSortDirection.Ascending ) );

            HighscoreManager.loadFromFile( "scores.xml" );
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {

            int difficulty = int.Parse( ( (Button)sender ).Tag.ToString() );

            switch ( difficulty ) {

                case 0:
                    Game.New( Difficulty.Easy );
                    break;
                case 1:
                    Game.New( Difficulty.Medium );
                    break;
                case 2:
                    Game.New( Difficulty.Hard );
                    break;
                case 3:
                    Game.New( Difficulty.Expert );
                    break;

            }

        }

        private void Window_Closed( object sender, EventArgs e ) {
            HighscoreManager.saveToFile( "scores.xml" );
        }
    }
}
