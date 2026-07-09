using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitHub;
using Octokit;
using Serilog;

using INukeBuild = Nuke.Common.INukeBuild;
using Target = Nuke.Common.Target;

namespace Components;

public partial interface ICreateGithubRelease : INukeBuild {
    private static readonly string[] CommitTypes = [
        "feat",
        "fix",
        "refactor",
        "perf",
        "chore",
        "ci",
        "release"
    ];
    
    private static readonly (string Type, string Title)[] Sections = [
        ("feat", "## ✨ Features"),
        ("fix", "## 🐛 Bug Fixes"),
        ("refactor", "## ♻️ Refactoring"),
        ("perf", "## ⚡ Performance"),
        ("chore", "## 🔧 Chores"),
        ("ci", "## 🤖 CI"),
        ("release", "## 🚀 Release"),
        ("other", "## 📦 Other")
    ];
     
    public const string GitHubRelease = nameof(GitHubRelease);

    [GitRepository] [Required] GitRepository GitRepository => TryGetValue(() => GitRepository);
    [Parameter] [Secret] string GitHubToken => TryGetValue(() => GitHubToken) ?? GitHubActions.Instance?.Token;

    [Parameter] bool Prerelease => TryGetValue<bool>(() => Prerelease);

    string Name { get; }
    
    bool Draft => false;

    IEnumerable<AbsolutePath> AssetFiles { get; }

    Target CreateGitHubRelease => _ => _
        .Requires(() => GitHubToken)
        .OnlyWhenDynamic(ShouldRelease)
        .Executes(async () => {
            Log.Information("{Prerelease}", Prerelease);
            GitHubTasks.GitHubClient.Credentials = new Credentials(GitHubToken.NotNull());
            Log.Information("Starting create release...");
            
            var releaseName = await GetReleaseNameAsync();
            var release = await GetOrCreateReleaseAsync(releaseName);
            
            await Task.WhenAll(AssetFiles.Select(async file => {
                await using var stream = File.OpenRead(file);
                
                var fileName = string.Format(file.Name, releaseName);
        
                await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(
                    release,
                    new ReleaseAssetUpload {
                        FileName = fileName,
                        ContentType = "application/octet-stream",
                        RawData = stream
                    });
                
                Log.Information("{Name} uploaded successfully!", fileName);
            }));
            
            return;
        
            async Task<string> GetReleaseNameAsync() {
                if (!Prerelease)
                    return Name;
        
                var tags = await GitHubTasks.GitHubClient.Repository.GetAllTags(
                    GitRepository.GetGitHubOwner(),
                    GitRepository.GetGitHubName());
        
                var nextPreview = tags
                    .Where(x => x.Name.StartsWith($"{Name}-preview", StringComparison.OrdinalIgnoreCase))
                    .Select(x => {
                        var match = SuffixRegex().Match(x.Name);
                        return match.Success
                            ? int.Parse(match.Groups[1].Value)
                            : 0;
                    })
                    .DefaultIfEmpty()
                    .Max() + 1;
        
                return $"{Name}-preview{nextPreview}"; 
            }

            async Task<Release> GetOrCreateReleaseAsync(string releaseName) {
                try {
                    Log.Information("Creating {Name}...", releaseName);
                    
                    return await GitHubTasks.GitHubClient.Repository.Release.Create(
                        GitRepository.GetGitHubOwner(),
                        GitRepository.GetGitHubName(),
                        new NewRelease(releaseName) {
                            Name = releaseName,
                            Draft = Draft,
                            Prerelease = Prerelease,
                            Body = BuildLog()
                        });
                }
                catch (ApiValidationException ex)
                    when (ex.ApiError?.Errors.Any(x =>
                        x.Code == "already_exists" &&
                        x.Field == "tag_name") == true) {
                    Log.Information("Release already exists, loading existing release...");
        
                    return await GitHubTasks.GitHubClient.Repository.Release.Get(
                        GitRepository.GetGitHubOwner(),
                        GitRepository.GetGitHubName(),
                        releaseName);
                }
            }

            string BuildLog() {
                var commitInfos = GetCommitInfos();
                var groups = commitInfos
                    .GroupBy(x => x.Item1)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Item2).ToList());

                var sb = new StringBuilder();

                sb.AppendLine("## What's Changed");
                sb.AppendLine();

                foreach (var (type, title) in Sections) {
                    if (!groups.TryGetValue(type, out var items) || items.Count == 0)
                        continue;
                    
                    sb.AppendLine(title);

                    foreach (var item in items)
                        sb.AppendLine(item);

                    sb.AppendLine();
                }

                return sb.ToString().TrimEnd();
            }
        });
    
    private bool ShouldRelease() {
        var commits = GetCommitInfos();

        return commits.Any(x =>
            Enumerable.Contains(CommitTypes, x.Item1) &&
            x.Item1 != "chore" &&
            x.Item1 != "ci");
    }
    
    private string GetPreviousTag() {
        try {
            return GitTasks
                .Git("describe --tags --abbrev=0 HEAD^")
                .FirstOrDefault()
                .Text;
        }
        catch (ProcessException) {
            return null;
        }
    }

    private IEnumerable<(string, string)> GetCommitInfos() {
        var previousTag = GetPreviousTag();

        Log.Information("Generate release notes from {Tag} to HEAD", previousTag);
        Log.Information("Prerelease: {Prerelease}", Prerelease);
        
        var range = previousTag is null
            ? "HEAD"
            : $"{previousTag}..HEAD";

        var infos = GitTasks
            .Git($"log {range} --pretty=format:\"%h|%s|@%an\"")
            .Select(x => x.Text);

        foreach (var info in infos) {
            var parts = info.Split('|', 3);
            if (parts.Length != 3)
                continue;

            var type = CommitTypes.FirstOrDefault(parts[1].StartsWith) ?? "other";
            yield return (type, $"- {parts[0]}: {CCRegex().Replace(parts[1], "").Trim()} (by {parts[2]})");
        }
    }
    
    [GeneratedRegex(@"preview(\d+)$")]
    private static partial Regex SuffixRegex();
    
    [GeneratedRegex(@"^(feat|fix|refactor|perf|chore|ci|release)(\([^)]+\))?!?[:：][\s\u3000]*")]
    private static partial Regex CCRegex();
}