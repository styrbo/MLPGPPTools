using System.Security.Cryptography;

namespace MLPGPPParser;

public static class HashFileCreator {


    public static async Task CalculateHashes(string? gamesStoragePath, bool isForceRecalc) {

        if (gamesStoragePath == null)
            gamesStoragePath = DefaultFolder;
        
        if(gamesStoragePath.EndsWith("\\") is false) {
            gamesStoragePath += "\\";
        }
        
        var games = Directory.GetDirectories(gamesStoragePath)
            .Select(str => str.Split("\\").Last())
            .Where(x => x != null && uint.TryParse(x, out _))
            .Select(uint.Parse)
            .Order()
            .ToArray();
        
        if(games.Length == 0) {
            ConsoleDrawer.DrawText("No games found");
            return;
        }
        
        ConsoleDrawer.DrawText($"Start hashing {games.Length} games");
        _currentOperationPath = gamesStoragePath;
        _isRecalc = isForceRecalc;
        
        await Parallel.ForAsync(games[0], games[^1],Task);

        return;
        
        ValueTask Task(uint i, CancellationToken cancellationToken) {
            return CreateHashFile(_currentOperationPath, i, _isRecalc);
        }
    }

    private static string _currentOperationPath;
    private static bool _isRecalc;
    
    public const string DefaultFolder = @"O:\MLGPP\g\";
    private const string HashFileFormat = ".{0}.in.hash";

    public static async ValueTask CreateHashFile(string folderPath, uint buildID, bool recalc) {
        var hashFilePath = @$"{folderPath}{buildID}\{string.Format(HashFileFormat, buildID)}";
        
        if(File.Exists(hashFilePath)) {
            if(recalc) {
                Logger.LogInfo($"Recalculating hash for {buildID}");
                File.Delete(hashFilePath);
            } else {
                Logger.LogWarning($"Hash file for {buildID} already exists, skipping");
                return;
            }
        }
        
        var files = Directory.GetFiles($"{folderPath}{buildID}");
        if(files.Length == 0) {
            Logger.LogWarning($"No files found for {buildID}, skipping");
            return;
        }
        
        var fs = new FileStream(hashFilePath, FileMode.Create);
        var sw = new StreamWriter(fs);
        await sw.WriteAsync("Memory Leak Slopity Check! 67 /)\n");
        await sw.WriteAsync("MD5 ");
        
        var gameFilePath = files.First();
        var gameFileStream = new FileStream(gameFilePath, FileMode.Open);
        var hash = await MD5.HashDataAsync(gameFileStream);
        await sw.WriteLineAsync(Convert.ToHexString(hash));
        sw.Close();
        fs.Close();
        
        ConsoleDrawer.DrawText($"Hashed File: {buildID} name {Path.GetFileName(gameFilePath)}");
    }
}
