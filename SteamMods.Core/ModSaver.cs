using SteamMods.Core.Configuration;

namespace SteamMods.Core;

public class ModSaver
{
    public async Task SaveAsync(IReadOnlyCollection<ModData> mods)
    {
        var gameDirectoryLocator = new GameDirectoryLocator();
        var modOptionsUserConfigLocation = gameDirectoryLocator.GetUserConfigLocation(Constants.Files.ModOptions);
        var modOptionsFileWriter = new IniFileWriter(modOptionsUserConfigLocation);
        var timestamp = File.GetLastWriteTimeUtc(gameDirectoryLocator.GetDefaultConfigLocation(Constants.Files.ModOptions));

        var modOptions = CreateModOptions();

        var activeMods = modOptions.Sections[Constants.Sections.ModOptions.XComModOptions][Constants.Values.ActiveMods];
        foreach (var mod in mods)
        {
            activeMods.Add(mod.Title);
        }

        await modOptionsFileWriter.WriteAsync(modOptions, timestamp);
    }

    private static IniFile CreateModOptions()
    {
        return new IniFile
        {
            Sections = new Dictionary<string, Dictionary<string, List<string>>>
            {
                {
                    Constants.Sections.ModOptions.XComModOptions, new Dictionary<string, List<string>>
                    {
                        { Constants.Values.ActiveMods , new List<string>() }
                    }
                }
            }
        };
    }
}