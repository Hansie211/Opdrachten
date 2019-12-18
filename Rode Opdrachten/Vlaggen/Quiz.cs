namespace Vlaggen {
    using System;

    /// <summary>
    /// Defines the <see cref="Quiz" />
    /// </summary>
    public class Quiz {
        /// <summary>
        /// Defines the MAX_QUIZ_QUESTION
        /// </summary>
        public const int MAX_QUIZ_QUESTION = 10;

        /// <summary>
        /// Defines the questionAnswers
        /// </summary>
        public int[] questionAnswers;

        /// <summary>
        /// Defines the userAnswers
        /// </summary>
        public int[] userAnswers;

        /// <summary>
        /// Defines the questionTypes
        /// </summary>
        public int[] questionTypes;

        /// <summary>
        /// Defines the currentQuestion
        /// </summary>
        public int currentQuestion;

        /// <summary>
        /// Defines the correctAnswerCount
        /// </summary>
        public int correctAnswerCount;

        /// <summary>
        /// The answerQuestion
        /// </summary>
        /// <param name="answer">The answer<see cref="int"/></param>
        /// <returns>The <see cref="Boolean"/></returns>
        public Boolean answerQuestion( int answer ) {

            userAnswers[ currentQuestion ] = answer;

            Boolean correct = ( answer == getCurrentAnswer() );

            if ( correct ) {
                correctAnswerCount++;
            }

            return correct;
        }

        /// <summary>
        /// The quizFinished
        /// </summary>
        /// <returns>The <see cref="Boolean"/></returns>
        public Boolean quizFinished() {

            return currentQuestion >= MAX_QUIZ_QUESTION;
        }

        /// <summary>
        /// The getCurrentAnswer
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public int getCurrentAnswer() {

            return questionAnswers[ currentQuestion ];
        }

        /// <summary>
        /// The nextQuestion
        /// </summary>
        /// <returns>The <see cref="int"/></returns>
        public int nextQuestion() {

            currentQuestion++;

            if ( quizFinished() ) {
                return -1;
            }

            return getCurrentAnswer();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quiz"/> class.
        /// </summary>
        public Quiz() {

            correctAnswerCount  = 0;
            currentQuestion     = 0;

            questionAnswers = new int[ MAX_QUIZ_QUESTION ];
            userAnswers     = new int[ MAX_QUIZ_QUESTION ];

            // Get our 10 questions
            int[] randomCountries = CountryManager.randomCountries();

            for ( int i = 0; i < MAX_QUIZ_QUESTION; i++ ) {
                questionAnswers[ i ] = randomCountries[ i ];
            }

            // I only want 3 open questions
            questionTypes = new int[ MAX_QUIZ_QUESTION ] { 0, 0, 0, 1, 1, 1, 1, 1, 1, 1 };
            questionTypes = Randomizer.randomize( questionTypes );
        }
    }
}
