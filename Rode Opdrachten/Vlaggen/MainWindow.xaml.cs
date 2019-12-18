using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Xml;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;


namespace Vlaggen {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    class MarkHandler {

        public Grid[] marks;

        public SolidColorBrush colorEmpty       = Brushes.Orange;
        public SolidColorBrush colorCorrect     = Brushes.Green;
        public SolidColorBrush colorWrong       = Brushes.Red;

        public MarkHandler( int sz ) {

            marks = new Grid[ sz ];
        }

        public void reset() {

            for ( int i = 0; i < marks.Length; i++ ) {
                marks[ i ].Background = colorEmpty;
            }
        }

        public void add( int idx, Grid mark ) {

            marks[ idx ] = mark;
        }

        public void set( int idx, Boolean correct ) {

            marks[ idx ].Background = ( correct ) ? colorCorrect : colorWrong;
        }

    }


    public partial class MainWindow : Window {

        private int currentIndex;

        private Quiz quiz;
        private MarkHandler markHandler;


        private void initDatabase() {

            CountryManager.init();

            for ( int i = 0; i < CountryManager.countries.Length; i++ ) {
                boxName.Items.Add( CountryManager.countries[ i ].name );
                boxNameQuestion.Items.Add( CountryManager.countries[ i ].name );
            }

            boxName.SelectedIndex = 0;
        }

        public MainWindow() {
            InitializeComponent();

            Randomizer.init();
            initDatabase();

            markHandler = new MarkHandler( Quiz.MAX_QUIZ_QUESTION );
            markHandler.add( 0, mark0 );
            markHandler.add( 1, mark1 );
            markHandler.add( 2, mark2 );
            markHandler.add( 3, mark3 );
            markHandler.add( 4, mark4 );
            markHandler.add( 5, mark5 );
            markHandler.add( 6, mark6 );
            markHandler.add( 7, mark7 );
            markHandler.add( 8, mark8 );
            markHandler.add( 9, mark9 );

            // Make sure we always start correct
            tabControl.SelectedIndex = 0;
        }

        private void BtnNextFlag_Click( object sender, RoutedEventArgs e ) {
            int nextIndex = currentIndex + 1;

            if ( nextIndex > CountryManager.countries.Length - 1 ) {
                nextIndex = 0;
            }

            boxName.SelectedIndex = nextIndex;
        }

        private void BtnPrevFlag_Click( object sender, RoutedEventArgs e ) {
            int nextIndex = currentIndex - 1;

            if ( nextIndex < 0 ) {
                nextIndex = CountryManager.countries.Length - 1;
            }

            boxName.SelectedIndex = nextIndex;
        }

        private void BoxName_SelectionChanged( object sender, SelectionChangedEventArgs e ) {

            currentIndex = ( (ComboBox)sender ).SelectedIndex;
            // Load the matching flag
            imgFlag.Source = CountryManager.countries[ currentIndex ].flag;
        }


        private void askOpenQuestion() {

            // Prepare the question
            imgFlagOpen.Source              = CountryManager.countries[ quiz.getCurrentAnswer() ].flag;
            boxNameQuestion.SelectedIndex   = CountryManager.randomCountry();

            // Display the question
            tabControlQuiz.SelectedIndex = 0;
        }

        private void askClosedQuestion() {

            // Generate some random answers
            int[] pool = CountryManager.randomCountries();
            for ( int i = 0; i < 3; i++ ) {
                if ( pool[ i ] == quiz.getCurrentAnswer() ) { // small chance, but a chance
                    pool[ i ] = pool[ i+10 ];
                    break; // it cant happen twice
                }
            }

            // Insert the correct answer
            int localAnswer     = Randomizer.randomNumber(3);
            pool[ localAnswer ] = quiz.getCurrentAnswer();

            // Display the possible answers
            rbClosedA.Tag = pool[ 0 ];
            rbClosedB.Tag = pool[ 1 ];
            rbClosedC.Tag = pool[ 2 ];

            rbClosedA.Content = CountryManager.countries[ pool[ 0 ] ].name;
            rbClosedB.Content = CountryManager.countries[ pool[ 1 ] ].name;
            rbClosedC.Content = CountryManager.countries[ pool[ 2 ] ].name;

            rbClosedA.ToolTip = rbClosedA.Content;
            rbClosedB.ToolTip = rbClosedB.Content;
            rbClosedC.ToolTip = rbClosedC.Content;

            // And the correct flag
            imgFlagClosed.Source = CountryManager.countries[ quiz.getCurrentAnswer() ].flag;

            tabControlQuiz.SelectedIndex = 1;
        }

        private void askQuestion() {

            switch ( quiz.questionTypes[ quiz.currentQuestion ] ) {
                case 0:
                    askOpenQuestion();
                    break;
                case 1:
                    askClosedQuestion();
                    break;
            }

        }

        private void answerQuestion( int answer ) {

            Boolean correct = quiz.answerQuestion( answer );

            // Update the sidebar
            markHandler.set( quiz.currentQuestion, correct );

            if ( quiz.nextQuestion() < 0 ) {

                MessageBox.Show( String.Format( "Einde van de quiz! Je had {0} van de {1} vragen goed!", quiz.correctAnswerCount, Quiz.MAX_QUIZ_QUESTION ) );
                tabControl.SelectedIndex = 0;

                new WindowResults( quiz ).Show();
                Hide();

                return;
            }

            askQuestion();
        }

        private void BtnStartQuiz_Click( object sender, RoutedEventArgs e ) {

            // Generate the quiz
            quiz = new Quiz();

            // Clean any previous results
            markHandler.reset();

            // Go to tab 'Quiz'
            tabControl.SelectedIndex = 1;
            askQuestion();
        }

        private void BtnAnswerOpen_Click( object sender, RoutedEventArgs e ) {

            answerQuestion( boxNameQuestion.SelectedIndex );
        }

        private void BtnAnswerClosed_Click( object sender, RoutedEventArgs e ) {

            if ( (bool)rbClosedA.IsChecked ) {

                rbClosedA.IsChecked = false;
                answerQuestion( (int)rbClosedA.Tag );
                return;
            }

            if ( (bool)rbClosedB.IsChecked ) {

                rbClosedB.IsChecked = false;
                answerQuestion( (int)rbClosedB.Tag );
                return;
            }

            if ( (bool)rbClosedC.IsChecked ) {

                rbClosedC.IsChecked = false;
                answerQuestion( (int)rbClosedC.Tag );
                return;
            }
        }
    }
}
