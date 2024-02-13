using System.Text.RegularExpressions;
using Xcommunity.Launcher.Core.Configuration;

namespace Xcommunity.Launcher.Core;

public class ModFinder
{
    private readonly GameDirectoryLocator _gameDirectoryLocator;
    private readonly IniFileReader _reader;
    private readonly Regex _titleRegex = new(@"^Title=(?<TITLE>.*)$");

    public ModFinder()
    {
        _gameDirectoryLocator = new GameDirectoryLocator();
        _reader = new IniFileReader(_gameDirectoryLocator);
    }

    public async Task<IReadOnlyCollection<ModData>> FindModsAsync()
    {
        var modDirectories = await GetModDirectories();
        var modDirs = modDirectories.Select(x => new DirectoryInfo(x));

        var modDataList = new List<ModData>();

        foreach (var modDirectory in modDirs.Where(x => x.Exists).SelectMany(x => x.GetDirectories()))
        {
            var modDataFile = modDirectory.GetFiles("*.xcommod").SingleOrDefault();
            if (modDataFile is null) continue;

            if (!ulong.TryParse(modDirectory.Name, out var modId))
            {
                continue;
            }

            await using var fileStream = File.OpenRead(modDataFile.FullName);
            using var streamReader = new StreamReader(fileStream);
            while (!streamReader.EndOfStream)
            {
                var line = await streamReader.ReadLineAsync();
                if (line == null) continue;
                var match = _titleRegex.Match(line);
                if (!match.Success) continue;
                modDataList.Add(new ModData
                {
                    Id = modId,
                    Title = match.Groups["TITLE"].Value,
                    Location = modDirectory.FullName
                });

                break;
            }
        }

        return modDataList;
    }

    private async Task<List<string>> GetModDirectories()
    {
        var result = await _reader.Read(Constants.Files.Engine);
        var baseDirectory = _gameDirectoryLocator.GetBaseGameDirectory();
        var modDirs = result.Sections["Engine.DownloadableContentEnumerator"]["ModRootDirs"]
            .Select(path => Path.Combine(baseDirectory, path))
            .ToList();
        var workshopModDir = Path.GetFullPath(Path.Combine(baseDirectory, "../../../../workshop/content/268500"));
        modDirs.Add(workshopModDir);
        return modDirs;
    }
}