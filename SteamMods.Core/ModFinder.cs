using System.Text.RegularExpressions;
using SteamMods.Core.Configuration;

namespace SteamMods.Core;

public class ModFinder
{
    private readonly IniFileReader _reader;
    private readonly GameDirectoryLocator _gameDirectoryLocator;
    private readonly Regex _titleRegex = new Regex(@"^Title=(?<TITLE>.*)$");

    public ModFinder()
    {
        _gameDirectoryLocator = new GameDirectoryLocator();
        _reader = new IniFileReader(_gameDirectoryLocator);
    }
    
    public async Task<IReadOnlyCollection<ModData>> FindModsAsync()
    {
        var modDirs = await GetModDirectories();

        var modDataList = new List<ModData>();

        foreach (var modDirectory in modDirs.Where(Directory.Exists).SelectMany(Directory.GetDirectories))
        {
            var modDataFile = Directory.GetFiles(modDirectory, "*.xcommod").SingleOrDefault();
            if (modDataFile is null)
            {
                continue;
            }

            await using var fileStream = File.OpenRead(modDataFile);
            using var streamReader = new StreamReader(fileStream);
            while (!streamReader.EndOfStream)
            {
                var line = await streamReader.ReadLineAsync();
                if (line == null) continue;
                var match = _titleRegex.Match(line);
                if (!match.Success) continue;
                modDataList.Add(new ModData
                {
                    Title = match.Groups["TITLE"].Value,
                    Location = modDirectory
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