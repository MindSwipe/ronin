using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Octokit;
using Ronin.Updater;
using Ronin.Updater.Github;
using Ronin.Updater.Update;

namespace Ronin.Cli.Commands
{
    [Command]
    public class UpdateCommand : ICommand
    {
        [CommandOption("install-location", Description = "Specify the folder where Titanfall2.exe is located")]
        public string TitanfallInstallLocation { get; init; } = @"C:\Program Files (x86)\Origin Games\Titanfall2";

        [CommandOption("ignore-files", Description = "Ignore files")]
        public IReadOnlyList<FileInfo> IgnoreFiles { get; init; } = new List<FileInfo>();

        public async ValueTask ExecuteAsync(IConsole console)
        {
            var ghClient = new GitHubClient(new ProductHeaderValue("MindSwipe"));
            var githubDownloader = new GithubDownloader(ghClient, new RepoConfiguration
            {
                RepoName = "Northstar",
                RepoOwner = "R2Northstar"
            });

            await console.Output.WriteAsync("Downloading latest release...");
            var downloadFile = await githubDownloader.DownloadLatestReleaseAssets(CancellationToken.None);
            await console.Output.WriteLineAsync(" done!");
            foreach (var file in Directory.GetFiles(downloadFile, "*.zip"))
            {
                await console.Output.WriteLineAsync($"Unpacking {Path.GetFileName(file)}");
                Unpacker.UnpackIntoSameFolderZip(file);
            }

            var sourceFiles = Directory.GetFiles(downloadFile);
            var result = new List<(string, string)>();
            foreach (var sourceFile in sourceFiles)
            {
                var targetFiles = Directory.GetFiles(TitanfallInstallLocation, Path.GetFileName(sourceFile));
                if (targetFiles.Length != 1)
                    continue;

                result.Add(new(sourceFile, targetFiles[0]));
            }

            var collisions = await FileCopier.GetCollidingFiles(result);
            if (collisions.Count == 0)
            {
                await console.Output.WriteLineAsync("Local files already up to date");
                return;
            }

            foreach (var (sourceFile, targetFile) in collisions)
            {
                if (IgnoreFiles.Select(x => Path.Combine(TitanfallInstallLocation, x.Name)).Contains(Path.GetFileName(targetFile)))
                {
                    await console.Output.WriteLineAsync($"Ignoring {sourceFile}");
                    continue;
                }

                await console.Output.WriteLineAsync($"Updating {Path.GetFileName(targetFile)}");
                File.Delete(targetFile);
                File.Copy(sourceFile, targetFile);
            }
        }
    }
}
