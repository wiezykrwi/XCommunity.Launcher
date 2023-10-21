using Steamworks;

namespace SteamMods.Core.Configuration;

public class GameDirectoryLocator
{
    private const string UserConfigFolder = @"My Games\XCOM2 War of the Chosen\XComGame\Config";
    private const string DefaultConfigFolder = "Config";
    private const string GameFolder = @"XCom2-WarOfTheChosen\XComGame";

    public string GetBaseGameDirectory()
    {
        var steamAppIdFile = "steam_appid.txt";
        if (File.Exists(steamAppIdFile))
        {
            File.Delete(steamAppIdFile);
        }

        var appId = Constants.Identifiers.XCom2SteamId;
        File.WriteAllText(steamAppIdFile, appId.ToString());

        SteamAPI.Init();
        
        var appInstallDirResult = SteamApps.GetAppInstallDir((AppId_t)appId, out var gameDirectory, 260);
        if (appInstallDirResult <= 0)
        {
            throw new Exception("Could not locate steam app install folder");
        }

        return Path.Combine(gameDirectory, GameFolder);
    }

    public string GetUserConfigLocation(string filename)
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            UserConfigFolder,
            $"XCom{filename}.ini.test");
    }

    public string GetDefaultConfigLocation(string filename)
    {
        var gameDirectory = GetBaseGameDirectory();
        return Path.Combine(gameDirectory, DefaultConfigFolder, $"Default{filename}.ini");
    }
}