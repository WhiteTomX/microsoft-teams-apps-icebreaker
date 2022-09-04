// <copyright file="PairUpNotificationAdaptiveCardFactory.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Icebreaker.Helpers.AdaptiveCards
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using System.Web.Helpers;
    using global::AdaptiveCards;
    using global::AdaptiveCards.Templating;
    using Icebreaker.Properties;
    using Icebreaker.Services;
    using Microsoft.Azure;
    using Microsoft.Bot.Schema;
    using Microsoft.Bot.Schema.Teams;

    /// <summary>
    /// Builder class for the pairup notification card
    /// </summary>
    public class AdaptiveCardFactory : AdaptiveCardBase
    {
        /// <summary>
        /// Default marker string in the UPN that indicates a user is externally-authenticated
        /// </summary>
        private const string ExternallyAuthenticatedUpnMarker = "#ext#";

        private static readonly Lazy<AdaptiveCardTemplate> WelcomeTeamAdaptiveCardTemplate =
            new Lazy<AdaptiveCardTemplate>(() => CardTemplateHelper.GetAdaptiveCardTemplate(AdaptiveCardName.WelcomeTeam));

        private static readonly Lazy<AdaptiveCardTemplate> WelcomeNewMemberAdaptiveCardTemplate =
            new Lazy<AdaptiveCardTemplate>(() => CardTemplateHelper.GetAdaptiveCardTemplate(AdaptiveCardName.WelcomeNewMember));

        private static readonly Lazy<AdaptiveCardTemplate> UnrecognizedInputAdaptiveCardTemplate =
            new Lazy<AdaptiveCardTemplate>(() => CardTemplateHelper.GetAdaptiveCardTemplate(AdaptiveCardName.UnrecognizedInput));

        private static readonly Lazy<AdaptiveCardTemplate> PairUpNotificationAdaptiveCardTemplate =
            new Lazy<AdaptiveCardTemplate>(() => CardTemplateHelper.GetAdaptiveCardTemplate(AdaptiveCardName.PairUpNotification));

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveCardFactory"/> class.
        /// </summary>
        /// <param name="resourcesService">Service to get ResourceStrings</param>
        public AdaptiveCardFactory(ResourcesService resourcesService)
            : base(resourcesService)
        {
        }


        /// <summary>
        /// Creates the adaptive card for the team welcome message
        /// </summary>
        /// <param name="teamName">The team name</param>
        /// <param name="botInstaller">The name of the person that installed the bot</param>
        /// <returns>The welcome team adaptive card</returns>
        public async Task<Attachment> GetWelcomeTeamCard(string teamName, string botInstaller)
        {
            // Set alignment of text based on default locale.
            var textAlignment = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? AdaptiveHorizontalAlignment.Right.ToString() : AdaptiveHorizontalAlignment.Left.ToString();

            string teamIntroPart1;
            string teamIntroPart2;
            string teamIntroPart3;

            if (string.IsNullOrEmpty(botInstaller))
            {
                teamIntroPart1 = string.Format(await this.GetResourceString(nameof(Resources.InstallMessageUnknownInstallerPart1)), teamName);
                teamIntroPart2 = await this.GetResourceString(nameof(Resources.InstallMessageUnknownInstallerPart2));
                teamIntroPart3 = await this.GetResourceString(nameof(Resources.InstallMessageUnknownInstallerPart3));
            }
            else
            {
                teamIntroPart1 = string.Format(await this.GetResourceString(nameof(Resources.InstallMessageKnownInstallerPart1)), botInstaller, teamName);
                teamIntroPart2 = await this.GetResourceString(nameof(Resources.InstallMessageKnownInstallerPart2));
                teamIntroPart3 = await this.GetResourceString(nameof(Resources.InstallMessageKnownInstallerPart3));
            }

            var baseDomain = CloudConfigurationManager.GetSetting("AppBaseDomain");
            var tourTitle = await this.GetResourceString(nameof(Resources.WelcomeTourTitle));
            var appId = CloudConfigurationManager.GetSetting("ManifestAppId");

            var welcomeData = new
            {
                textAlignment,
                teamIntroPart1,
                teamIntroPart2,
                teamIntroPart3,
                welcomeCardImageUrl = $"https://{baseDomain}/Content/welcome-card-image.png",
                tourUrl = GetTourFullUrl(appId, GetTourUrl(baseDomain), tourTitle),
                salutationText = await this.GetResourceString(nameof(Resources.SalutationTitleText)),
                tourButtonText = await this.GetResourceString(nameof(Resources.TakeATourButtonText)),
            };

            return GetCard(WelcomeTeamAdaptiveCardTemplate.Value, welcomeData);
        }

        /// <summary>
        /// Creates the welcome new member card.
        /// </summary>
        /// <param name="teamName">The team name</param>
        /// <param name="personFirstName">The first name of the new member</param>
        /// <param name="botDisplayName">The bot name</param>
        /// <param name="botInstaller">The person that installed the bot to the team</param>
        /// <returns>The welcome new member card</returns>
        public async Task<Attachment> GetWelcomeNewMemberCard(string teamName, string personFirstName, string botDisplayName, string botInstaller)
        {
            // Set alignment of text based on default locale.
            var textAlignment = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? AdaptiveHorizontalAlignment.Right.ToString() : AdaptiveHorizontalAlignment.Left.ToString();

            string introMessagePart1;
            string introMessagePart2;
            string introMessagePart3 = await this.GetResourceString(nameof(Resources.OptInText));

            if (string.IsNullOrEmpty(botInstaller))
            {
                introMessagePart1 = string.Format(await this.GetResourceString(nameof(Resources.InstallMessageUnknownInstallerPart1)), teamName);
                introMessagePart2 = await this.GetResourceString(nameof(Resources.InstallMessageUnknownInstallerPart2));
            }
            else
            {
                introMessagePart1 = string.Format(await this.GetResourceString(nameof(Resources.InstallMessageKnownInstallerPart1)), botInstaller, teamName);
                introMessagePart2 = await this.GetResourceString(nameof(Resources.InstallMessageKnownInstallerPart2));
            }

            var baseDomain = CloudConfigurationManager.GetSetting("AppBaseDomain");
            var tourTitle = await this.GetResourceString(nameof(Resources.WelcomeTourTitle));
            var appId = CloudConfigurationManager.GetSetting("ManifestAppId");

            var welcomeData = new
            {
                textAlignment,
                personFirstName,
                botDisplayName,
                introMessagePart1,
                introMessagePart2,
                introMessagePart3,
                team = teamName,
                welcomeCardImageUrl = $"https://{baseDomain}/Content/welcome-card-image.png",
                optInText = await this.GetResourceString(nameof(Resources.OptInText)),
                optInButtonText = await this.GetResourceString(nameof(Resources.OptInButtonText)),
                tourUrl = GetTourFullUrl(appId, GetTourUrl(baseDomain), tourTitle),
                salutationText = await this.GetResourceString(nameof(Resources.SalutationTitleText)),
                tourButtonText = await this.GetResourceString(nameof(Resources.TakeATourButtonText)),
            };

            return GetCard(WelcomeNewMemberAdaptiveCardTemplate.Value, welcomeData);
        }

        /// <summary>
        /// Generates the adaptive card string for the unrecognized input.
        /// </summary>
        /// <returns>The adaptive card for the unrecognized input</returns>
        public async Task<Attachment> GetUnrecognizedInputCard()
        {
            // Set alignment of text based on default locale.
            var textAlignment = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? AdaptiveHorizontalAlignment.Right.ToString() : AdaptiveHorizontalAlignment.Left.ToString();

            var baseDomain = CloudConfigurationManager.GetSetting("AppBaseDomain");
            var tourTitle = await this.GetResourceString(nameof(Resources.WelcomeTourTitle));
            var appId = CloudConfigurationManager.GetSetting("ManifestAppId");

            var cardData = new
            {
                messageContent = await this.GetResourceString(nameof(Resources.UnrecognizedInput)),
                tourUrl = GetTourFullUrl(appId, GetTourUrl(baseDomain), tourTitle),
                tourButtonText = await this.GetResourceString(nameof(Resources.TakeATourButtonText)),
                textAlignment,
            };

            return GetCard(UnrecognizedInputAdaptiveCardTemplate.Value, cardData);
        }

        /// <summary>
        /// Creates the pairup notification card.
        /// </summary>
        /// <param name="teamName">The team name.</param>
        /// <param name="sender">The user who will be sending this card.</param>
        /// <param name="recipient">The user who will be receiving this card.</param>
        /// <param name="botDisplayName">The bot display name.</param>
        /// <param name="question">Icebreaker question</param>
        /// <returns>Pairup notification card</returns>
        public async Task<Attachment> GetPairUpNotificationCard(string teamName, TeamsChannelAccount sender, TeamsChannelAccount recipient, string botDisplayName, string question)
        {
            if (string.IsNullOrEmpty(teamName))
            {
                throw new ArgumentException($"'{nameof(teamName)}' cannot be null or empty.", nameof(teamName));
            }

            if (sender is null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (recipient is null)
            {
                throw new ArgumentNullException(nameof(recipient));
            }
            else if (string.IsNullOrEmpty(recipient.UserPrincipalName))
            {
                throw new ArgumentException($"'{nameof(recipient.UserPrincipalName)}' cannot be null or empty", nameof(recipient));
            }

            if (string.IsNullOrEmpty(botDisplayName))
            {
                throw new ArgumentException($"'{nameof(botDisplayName)}' cannot be null or empty.", nameof(botDisplayName));
            }

            if (string.IsNullOrEmpty(question))
            {
                throw new ArgumentException($"'{nameof(question)}' cannot be null or empty.", nameof(question));
            }

            // Set alignment of text based on default locale.
            var textAlignment = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? AdaptiveHorizontalAlignment.Right.ToString() : AdaptiveHorizontalAlignment.Left.ToString();

            // Guest users may not have their given name specified in AAD, so fall back to the full name if needed
            var senderGivenName = string.IsNullOrEmpty(sender.GivenName) ? sender.Name : sender.GivenName;
            var recipientGivenName = string.IsNullOrEmpty(recipient.GivenName) ? recipient.Name : recipient.GivenName;

            // To start a chat with a guest user, use their external email, not the UPN
            var recipientUpn = !IsGuestUser(recipient) ? recipient.UserPrincipalName : recipient.Email;

            var meetingTitle = string.Format(await this.GetResourceString(nameof(Resources.MeetupTitle)), senderGivenName, recipientGivenName);
            var meetingContent = string.Format(await this.GetResourceString(nameof(Resources.MeetupContent)), botDisplayName);
            var meetingLink = "https://teams.microsoft.com/l/meeting/new?subject=" + Uri.EscapeDataString(meetingTitle) + "&attendees=" + recipientUpn + "&content=" + Uri.EscapeDataString(meetingContent);

            var cardData = new
            {
                matchUpCardTitleContent = await this.GetResourceString(nameof(Resources.MatchUpCardTitleContent)),
                matchUpCardMatchedText = string.Format(await this.GetResourceString(nameof(Resources.MatchUpCardMatchedText)), recipient.Name),
                matchUpCardContentPart1 = string.Format(await this.GetResourceString(nameof(Resources.MatchUpCardContentPart1)), botDisplayName, teamName, recipient.Name),
                matchUpCardContentPart2 = await this.GetResourceString(nameof(Resources.MatchUpCardContentPart2)),
                MatchUpCardQuestion = string.Format(await this.GetResourceString(nameof(Resources.MatchUpCardQuestion)), question),
                chatWithMatchButtonText = string.Format(await this.GetResourceString(nameof(Resources.ChatWithMatchButtonText)), recipientGivenName),
                chatWithMessageGreeting = Uri.EscapeDataString(string.Format(await this.GetResourceString(nameof(Resources.ChatWithMessageGreeting)), question)),
                pauseMatchesButtonText = await this.GetResourceString(nameof(Resources.PausePairingsButtonText)),
                proposeMeetupButtonText = await this.GetResourceString(nameof(Resources.ProposeMeetupButtonText)),
                reportInactiveButtonText = await this.GetResourceString(nameof(Resources.ReportInactiveButtonText)),
                reportValue = new { id = recipient.Id },
                personUpn = recipientUpn,
                meetingLink,
                textAlignment,
            };

            return GetCard(PairUpNotificationAdaptiveCardTemplate.Value, cardData);
        }

        /// <summary>
        /// Checks whether or not the account is a guest user.
        /// </summary>
        /// <param name="account">The <see cref="TeamsChannelAccount"/> user to check.</param>
        /// <returns>True if the account is a guest user, false otherwise.</returns>
        private static bool IsGuestUser(TeamsChannelAccount account)
        {
            return account.UserPrincipalName.IndexOf(ExternallyAuthenticatedUpnMarker, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
    }
}