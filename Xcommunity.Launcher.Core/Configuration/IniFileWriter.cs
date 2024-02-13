namespace Xcommunity.Launcher.Core.Configuration;

public class IniFileWriter
{
    private readonly string _path;

    public IniFileWriter(string path)
    {
        _path = path;
    }

    public async Task WriteAsync(IniFile iniFile, DateTime timestamp)
    {
        SetIniVersion(iniFile, timestamp);
        EnsureDirectoryExists();

        await using var stream = File.Open(_path, FileMode.Create);
        await using var writer = new StreamWriter(stream);

        foreach (var section in iniFile.Sections)
        {
            await writer.WriteLineAsync($"[{section.Key}]");
            foreach (var entry in section.Value.SelectMany(x => x.Value, (x, y) => $"{x.Key}={y}")) await writer.WriteLineAsync(entry);

            await writer.WriteLineAsync();
        }
    }

    private void EnsureDirectoryExists()
    {
        var fileInfo = new FileInfo(_path);
        if (fileInfo.Directory == null) throw new Exception("Could not determine target config folder");

        if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();
    }

    private static void SetIniVersion(IniFile iniFile, DateTime timestamp)
    {
        if (!iniFile.Sections.ContainsKey(Constants.Sections.IniVersion))
            iniFile.Sections.Add(Constants.Sections.IniVersion, new Dictionary<string, List<string>>());
        else if (iniFile.Sections[Constants.Sections.IniVersion].Any()) iniFile.Sections[Constants.Sections.IniVersion].Clear();

        iniFile.Sections[Constants.Sections.IniVersion].Add("0", new List<string>
        {
            $"{new DateTimeOffset(timestamp).ToUnixTimeSeconds()}.000000"
        });
    }
}