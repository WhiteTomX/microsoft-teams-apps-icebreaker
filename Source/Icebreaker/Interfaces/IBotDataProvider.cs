﻿// <copyright file="IBotDataProvider.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// </copyright>

namespace Icebreaker.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Icebreaker.Helpers;

    /// <summary>
    /// Data provider routines
    /// </summary>
    public interface IBotDataProvider
    {
        /// <summary>
        /// Get the list of teams to which the app was installed.
        /// </summary>
        /// <returns>List of installed teams</returns>
        Task<IList<TeamInstallInfo>> GetInstalledTeamsAsync();

        /// <summary>
        /// Get the stored information about given users
        /// </summary>
        /// <returns>User information</returns>
        Task<Dictionary<string, bool>> GetAllUsersOptInStatusAsync();

        /// <summary>
        /// Returns the team that the bot has been installed to
        /// </summary>
        /// <param name="teamId">The team id</param>
        /// <returns>Team that the bot is installed to</returns>
        Task<TeamInstallInfo> GetInstalledTeamAsync(string teamId);

        /// <summary>
        /// Updates team installation status in store. If the bot is installed, the info is saved, otherwise info for the team is deleted.
        /// </summary>
        /// <param name="team">The team installation info</param>
        /// <param name="installed">Value that indicates if bot is installed</param>
        /// <returns>Tracking task</returns>
        Task UpdateTeamInstallStatusAsync(TeamInstallInfo team, bool installed);

        /// <summary>
        /// Set the user info for the given user
        /// </summary>
        /// <param name="tenantId">Tenant id</param>
        /// <param name="userId">User id</param>
        /// <param name="optedIn">User opt-in status</param>
        /// <param name="serviceUrl">User service URL</param>
        /// <returns>Tracking task</returns>
        Task SetUserInfoAsync(string tenantId, string userId, bool optedIn, string serviceUrl);

        /// <summary>
        /// Get questions for language
        /// </summary>
        /// <param name="language">Language to get the questions in</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task<string[]> GetQuestionsAsync(string language);

        /// <summary>
        /// Set questions for language
        /// </summary>
        /// <param name="language">Language to create question for</param>
        /// <param name="questions">Questions to set for language</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task SetQuestionsAsync(string language, string[] questions);

        /// <summary>
        /// Get ResourceStrings from Database
        /// </summary>
        /// <param name="language">The language to get the resource in</param>
        /// <param name="name">The name of the ResourceString to get</param>
        /// <returns>A <see cref="Task"/> containing the value of the resource string eventualy.</returns>
        Task<string> GetResourceStringAsync(string language, string name);
    }
}