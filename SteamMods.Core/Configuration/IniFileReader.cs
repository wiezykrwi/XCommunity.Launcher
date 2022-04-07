using System.Text.RegularExpressions;

namespace SteamMods.Core.Configuration;

public class IniFileReader
{
    private readonly Regex _sectionHeaderRegex = new(@"^\[(?<NAME>.*)\]$");
    private readonly Regex _keyValuePairRegex = new(@"(?<KEY>.*)=(?<VALUE>.*)");

    public async Task<IniFile> Read(string path)
    {
        var sections = new Dictionary<string, Dictionary<string, List<string>>>();

        await using var stream = File.Open(path, FileMode.Open);
        using var reader = new StreamReader(stream);

        var unknownSection = new Dictionary<string, List<string>>();
        var currentSection = unknownSection;

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            line = line?.Trim();

            if (string.IsNullOrEmpty(line) || line[0] == ';')
                continue;

            var sectionHeaderMatch = _sectionHeaderRegex.Match(line);
            if (sectionHeaderMatch.Success)
            {
                currentSection = AddNewSection(sections, sectionHeaderMatch.Groups["NAME"].Value);
                continue;
            }

            var keyValuePairMatch = _keyValuePairRegex.Match(line);
            if (!keyValuePairMatch.Success)
            {
                continue;
            }

            await ProcessKeyValuePair(keyValuePairMatch, reader, currentSection);
        }

        if (unknownSection.Count > 0)
        {
            sections.Add("unknown", unknownSection);
        }

        return new IniFile
        {
            Sections = sections
        };
    }

    private static async Task ProcessKeyValuePair(Match keyValuePairMatch, StreamReader reader, Dictionary<string, List<string>> currentSection)
    {
        var currentKey = keyValuePairMatch.Groups["KEY"].Value.TrimEnd();
        var currentValue = keyValuePairMatch.Groups["VALUE"].Value;

        while (currentValue.Length > 2 && currentValue.Substring(currentValue.Length - 2) == "\\\\")
        {
            currentValue = currentValue.Substring(0, currentValue.Length - 2) + "\n" + await reader.ReadLineAsync();
        }

        AddToSection(currentSection, currentKey, currentValue);
    }

    private static Dictionary<string, List<string>> AddNewSection(Dictionary<string, Dictionary<string, List<string>>> sections, string sectionHeaderName)
    {
        if (sections.ContainsKey(sectionHeaderName))
        {
            return sections[sectionHeaderName];
        }

        var currentSection = new Dictionary<string, List<string>>();
        sections.Add(sectionHeaderName, currentSection);

        return currentSection;
    }

    private static void AddToSection(Dictionary<string, List<string>> currentSection, string currentKey, string currentValue)
    {
        if (!currentSection.ContainsKey(currentKey))
        {
            currentSection.Add(currentKey, new List<string>
            {
                currentValue.TrimStart()
            });
        }
        else
        {
            currentSection[currentKey].Add(currentValue);
        }
    }
}