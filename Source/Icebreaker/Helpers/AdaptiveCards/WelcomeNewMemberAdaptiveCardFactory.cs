// <copyright file="WelcomeNewMemberAdaptiveCardFactory.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Icebreaker.Helpers.AdaptiveCards
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using global::AdaptiveCards;
    using global::AdaptiveCards.Templating;
    using Icebreaker.Properties;
    using Icebreaker.Services;
    using Microsoft.Azure;
    using Microsoft.Bot.Schema;

    /// <summary>
    /// Builder class for the welcome new member card
    /// </summary>
    public class WelcomeNewMemberAdaptiveCardFactory : AdaptiveCardBase
    {
        private readonly Lazy<AdaptiveCardTemplate> adaptiveCardTemplate =
            new Lazy<AdaptiveCardTemplate>(() => CardTemplateHelper.GetAdaptiveCardTemplate(AdaptiveCardName.WelcomeNewMember));

        private readonly ResourcesService resourceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeNewMemberAdaptiveCard"/> class.
        /// </summary>
        /// <param name="resourcesService">The resources Service to get the ResourceStrings.</param>
        public WelcomeNewMemberAdaptiveCardFactory(ResourcesService resourcesService)
        {
            this.resourceService = resourcesService;
        }

        /// <summary>
        /// Creates the welcome new member card.
        /// </summary>
        /// <param name="teamName">The team name</param>
        /// <param name="personFirstName">The first name of the new member</param>
        /// <param name="botDisplayName">The bot name</param>
        /// <param name="botInstaller">The person that installed the bot to the team</param>
        /// <returns>The welcome new member card</returns>
        public async Task<Attachment> GetCard(string teamName, string personFirstName, string botDisplayName, string botInstaller)
        {
            // Set alignment of text based on default locale.
            var textAlignment = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? AdaptiveHorizontalAlignment.Right.ToString() : AdaptiveHorizontalAlignment.Left.ToString();

            string introMessagePart1;
            string introMessagePart2;
            string introMessagePart3 = await this.resourceService.GetResourceString(System.Globalization.CultureInfo.CurrentCulture.NativeName, nameof(Resources.OptInText));

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

            return GetCard(this.adaptiveCardTemplate.Value, welcomeData);
        }

        private async Task<string> GetResourceString(string name)
        {
            return await this.resourceService.GetResourceString(System.Globalization.CultureInfo.CurrentCulture.NativeName, name);
        }
    }
}