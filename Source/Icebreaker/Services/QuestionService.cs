namespace Icebreaker.Services
{
    using Icebreaker.Interfaces;
    using Icebreaker.Properties;
    using Microsoft.Azure;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Implements minor question logic
    /// </summary>
    public class QuestionService
    {
        private readonly IBotDataProvider dataProvider;
        private readonly IDictionary<string, string[]> questions;
        private readonly Random random;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionService"/> class.
        /// </summary>
        /// <param name="dataProvider">DataProvider to use</param>
        public QuestionService(IBotDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
            this.questions = new Dictionary<string, string[]>();
            this.random = new Random();
        }

        /// <summary>
        /// Select a random question.
        /// </summary>
        /// <param name="cultureName">Language of Question</param>
        /// <returns>Question</returns>
        public async Task<string> GetQuestion(string cultureName)
        {
            string question;
            if (this.questions.ContainsKey(cultureName))
            {
                var languageQuestions = this.questions[cultureName];
                question = languageQuestions[this.random.Next(languageQuestions.Length)];
            }
            else
            {
                question = Resources.DefaultQuestion;
                await this.dataProvider.SetQuestionsAsync(cultureName, new string[] { question });
            }

            return question;
        }
    }
}