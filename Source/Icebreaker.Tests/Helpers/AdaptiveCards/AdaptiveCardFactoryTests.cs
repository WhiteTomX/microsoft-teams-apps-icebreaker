namespace Icebreaker.Helpers.AdaptiveCards.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using global::AdaptiveCards;
    using Icebreaker.Interfaces;
    using Icebreaker.Services;
    using Microsoft.Bot.Schema.Teams;
    using Moq;
    using Xunit;

    public class AdaptiveCardFactoryTests
    {
        private readonly Mock<IBotDataProvider> dataProvider;
        private readonly Mock<ResourcesService> resourcesService;
        private readonly AdaptiveCardFactory sut;
        public AdaptiveCardFactoryTests()
        {
            this.dataProvider = new Mock<IBotDataProvider>();
            this.resourcesService = new Mock<ResourcesService>(this.dataProvider.Object, new Microsoft.ApplicationInsights.TelemetryClient());
            this.sut = new AdaptiveCardFactory(this.resourcesService.Object);
        }

        public static IEnumerable<object[]> GetNullTests()
        {
            yield return new object[] { null, new TeamsChannelAccount(), new TeamsChannelAccount(), "bot", "question" };
            yield return new object[] { "Team", null, new TeamsChannelAccount(), "bot", "question" };
            yield return new object[] { "Team", new TeamsChannelAccount(), null,  "bot", "question" };
            yield return new object[] { "Team", new TeamsChannelAccount(), new TeamsChannelAccount(), null, "question" };
            yield return new object[] { "Team", new TeamsChannelAccount(), new TeamsChannelAccount(), "bot", null };
            yield return new object[] { "Team", new TeamsChannelAccount(), new TeamsChannelAccount(), "bot", "question" };
        }

        /// <summary>
        /// Check that GetCard Method tests for NullValues
        /// </summary>
        /// <param name="teamName">Name of Team to pass to MethodUnderTest</param>
        /// <param name="account1">Account1 to pass to MethodUnderTest</param>
        /// <param name="account2">Account2 to pass to MethodUnderTest</param>
        /// <param name="botDisplayName">DisplayName of Bot to pass to MethodUnderTest</param>
        /// <param name="question">Question to pass to MethodUnderTest</param>
        [Theory]
        [MemberData(nameof(GetNullTests))]
        public async void GetPairUpNotificationCardNullTest(string teamName, TeamsChannelAccount account1, TeamsChannelAccount account2, string botDisplayName, string question)
        {
            await Assert.ThrowsAnyAsync<ArgumentException>(() => this.sut.GetPairUpNotificationCard(teamName, account1, account2, botDisplayName, question));
        }

        [Fact]
        public async void GetPairUpNotificationCardWithQuestionTest()
        {
            var teamName = "Team";
            var account1 = new TeamsChannelAccount() { UserPrincipalName = "test@test.com" };
            var account2 = new TeamsChannelAccount() { UserPrincipalName = "test@test.com" };
            var botDisplayName = "Bot";
            var question = "questionsfdg24323";

            var attachement = await this.sut.GetPairUpNotificationCard(teamName, account1, account2, botDisplayName, question);

            Assert.Contains(question, ((AdaptiveTextBlock)((AdaptiveContainer)((AdaptiveCard)attachement.Content).Body[1]).Items[3]).Text);
        }
    }
}