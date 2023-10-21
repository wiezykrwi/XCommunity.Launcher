namespace SteamMods.Core.Configuration;

public static class Constants
{
    public static class Files
    {
        public const string Engine = nameof(Engine);
        public const string ModOptions = nameof(ModOptions);
    }

    public static class Identifiers
    {
        public const uint XCom2SteamId = 268500;
    }
    
    public static class Sections
    {
        public const string Configuration = nameof(Configuration);
        public const string IniVersion = nameof(IniVersion);

        public class ModOptions
        {
            public const string XComModOptions = $"{nameof(ModOptions)}.{nameof(XComModOptions)}";
        }

        public static class Engine
        {
            public const string DownloadableContentEnumerator = $"{nameof(Engine)}.{nameof(DownloadableContentEnumerator)}";
        }
    }

    public static class Values
    {
        public const string BasedOn = nameof(BasedOn);
        public const string ModPath = nameof(ModPath);
        public const string ActiveMods = nameof(ActiveMods);
    }
}