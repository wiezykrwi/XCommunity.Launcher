using Steamworks;

namespace SteamMods.Core.Configuration;

public class GameDirectoryLocator
{
    public string? Locate(uint appId)
    {
        var appInstallDirResult = SteamApps.GetAppInstallDir((AppId_t)appId, out var gameDirectory, 260);
        return appInstallDirResult <= 0 ? null : gameDirectory;
    }
}