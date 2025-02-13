// <copyright file="MatchingServiceTests.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Icebreaker.Tests.ServicesTests
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Icebreaker.Helpers;
    using Icebreaker.Helpers.AdaptiveCards;
    using Icebreaker.Interfaces;
    using Icebreaker.Services;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Adapters;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="MatchingService"/> class.
    /// </summary>
    public class MatchingServiceTests
    {
        private readonly IMatchingService sut;
        private readonly TestAdapter botAdapter;
        private readonly Mock<IBotDataProvider> dataProvider;
        private readonly Mock<QuestionService> questionService;
        private readonly Mock<ConversationHelper> conversationHelper;
        private readonly Mock<ResourcesService> resourcesService;
        private readonly Mock<AdaptiveCardFactory> adaptiveCardFactory; 
        private readonly string maxPairsSettingsKey = "MaxPairUpsPerTeam";

        /// <summary>
        /// Initializes a new instance of the <see cref="MatchingServiceTests"/> class.
        /// </summary>
        public MatchingServiceTests()
        {
            this.botAdapter = new TestAdapter(Channels.Msteams)
            {
                Conversation =
                {
                    Conversation = new ConversationAccount
                    {
                        ConversationType = "channel",
                    },
                },
            };
            var telemetryClient = new TelemetryClient();
            this.conversationHelper = new Mock<ConversationHelper>(MockBehavior.Loose, new MicrosoftAppCredentials(string.Empty, string.Empty), telemetryClient, this.botAdapter);
            this.conversationHelper.Setup(x => x.GetTeamNameByIdAsync(this.botAdapter, It.IsAny<TeamInstallInfo>()))
                .Returns(() => Task.FromResult("IceBreakerTeam"));
            // make sure that Teammembers are NEVER retrieved
            this.conversationHelper.Setup(x => x.GetTeamMembers(It.IsAny<string>(), It.IsAny<string>())).Throws(new NotSupportedException("We should not retrieves all members of a team as they may not opted in."));

            this.dataProvider = new Mock<IBotDataProvider>();
            this.dataProvider.Setup(x => x.GetInstalledTeamAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult(new TeamInstallInfo()));

            this.resourcesService = new Mock<ResourcesService>(this.dataProvider.Object, new Microsoft.ApplicationInsights.TelemetryClient());
            this.adaptiveCardFactory = new Mock<AdaptiveCardFactory>(this.resourcesService.Object);

            this.questionService = new Mock<QuestionService>(MockBehavior.Loose, this.dataProvider.Object, telemetryClient);
            this.questionService.Setup(x => x.GetRandomOrDefaultQuestion(It.IsAny<string>())).Returns(() => Task.FromResult("question"));

            this.sut = new MatchingService(this.dataProvider.Object, this.conversationHelper.Object, this.questionService.Object, telemetryClient, this.botAdapter, this.adaptiveCardFactory.Object);
        }

        [Fact]
        public async Task MatchPairs_NoTeamsInstalled_NoPairsGenerated()
        {
            // Arrange
            this.dataProvider.Setup(x => x.GetInstalledTeamsAsync())
                .Returns(() => Task.FromResult((IList<TeamInstallInfo>)new List<TeamInstallInfo>()));

            this.dataProvider.Setup(x => x.GetAllUsersOptInStatusAsync())
                .Returns(() => Task.FromResult(new Dictionary<string, bool>()));

            // Act
            // Send the message activity to the bot.
            var pairsNotifiedCount = await this.sut.MakePairsAndNotifyAsync();

            // Assert GetInstalledTeamsAsync is called once
            this.dataProvider.Verify(m => m.GetInstalledTeamsAsync(), Times.Once);

            // Assert GetAllUsersOptInStatusAsync is called once
            this.dataProvider.Verify(m => m.GetAllUsersOptInStatusAsync(), Times.Once);

            // No call to GetTeamNameByIdAsync since no match
            this.conversationHelper.Verify(m => m.GetTeamNameByIdAsync(this.botAdapter, It.IsAny<TeamInstallInfo>()), Times.Never);

            // No call to GetTeamMembers since no match
            this.conversationHelper.Verify(m => m.GetTeamMembers(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            // No groups paired since no teams installed
            Assert.Equal(0, pairsNotifiedCount);
        }

        [Fact]
        public async Task MatchPairs_NoOptedIn_NoPairsGenerated()
        {
            // Arrange
            this.dataProvider.Setup(x => x.GetInstalledTeamsAsync())
                .Returns(() => Task.FromResult((IList<TeamInstallInfo>)new List<TeamInstallInfo>
                {
                    new TeamInstallInfo { TeamId = Guid.NewGuid().ToString() },
                    new TeamInstallInfo { TeamId = Guid.NewGuid().ToString() },
                }));

            // No user opted-out
            this.dataProvider.Setup(x => x.GetAllUsersOptInStatusAsync())
                .Returns(() => Task.FromResult(new Dictionary<string, bool>()));

            // Act
            // Send the message activity to the bot.
            var pairsNotifiedCount = await this.sut.MakePairsAndNotifyAsync();

            // Assert GetInstalledTeamsAsync is called once
            this.dataProvider.Verify(m => m.GetInstalledTeamsAsync(), Times.Once);

            // Assert GetAllUsersOptInStatusAsync is called once
            this.dataProvider.Verify(m => m.GetAllUsersOptInStatusAsync(), Times.Once);

            // 2 calls to GetTeamNameByIdAsync since we have 2 teams
            this.conversationHelper.Verify(m => m.GetTeamNameByIdAsync(this.botAdapter, It.IsAny<TeamInstallInfo>()), Times.Exactly(2));

            // No groups paired since no one opted in
            Assert.Equal(0, pairsNotifiedCount);
        }

        [Fact]
        public async Task MatchPairs_OneMemberOptedIn_NoPairsGenerated()
        {
            var teams = new List<TeamInstallInfo>
                {
                    new TeamInstallInfo { TeamId = Guid.NewGuid().ToString() },
                    new TeamInstallInfo { TeamId = Guid.NewGuid().ToString() },
                };

            // Arrange
            this.dataProvider.Setup(x => x.GetInstalledTeamsAsync())
                .Returns(() => Task.FromResult<IList<TeamInstallInfo>>(teams));

            var users = this.GenerateUsers(2);

            // Two people opted in
            this.dataProvider.Setup(x => x.GetAllUsersOptInStatusAsync())
                .Returns(() => Task.FromResult(users.ToDictionary(u => u.Id, u => true)));

            // only one opted in member per team
            this.conversationHelper.Setup(x => x.GetTeamMemberAsync(users[0].Id, teams[0].TeamId, It.IsAny<string>()))
                .Returns((string userId, string teamId, string serviceUrl) => Task.FromResult(users[0]));
            this.conversationHelper.Setup(x => x.GetTeamMemberAsync(users[1].Id, teams[1].TeamId, It.IsAny<string>()))
                .Returns((string userId, string teamId, string serviceUrl) => Task.FromResult(users[1]));

            // Act
            // Send the message activity to the bot.
            var pairsNotifiedCount = await this.sut.MakePairsAndNotifyAsync();

            // Assert GetInstalledTeamsAsync is called once
            this.dataProvider.Verify(m => m.GetInstalledTeamsAsync(), Times.Once);

            // Assert GetAllUsersOptInStatusAsync is called once
            this.dataProvider.Verify(m => m.GetAllUsersOptInStatusAsync(), Times.Once);

            // 2 calls to GetTeamNameByIdAsync since we have 2 teams
            this.conversationHelper.Verify(m => m.GetTeamNameByIdAsync(this.botAdapter, It.IsAny<TeamInstallInfo>()), Times.Exactly(2));

            // 4 calls to GetTeamMemberAsync since we have 2 users and two teams
            this.conversationHelper.Verify(m => m.GetTeamMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(4));

            // 0 groups are paired
            Assert.Equal(0, pairsNotifiedCount);
        }

        [Fact]
        public async Task MatchPairs_MemberOptedOut_NoPairsGenerated()
        {
            // Arrange
            this.dataProvider.Setup(x => x.GetInstalledTeamsAsync())
                .Returns(() => Task.FromResult<IList<TeamInstallInfo>>(new List<TeamInstallInfo>
                {
                    new TeamInstallInfo { TeamId = Guid.NewGuid().ToString() },
                    new TeamInstallInfo { TeamId = Guid.NewGuid().ToString() },
                }));

            var optedOutUserId = Guid.NewGuid().ToString();
            var optedInUserId = Guid.NewGuid().ToString();

            // No user opted-out
            this.dataProvider.Setup(x => x.GetAllUsersOptInStatusAsync())
                .Returns(() => Task.FromResult(new Dictionary<string, bool>
                {
                    { optedOutUserId, false },
                    { optedInUserId, true },
                }));

            // all users are in all teams
            this.conversationHelper.Setup(x => x.GetTeamMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string userId, string teamId, string serviceUrl) => Task.FromResult<TeamsChannelAccount>(new TeamsChannelAccount()
                    {
                        Id = userId,
                        AadObjectId = Guid.NewGuid().ToString(),
                        UserPrincipalName = string.Empty,
                        Email = string.Empty,
                    }));

            // Act
            // Send the message activity to the bot.
            var pairsNotifiedCount = await this.sut.MakePairsAndNotifyAsync();

            // Assert GetInstalledTeamsAsync is called once
            this.dataProvider.Verify(m => m.GetInstalledTeamsAsync(), Times.Once);

            // Assert GetAllUsersOptInStatusAsync is called once
            this.dataProvider.Verify(m => m.GetAllUsersOptInStatusAsync(), Times.Once);

            // 2 calls to GetTeamNameByIdAsync since we have 2 teams
            this.conversationHelper.Verify(m => m.GetTeamNameByIdAsync(this.botAdapter, It.IsAny<TeamInstallInfo>()), Times.Exactly(2));

            // 2 calls to GetTeamMemberAsync since we have 2 teams and one opted in user
            this.conversationHelper.Verify(m => m.GetTeamMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));

            // No groups paired since only 1 member opted-in per team
            Assert.Equal(0, pairsNotifiedCount);
        }

        [Fact]
        public async Task MatchPairs_MembersOptedIn_PairsGenerated()
        {
            // Arrange
            this.dataProvider.Setup(x => x.GetInstalledTeamsAsync())
                .Returns(() => Task.FromResult<IList<TeamInstallInfo>>(new List<TeamInstallInfo>
                {
                    new TeamInstallInfo { TeamId = Guid.NewGuid().ToString() },
                    new TeamInstallInfo { TeamId = Guid.NewGuid().ToString() },
                }));

            var optedInUserIds = new string[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            // No user opted-out
            this.dataProvider.Setup(x => x.GetAllUsersOptInStatusAsync())
                .Returns(() => Task.FromResult(new Dictionary<string, bool>
                {
                    { optedInUserIds[0], true },
                    { optedInUserIds[1], true },
                }));

            // all users are in all teams
            this.conversationHelper.Setup(x => x.GetTeamMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string userId, string teamId, string serviceUrl) => Task.FromResult<TeamsChannelAccount>(new TeamsChannelAccount()
                {
                    Id = userId,
                    AadObjectId = Guid.NewGuid().ToString(),
                    UserPrincipalName = "something@other.local",
                    Email = string.Empty,
                }));

            // Act
            // Send the message activity to the bot.
            var pairsNotifiedCount = await this.sut.MakePairsAndNotifyAsync();

            // Assert GetInstalledTeamsAsync is called once
            this.dataProvider.Verify(m => m.GetInstalledTeamsAsync(), Times.Once);

            // Assert GetAllUsersOptInStatusAsync is called once
            this.dataProvider.Verify(m => m.GetAllUsersOptInStatusAsync(), Times.Once);

            // 2 calls to GetTeamNameByIdAsync since we have 2 teams
            this.conversationHelper.Verify(m => m.GetTeamNameByIdAsync(this.botAdapter, It.IsAny<TeamInstallInfo>()), Times.Exactly(2));

            // 4 calls to GetTeamMemberAsync since we have 2 teams
            this.conversationHelper.Verify(m => m.GetTeamMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(4));

            // 2 groups are paired (1 group per team)
            Assert.Equal(2, pairsNotifiedCount);
        }

        [Fact]
        public async Task MatchPairs_MaxPairSettingIsZero_NoPairsGenerated()
        {
            // Arrange
            this.dataProvider.Setup(x => x.GetInstalledTeamsAsync())
                .Returns(() => Task.FromResult<IList<TeamInstallInfo>>(new List<TeamInstallInfo>
                {
                    new TeamInstallInfo { TeamId = Guid.NewGuid().ToString() },
                }));

            var users = GenerateUsers(2);

            // All users opted in
            this.dataProvider.Setup(x => x.GetAllUsersOptInStatusAsync())
                .Returns(() => Task.FromResult(users.ToDictionary(u => u.Id, u => true)));

            // All users member of all teams
            this.conversationHelper.Setup(x => x.GetTeamMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string userId, string teamId, string serviceUrl) => Task.FromResult<TeamsChannelAccount>(new TeamsChannelAccount()
                {
                    Id = userId,
                    AadObjectId = Guid.NewGuid().ToString(),
                    UserPrincipalName = string.Empty,
                    Email = string.Empty,
                }));

            var maxPairUpsPerTeam = ConfigurationManager.AppSettings[this.maxPairsSettingsKey];
            ConfigurationManager.AppSettings[this.maxPairsSettingsKey] = "0";
            var sut = new MatchingService(this.dataProvider.Object, this.conversationHelper.Object, this.questionService.Object, new TelemetryClient(), this.botAdapter, this.adaptiveCardFactory.Object);

            // Act

            // Send the message activity to the bot.
            var pairsNotifiedCount = await sut.MakePairsAndNotifyAsync();

            // Set original value back
            ConfigurationManager.AppSettings[this.maxPairsSettingsKey] = maxPairUpsPerTeam;

            // Assert GetInstalledTeamsAsync is called once
            this.dataProvider.Verify(m => m.GetInstalledTeamsAsync(), Times.Once);

            // Assert GetAllUsersOptInStatusAsync is called once
            this.dataProvider.Verify(m => m.GetAllUsersOptInStatusAsync(), Times.Once);

            // 1 calls to GetTeamNameByIdAsync since we have 1 team
            this.conversationHelper.Verify(m => m.GetTeamNameByIdAsync(this.botAdapter, It.IsAny<TeamInstallInfo>()), Times.Once);

            // 2 calls to GetTeamMemberAsync since we have 1 team
            this.conversationHelper.Verify(m => m.GetTeamMemberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));

            // No pairs since max limit is reached
            Assert.Equal(0, pairsNotifiedCount);
        }

        private List<TeamsChannelAccount> GenerateUsers(int number)
        {
            var users = new List<TeamsChannelAccount>();
            for (int i = 0; i < number; i++)
            {
                users.Add(new TeamsChannelAccount
                {
                    Id = Guid.NewGuid().ToString(),
                    AadObjectId = Guid.NewGuid().ToString(),
                    UserPrincipalName = string.Empty,
                    Email = string.Empty,
                });
            }
            return users;
        }
    }
}
