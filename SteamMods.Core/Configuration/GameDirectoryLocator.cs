using Steamworks;

namespace SteamMods.Core.Configuration;

public class GameDirectoryLocator
{
    public string? Locate(uint appId)
    {
        var appInstallDirResult = SteamApps.GetAppInstallDir((AppId_t)appId, out var gameDirectory, 260);
        return appInstallDirResult <= 0 ? null : gameDirectory;
    }

    public string? GetBaseGameDirectory(uint appId)
    {
        var steamAppIdFile = "steam_appid.txt";
        if (File.Exists(steamAppIdFile))
        {
            File.Delete(steamAppIdFile);
        }
        
        File.WriteAllText(steamAppIdFile, appId.ToString());

        SteamAPI.Init();
        
        var appInstallDirResult = SteamApps.GetAppInstallDir((AppId_t)appId, out var gameDirectory, 260);
        if (appInstallDirResult <= 0)
        {
            return null;
        }

        return Path.Combine(gameDirectory, @"XCom2-WarOfTheChosen\XComGame");
    }
}