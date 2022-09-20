using System.Text.RegularExpressions;

namespace SteamMods.Core.Configuration;

public class IniFileReader
{
    private readonly Regex _sectionHeaderRegex = new(@"^\[(?<NAME>.*)\]$");
    private readonly Regex _keyValuePairRegex = new(@"(?<KEY>.*)=(?<VALUE>.*)");
    private readonly Regex _keyModificationRegex = new(@"(?<OPERATION>.)(?<KEY>.*)");

    public async Task<IniFile?> Read(string path)
    {
        var gameDirectoryLocator = new GameDirectoryLocator();
        var baseDirectory = gameDirectoryLocator.GetBaseGameDirectory(268500);
        if (baseDirectory == null)
        {
            return null;
        }

        var iniFilePath = Path.Combine(baseDirectory, path);
        var sections = await ReadSectionsFromFile(iniFilePath);

        if (sections.ContainsKey(Keys.Sections.Configuration) && sections[Keys.Sections.Configuration].ContainsKey(Keys.Values.BasedOn))
        {
            var baseIniFilePath = Path.Combine(baseDirectory, sections[Keys.Sections.Configuration][Keys.Values.BasedOn].Single());
            var baseSections = await ReadSectionsFromFile(baseIniFilePath);

            sections = CombineSections(baseSections, sections);
        }

        return new IniFile
        {
            Sections = sections
        };
    }

    private Dictionary<string,Dictionary<string,List<string>>> CombineSections(Dictionary<string,Dictionary<string,List<string>>> baseSections, Dictionary<string,Dictionary<string,List<string>>> sections)
    {
        foreach (var section in sections.Keys)
        {
            foreach (var key in sections[section].Keys)
            {
                if (section == Keys.Sections.Configuration && key == Keys.Values.BasedOn)
                {
                    continue;
                }

                var keyModificationMatch = _keyModificationRegex.Match(key);
                if (!keyModificationMatch.Success)
                {
                    continue;
                }

                void EnsureSection(string newSection)
                {
                    if (!baseSections.ContainsKey(newSection))
                    {
                        baseSections.Add(newSection, new Dictionary<string, List<string>>());
                    }
                }

                void EnsureKey(string newSection, string newKey)
                {
                    if (!baseSections[newSection].ContainsKey(newKey))
                    {
                        baseSections[newSection].Add(newKey, new List<string>());
                    }
                }

                var newKey = keyModificationMatch.Groups["KEY"].Value;
                switch (keyModificationMatch.Groups["OPERATION"].Value)
                {
                    case "+":
                    case ".":
                        EnsureSection(section);
                        EnsureKey(section, newKey);
                        
                        baseSections[section][newKey].AddRange(sections[section][key]);
                        break;
                    
                    case "-":
                        if (baseSections.ContainsKey(section) && baseSections[section].ContainsKey(newKey))
                        {
                            baseSections[section][newKey].RemoveAll(x => sections[section][key].Contains(x));
                        }
                        break;
                    
                    case "!":
                        if (baseSections.ContainsKey(section) && baseSections[section].ContainsKey(newKey))
                        {
                            baseSections[section][newKey].Clear();
                        }
                        break;
                    case ";":
                        break;
                    default:
                        EnsureSection(section);
                        EnsureKey(section, newKey);

                        baseSections[section][newKey] = sections[section][key];
                        break;
                }
            }
        }

        return baseSections;
    }

    private async Task<Dictionary<string, Dictionary<string, List<string>>>> ReadSectionsFromFile(string path)
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

        return sections;
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