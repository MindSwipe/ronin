using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Octokit;

namespace Ronin.Updater.Github
{
    public class GithubDownloader
    {
        private readonly GitHubClient _ghClient;
        private readonly RepoConfiguration _repoConfig;


        public GithubDownloader(GitHubClient ghClient, RepoConfiguration repoConfig)
        {
            _ghClient = ghClient;
            _repoConfig = repoConfig;
        }

        public async Task<Release> GetLatestNorthstarRelease()
        {
            return await _ghClient.Repository.Release.GetLatest(_repoConfig.RepoOwner, _repoConfig.RepoName);
        }

        public async Task<string> DownloadLatestReleaseAssets(CancellationToken ct)
        {
            var latestRelease = await GetLatestNorthstarRelease();
            var target = AppDataCreator.ClearAndGetDownloadFolder();

            foreach (var asset in latestRelease.Assets)
            {
                var response = await _ghClient.Connection.Get<byte[]>(new Uri(asset.Url), new Dictionary<string, string>(), "application/octet-stream", ct);
                if (response.HttpResponse.StatusCode != HttpStatusCode.OK)
                    continue;

                await using var fs = File.Create(Path.Combine(target, asset.Name));
                await fs.WriteAsync(response.Body, 0, response.Body.Length, ct);
            }

            return target;
        }
    }
}
