using Icebreaker.Interfaces;
using Icebreaker.Properties;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Icebreaker.Services
{
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
        public ResourcesService(IBotDataProvider dataProvider, TelemetryClient telemetryClient)
        {
            this.dataProvider = dataProvider;
            this.telemetryClient = telemetryClient;
            this.resourceManager = new ResourceManager("Resources", typeof(Icebreaker.Properties.Resources).Assembly);
        }


        public async Task<string> GetResourceString(string language, string name)
        {
            string resource = null;
            try
            {
                resource = await this.dataProvider.GetResourceStringAsync(language, name);
            }
            finally
            {
                if (string.IsNullOrEmpty(resource))
                {
                    resource = this.resourceManager.GetString(name: name, culture: CultureInfo.CreateSpecificCulture(language));
                }

                if (string.IsNullOrEmpty(resource))
                {
                    resource = this.resourceManager.GetString(name);
                }
            }

            return resource;
        }
    }
}
