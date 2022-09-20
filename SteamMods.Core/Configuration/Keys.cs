namespace SteamMods.Core.Configuration;

public static class Keys
{
    public static class Sections
    {
        public const string Configuration = nameof(Configuration);

        public static class Engine
        {
            public const string DownloadableContentEnumerator = $"{nameof(Engine)}.{nameof(DownloadableContentEnumerator)}";
        }
    }

    public static class Values
    {
        public const string BasedOn = nameof(BasedOn);
        public const string ModPath = nameof(ModPath);
    }
}