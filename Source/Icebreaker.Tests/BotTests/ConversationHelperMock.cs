// <copyright file="ConversationHelperMock.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Icebreaker.Tests.BotTests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Icebreaker.Helpers;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Adapters;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Provide fake implementation to static methods call in BotFramework.
    /// </summary>
    public class ConversationHelperMock : ConversationHelper
    {

        private readonly TestAdapter botAdapter;

        public ConversationHelperMock()
            : base(MicrosoftAppCredentials.Empty, new TelemetryClient(), new TestAdapter(Channels.Msteams)
            {
                Conversation =
                {
                Conversation = new ConversationAccount
                {
                    ConversationType = "channel",
                },
                },
            })
        {
        }

        public override Task<TeamsChannelAccount> GetMemberAsync(ITurnContext turnContext, string memberId, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TeamsChannelAccount());
        }
    }
}