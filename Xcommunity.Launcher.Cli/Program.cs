using Xcommunity.Launcher.Core;

var modFinder = new ModFinder();
var mods = await modFinder.FindModsAsync();

var modSaver = new ModSaver();
await modSaver.SaveAsync(mods);