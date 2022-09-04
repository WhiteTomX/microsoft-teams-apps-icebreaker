// <copyright file="ResourcesService.cs" company="WhiteTom">
// Copyright (c) WhiteTom.
// Licensed under the MIT License.
// </copyright>

namespace Icebreaker.Services
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Runtime.Caching;
    using System.Threading.Tasks;
    using Icebreaker.Interfaces;
    using Icebreaker.Properties;
    using Microsoft.ApplicationInsights;

    /// <summary>
    /// Service to get ResourceStrings
    /// </summary>
    public class ResourcesService
    {
        private readonly IBotDataProvider dataProvider;
        private readonly TelemetryClient telemetryClient;
        private readonly ResourceManager resourceManager;
        private readonly MemoryCache cache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcesService"/> class.
        /// </summary>
        /// <param name="dataProvider">Client to use to access Database</param>
        /// <param name="telemetryClient">Client to send telemetry</param>
        public ResourcesService(IBotDataProvider dataProvider, TelemetryClient telemetryClient)
        {
            this.dataProvider = dataProvider;
            this.telemetryClient = telemetryClient;
            this.resourceManager = Resources.ResourceManager;
            this.cache = new MemoryCache(nameof(ResourcesService));
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
            bool fromCache = true;
            string cacheKey = $"{language}-{name}";
            resource = await this.TryGetResource(() => Task.FromResult(this.cache.Get(cacheKey)?.ToString()));
            if (string.IsNullOrEmpty(resource))
            {
                fromCache = false;
                resource = await this.TryGetResource(async () => await this.dataProvider.GetResourceStringAsync(language, name));
                this.telemetryClient.TrackTrace($"Got ResourceString '{resource}' for {name} in {language} from db");
            }

            if (string.IsNullOrEmpty(resource))
            {
                resource = await this.TryGetResource(() => Task.FromResult(this.resourceManager.GetString(name: name, culture: CultureInfo.CreateSpecificCulture(language))));
            }

            if (string.IsNullOrEmpty(resource))
            {
                resource = await this.TryGetResource(() => Task.FromResult(this.resourceManager.GetString(name)));
            }

            if (!fromCache)
            {
                this.cache.Set(cacheKey, resource, DateTime.Now.AddSeconds(60));
            }


            this.telemetryClient.TrackTrace($"Final resourceString for {name} in {language}: {resource}");

            return resource;
        }

        private async Task<string> TryGetResource(Func<Task<string>> retrieve)
        {
            try
            {
                return await retrieve();
            }
            catch
            {
                return null;
            }
        }
    }
}
