using System.Text.RegularExpressions;

namespace SteamMods.Cli;

public class IniFile
{
    public Dictionary<string, Dictionary<string, string>> Sections { get; init; }
}

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
                var sectionHeaderName = sectionHeaderMatch.Groups["NAME"].Value;
                if (sections.ContainsKey(sectionHeaderName))
                {
                    currentSection = sections[sectionHeaderName];
                }
                else
                {
                    currentSection = new Dictionary<string, string>();
                    sections.Add(sectionHeaderName, currentSection);
                }
                continue;
            }

            var keyValuePairMatch = _keyValuePairRegex.Match(line);
            if (!keyValuePairMatch.Success)
            {
                continue;
            }

            var currentKey = keyValuePairMatch.Groups["KEY"].Value;
            var currentValue = keyValuePairMatch.Groups["VALUE"].Value;

            while (currentValue.Length > 2 && currentValue.Substring(currentValue.Length - 2) == "\\\\")
            {
                currentValue = currentValue.Substring(0, currentValue.Length - 2) + "\n" + await reader.ReadLineAsync();
            }

            currentSection.Add(currentKey.TrimEnd(), currentValue.TrimStart());
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
}