namespace SteamMods.Core.Configuration;

public class ConfigurationHelper
{
    public async Task<IReadOnlyCollection<string>> FindModFolders()
    {
        var configurationFileLocator = new ConfigurationFileLocator();
        var enginePath = configurationFileLocator.GetEnginePath();

        var iniFileReader = new IniFileReader();
        var iniFile = await iniFileReader.Read(enginePath);

        return iniFile.HasValue(Keys.Sections.Engine.DownloadableContentEnumerator, Keys.Values.ModPath)
            ? iniFile.GetValues(Keys.Sections.Engine.DownloadableContentEnumerator, Keys.Values.ModPath)
            : new List<string>();
    }
}