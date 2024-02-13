namespace Xcommunity.Launcher.Core.Configuration;

public class IniFile
{
    public Dictionary<string, Dictionary<string, List<string>>> Sections { get; init; }

    public bool HasValue(string section, string key)
    {
        return Sections.ContainsKey(section) && Sections[section].ContainsKey(key);
    }

    public IReadOnlyCollection<string> GetValues(string section, string key)
    {
        if (!HasValue(section, key)) throw new ArgumentException($"No values present for {section}, {key}");

        return Sections[section][key];
    }
}