// <copyright file="AdaptiveCardBase.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Icebreaker.Helpers.AdaptiveCards
{
    using System;
    using System.Threading.Tasks;
    using global::AdaptiveCards;
    using global::AdaptiveCards.Templating;
    using Icebreaker.Services;
    using Microsoft.Bot.Schema;

    /// <summary>
    /// Builder class for the adaptive cards
    /// </summary>
    public class AdaptiveCardBase
    {

        private readonly ResourcesService resourcesService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdaptiveCardBase"/> class.
        /// </summary>
        /// <param name="resourcesService">Service to retrieve ResourceStrings.</param>
        protected AdaptiveCardBase(ResourcesService resourcesService)
        {
            this.resourcesService = resourcesService;
        }

        /// <summary>
        /// Return the ResourceString in current language.
        /// </summary>
        /// <param name="name">Name of the ResourceString</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        protected Task<string> GetResourceString(string name)
        {
            return this.resourcesService.GetResourceString(name);
        }

        /// <summary>
        /// Creates the adaptive card from by processing template and related data
        /// </summary>
        /// <param name="template">Adaptive template</param>
        /// <param name="cardData">card data to merge into template</param>
        /// <returns>Card attachment</returns>
        protected static Attachment GetCard(AdaptiveCardTemplate template, dynamic cardData)
        {
            // "Expand" the template - this generates the final Adaptive Card payload
            var cardJson = template.Expand(cardData);

            try
            {
                var welcomeCard = AdaptiveCard.FromJson(cardJson);
                return new Attachment
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = welcomeCard.Card,
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get full MSTeams tour action url
        /// </summary>
        /// <param name="appId">app id</param>
        /// <param name="htmlUrl">app template tour content url</param>
        /// <param name="tourTitle">Tour title</param>
        /// <returns>MSTeams tour action url</returns>
        protected static string GetTourFullUrl(string appId, string htmlUrl, string tourTitle)
        {
            htmlUrl = Uri.EscapeDataString(htmlUrl);
            tourTitle = Uri.EscapeDataString(tourTitle);
            return
                $"https://teams.microsoft.com/l/task/{appId}?url={htmlUrl}&height=533px&width=600px&title={tourTitle}";
        }

        /// <summary>
        /// Get app template tour content url
        /// </summary>
        /// <param name="baseDomain">App template base domain</param>
        /// <returns>Tour content url</returns>
        protected static string GetTourUrl(string baseDomain)
        {
            return $"https://{baseDomain}/tour?theme={{theme}}&locale={{locale}}";
        }
    }
}