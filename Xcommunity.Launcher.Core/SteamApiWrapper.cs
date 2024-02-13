using Steamworks;

namespace Xcommunity.Launcher.Core;

public static class SteamApiWrapper
{
    private static readonly object Mutex = new();

    public static void RunCallbacks()
    {
        lock (Mutex)
        {
            SteamAPI.RunCallbacks();
        }
    }

    public static void Shutdown()
    {
        lock (Mutex)
        {
            SteamAPI.Shutdown();
        }
    }

    public static bool Init()
    {
        lock (Mutex)
        {
            return SteamAPI.Init();
        }
    }
}