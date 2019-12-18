namespace Vlaggen {
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using Brushes = System.Windows.Media.Brushes;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    internal class MarkHandler {
        /// <summary>
        /// Defines the marks
        /// </summary>
        public Grid[] marks;

        /// <summary>
        /// Defines the colorEmpty
        /// </summary>
        public SolidColorBrush colorEmpty       = Brushes.Orange;

        /// <summary>
        /// Defines the colorCorrect
        /// </summary>
        public SolidColorBrush colorCorrect     = Brushes.Green;

        /// <summary>
        /// Defines the colorWrong
        /// </summary>
        public SolidColorBrush colorWrong       = Brushes.Red;

        /// <summary>
        /// Initializes a new instance of the <see cref="MarkHandler"/> class.
        /// </summary>
        /// <param name="sz">The sz<see cref="int"/></param>
        public MarkHandler( int sz ) {

            marks = new Grid[ sz ];
        }

        /// <summary>
        /// The reset
        /// </summary>
        public void reset() {

            for ( int i = 0; i < marks.Length; i++ ) {
                marks[ i ].Background = colorEmpty;
            }
        }

        /// <summary>
        /// The add
        /// </summary>
        /// <param name="idx">The idx<see cref="int"/></param>
        /// <param name="mark">The mark<see cref="Grid"/></param>
        public void add( int idx, Grid mark ) {

            marks[ idx ] = mark;
        }

        /// <summary>
        /// The set
        /// </summary>
        /// <param name="idx">The idx<see cref="int"/></param>
        /// <param name="correct">The correct<see cref="Boolean"/></param>
        public void set( int idx, Boolean correct ) {

            marks[ idx ].Background = ( correct ) ? colorCorrect : colorWrong;
        }
    }

    /// <summary>
    /// Defines the <see cref="MainWindow" />
    /// </summary>
    public partial class MainWindow : Window {
        /// <summary>
        /// Defines the currentIndex
        /// </summary>
        private int currentIndex;

        /// <summary>
        /// Defines the quiz
        /// </summary>
        private Quiz quiz;

        /// <summary>
        /// Defines the markHandler
        /// </summary>
        private MarkHandler markHandler;

        /// <summary>
        /// The initDatabase
        /// </summary>
        private void initDatabase() {

            CountryManager.init();

            for ( int i = 0; i < CountryManager.countries.Length; i++ ) {
                boxName.Items.Add( CountryManager.countries[ i ].name );
                boxNameQuestion.Items.Add( CountryManager.countries[ i ].name );
            }

            boxName.SelectedIndex = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
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

        /// <summary>
        /// The BtnNextFlag_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void BtnNextFlag_Click( object sender, RoutedEventArgs e ) {
            int nextIndex = currentIndex + 1;

            if ( nextIndex > CountryManager.countries.Length - 1 ) {
                nextIndex = 0;
            }

            boxName.SelectedIndex = nextIndex;
        }

        /// <summary>
        /// The BtnPrevFlag_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void BtnPrevFlag_Click( object sender, RoutedEventArgs e ) {
            int nextIndex = currentIndex - 1;

            if ( nextIndex < 0 ) {
                nextIndex = CountryManager.countries.Length - 1;
            }

            boxName.SelectedIndex = nextIndex;
        }

        /// <summary>
        /// The BoxName_SelectionChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="SelectionChangedEventArgs"/></param>
        private void BoxName_SelectionChanged( object sender, SelectionChangedEventArgs e ) {

            currentIndex = ( (ComboBox)sender ).SelectedIndex;
            // Load the matching flag
            imgFlag.Source = CountryManager.countries[ currentIndex ].flag;
        }

        /// <summary>
        /// The askOpenQuestion
        /// </summary>
        private void askOpenQuestion() {

            // Prepare the question
            imgFlagOpen.Source              = CountryManager.countries[ quiz.getCurrentAnswer() ].flag;
            boxNameQuestion.SelectedIndex   = CountryManager.randomCountry();

            // Display the question
            tabControlQuiz.SelectedIndex = 0;
        }

        /// <summary>
        /// The askClosedQuestion
        /// </summary>
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

        /// <summary>
        /// The askQuestion
        /// </summary>
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

        /// <summary>
        /// The answerQuestion
        /// </summary>
        /// <param name="answer">The answer<see cref="int"/></param>
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

        /// <summary>
        /// The BtnStartQuiz_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void BtnStartQuiz_Click( object sender, RoutedEventArgs e ) {

            // Generate the quiz
            quiz = new Quiz();

            // Clean any previous results
            markHandler.reset();

            // Go to tab 'Quiz'
            tabControl.SelectedIndex = 1;
            askQuestion();
        }

        /// <summary>
        /// The BtnAnswerOpen_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void BtnAnswerOpen_Click( object sender, RoutedEventArgs e ) {

            answerQuestion( boxNameQuestion.SelectedIndex );
        }

        /// <summary>
        /// The BtnAnswerClosed_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
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
