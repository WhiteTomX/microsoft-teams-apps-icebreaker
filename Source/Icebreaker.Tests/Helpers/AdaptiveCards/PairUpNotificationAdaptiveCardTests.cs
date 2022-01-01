﻿namespace Icebreaker.Helpers.AdaptiveCards.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Schema.Teams;
    using Xunit;

    public class PairUpNotificationAdaptiveCardTests
    {
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
        public void GetCardNullTest(string teamName, TeamsChannelAccount account1, TeamsChannelAccount account2, string botDisplayName, string question)
        {
            Assert.ThrowsAny<ArgumentException>(() => PairUpNotificationAdaptiveCard.GetCard(teamName, account1, account2, botDisplayName, question));
        }
    }
}