// <copyright file="ResourcesServiceTests.cs" company="WhiteTom">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Icebreaker.Tests.ServicesTests
{
    using Icebreaker.Interfaces;
    using Icebreaker.Properties;
    using Icebreaker.Services;
    using Moq;
    using System.Globalization;
    using System.Threading.Tasks;
    using Xunit;
    public class ResourcesServiceTests
    {
        private readonly Mock<IBotDataProvider> dataProvider;
        private readonly ResourcesService resourcesService;
        public ResourcesServiceTests()
        {
            this.dataProvider = new Mock<IBotDataProvider>();
            this.resourcesService = new ResourcesService(this.dataProvider.Object, new Microsoft.ApplicationInsights.TelemetryClient());
        }

        /// <summary>
        /// The value for the given langage from database should be returned first
        /// </summary>
        public async void ShouldReturnValueFromDatabaseFirst()
        {
            const string value = "This is us!";
            this.dataProvider.Setup(d => d.GetResourceStringAsync(It.IsAny<string>(), "Test"))
                .Returns((string language, string name) => Task.FromResult(value));

            var resource = await this.resourcesService.GetResourceString(CultureInfo.CurrentCulture.NativeName, "Test");

            Assert.Equal(value, resource);
        }

        /// <summary>
        /// If there is a failure in Database the Resource should be retrieved from File for given language
        /// </summary>
        public async void ShouldReturnValueFromResourcesIfDatabaseFails()
        {
            var name = nameof(Resources.BackButtonText);
            var culture = CultureInfo.CurrentCulture.NativeName;
            this.dataProvider.Setup(d => d.GetResourceStringAsync(It.IsAny<string>(), name))
                .Throws(new System.Exception());

            var resource = await this.resourcesService.GetResourceString(culture, name);

            Assert.Equal(Resources.BackButtonText, resource);
        }

        /// <summary>
        /// If name not found in Database the Resource should be retrieved from File for given language
        /// </summary>
        public async void ShouldReturnValueFromResourcesIfDatabaseNotFound()
        {
            var name = nameof(Resources.BackButtonText);
            var culture = CultureInfo.CurrentCulture.NativeName;
            this.dataProvider.Setup(d => d.GetResourceStringAsync(It.IsAny<string>(), name))
                .ReturnsAsync((string x, string y) => null);

            var resource = await this.resourcesService.GetResourceString(culture, name);

            Assert.Equal(Resources.BackButtonText, resource);
        }
    }
}
