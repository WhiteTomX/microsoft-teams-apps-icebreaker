// <copyright file="ResourcesService.cs" company="WhiteTom">
// Copyright (c) WhiteTom.
// Licensed under the MIT License.
// </copyright>

namespace Icebreaker.Services
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Threading.Tasks;
    using Icebreaker.Interfaces;
    using Microsoft.ApplicationInsights;

    /// <summary>
    /// Service to get ResourceStrings
    /// </summary>
    public class ResourcesService
    {
        private readonly IBotDataProvider dataProvider;
        private readonly TelemetryClient telemetryClient;
        private readonly ResourceManager resourceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesService"/> class.
        /// </summary>
        /// <param name="dataProvider">Client to use to access Database</param>
        /// <param name="telemetryClient">Client to send telemetry</param>
        public ResourcesService(IBotDataProvider dataProvider, TelemetryClient telemetryClient)
        {
            this.dataProvider = dataProvider;
            this.telemetryClient = telemetryClient;
            this.resourceManager = new ResourceManager("Resources", typeof(Icebreaker.Properties.Resources).Assembly);
        }

        /// <summary>
        /// Get ResourceString from database, fallback to Resources
        /// </summary>
        /// <param name="language">Language to get the Resource String in</param>
        /// <param name="name">Name of the ResourceString to get</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<string> GetResourceString(string language, string name)
        {
            this.telemetryClient.TrackTrace($"Get ResourceString for {name} in {language}");
            string resource = null;
            try
            {
                resource = await this.dataProvider.GetResourceStringAsync(language, name);
                this.telemetryClient.TrackTrace($"Got ResourceString '{resource}' for {name} in {language} from db");
            }
            finally
            {
                try
                {
                    if (string.IsNullOrEmpty(resource))
                    {
                        resource = this.resourceManager.GetString(name: name, culture: CultureInfo.CreateSpecificCulture(language));
                    }
                }
                finally
                {
                    if (string.IsNullOrEmpty(resource))
                    {
                        resource = this.resourceManager.GetString(name);
                    }
                }
            }

            this.telemetryClient.TrackTrace($"Final resourceString for {name} in {language}: {resource}");
            return resource;
        }
    }
}
