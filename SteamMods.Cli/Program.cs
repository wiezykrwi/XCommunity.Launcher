// See https://aka.ms/new-console-template for more information

using SteamMods.Cli;

Console.WriteLine("Hello, World!");

var path = @"C:\Users\Mathia\source\repos\SteamMods\DefaultEngine.ini";

var reader = new IniFileReader();
var result = await reader.Read(path);

Console.WriteLine(result.Sections["Engine.DownloadableContentEnumerator"]["ModRootDirs"]);