// See https://aka.ms/new-console-template for more information

using SteamMods.Core.Configuration;

Console.WriteLine("Hello, XCOM 2!");

var path = @"Config\DefaultEngine.ini";

var reader = new IniFileReader();
var result = await reader.Read(path);

var gameDirectoryLocator = new GameDirectoryLocator();
var baseDirectory = gameDirectoryLocator.GetBaseGameDirectory(268500);

// add default mod dir
var defaultModDir = Path.Combine(baseDirectory, "../../workshop/content/268500");

var modDir = Path.Combine(baseDirectory, result.Sections["Engine.DownloadableContentEnumerator"]["ModRootDirs"].First());
Console.WriteLine(modDir);