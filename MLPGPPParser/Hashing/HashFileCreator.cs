using System.Security.Cryptography;

namespace MLPGPPParser;

public static class HashFileCreator {


    public static void CalculateHashes() {
        
        Parallel.For(0, 1042, (i) => {
            CreateHashFile((uint)i).Wait();
        });
    }
    

    public const string BuildsFolder = @"O:\MLGPP\g\";
    public const string Folder = @"O:\MLGPP\g_hash\";

    public static async Task CreateHashFile(uint buildID) {
        if(File.Exists($"{Folder}{buildID}.in.hash")) {
            ConsoleDrawer.DrawWarning($"Hash file for build {buildID} already exists, skipping");
            return;
        }
        
        var fs = new FileStream($"{Folder}{buildID}.in.hash", FileMode.Create);
        var sw = new StreamWriter(fs);
        await sw.WriteLineAsync("Memory Leak Slopity Check! 67 /)");
        await sw.WriteAsync("MD5 ");
        
        var gameFilePath = Directory.GetFiles($"{BuildsFolder}{buildID}").FirstOrDefault();
        var gameFileStream = new FileStream(gameFilePath, FileMode.Open);
        var hash = await MD5.HashDataAsync(gameFileStream);
        await sw.WriteLineAsync(Convert.ToHexString(hash));
        sw.Close();
        fs.Close();
        
        ConsoleDrawer.DrawText($"Hashed File: {buildID} name {Path.GetFileName(gameFilePath)}");
    }
}
