namespace Vlaggen {
    using System;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class WindowResults : Window {
        /// <summary>
        /// Defines the quiz
        /// </summary>
        private Quiz quiz;

        /// <summary>
        /// Defines the currentQuestion
        /// </summary>
        private int currentQuestion = 0;

        /// <summary>
        /// The displayQuestion
        /// </summary>
        private void displayQuestion() {

            Title = String.Format( "{0} / {1}", currentQuestion+1, Quiz.MAX_QUIZ_QUESTION );

            imgFlagCorrect.Source       = CountryManager.countries[ quiz.questionAnswers[ currentQuestion ] ].flag;
            lblAnswerCorrect.Content    = CountryManager.countries[ quiz.questionAnswers[ currentQuestion ] ].name;

            imgFlagUser.Source    = CountryManager.countries[ quiz.userAnswers[ currentQuestion ] ].flag;
            lblAnswerUser.Content = CountryManager.countries[ quiz.userAnswers[ currentQuestion ] ].name;

            Boolean correct = ( quiz.userAnswers[currentQuestion] == quiz.questionAnswers[currentQuestion] );

            lblStatus.Content       = ( correct ) ? "CORRECT" : "FOUT";
            lblStatus.Foreground    = ( correct ) ? Brushes.Green : Brushes.Red;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowResults"/> class.
        /// </summary>
        /// <param name="quiz">The quiz<see cref="Quiz"/></param>
        public WindowResults( Quiz quiz ) {

            InitializeComponent();

            this.quiz       = quiz;
            currentQuestion = 0;

            displayQuestion();
        }

        /// <summary>
        /// The BtnNext_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void BtnNext_Click( object sender, RoutedEventArgs e ) {

            currentQuestion++;

            if ( currentQuestion >= Quiz.MAX_QUIZ_QUESTION ) {

                MessageBox.Show( "Dit waren alle antwoorden." );
                return;
            }

            displayQuestion();
        }

        /// <summary>
        /// The BtnPrev_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/></param>
        private void BtnPrev_Click( object sender, RoutedEventArgs e ) {

            if ( currentQuestion <= 0 ) {
                return;
            }

            currentQuestion--;
            displayQuestion();
        }

        /// <summary>
        /// The Window_Closed
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void Window_Closed( object sender, EventArgs e ) {
            App.Current.MainWindow.Show();
        }
    }
}
