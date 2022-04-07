// See https://aka.ms/new-console-template for more information

using SteamMods.Core.Configuration;

Console.WriteLine("Hello, World!");

var path = @"D:\SteamLibrary\steamapps\common\XCOM 2\XCom2-WarOfTheChosen\XComGame\Config\DefaultEngine.ini";

var reader = new IniFileReader();
var result = await reader.Read(path);

Console.WriteLine(result.Sections["Engine.DownloadableContentEnumerator"]["ModRootDirs"]);