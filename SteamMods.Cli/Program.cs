// See https://aka.ms/new-console-template for more information

using SteamMods.Core.Configuration;

Console.WriteLine("Hello, XCOM 2!");

var path = @"Config\DefaultEngine.ini";

var reader = new IniFileReader();
var result = await reader.Read(path);

Console.WriteLine(result.Sections["Engine.DownloadableContentEnumerator"]["ModRootDirs"]);