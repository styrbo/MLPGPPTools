namespace MLPGPPParser;

public class HashingRunner : ITaskRunner {

    public string TaskName => "hash";
    
    public int Run(string[] args) {
        if (args.Length < 1) {
            ConsoleDrawer.DrawError("no mode selected");
            return -1;
        }
        
        var mode = args[0];

        if (mode == "calc") {
            HashFileCreator.CalculateHashes();
        }
        else if (mode == "find") {
            Find(args.Length > 1 ? args[1] : throw new ArgumentException("no file to check")).Wait();
        }

        return -1;
    }

    private async Task Find(string fileToCheck) {
        ConsoleDrawer.DrawText($"Calculating hash for {fileToCheck}");
        var hash = await HashFile.Calculate(fileToCheck);
        ConsoleDrawer.DrawText($"{hash}");
        
        ConsoleDrawer.DrawText("Loading Hashes DB");

        var hashesTasks = Directory
            .GetFiles(@"O:\MLGPP\g_hash\")
            .Select(HashFile.LoadHashFile);
        
        HashFile?[] hashes = await Task.WhenAll(hashesTasks);

        ConsoleDrawer.DrawText("Finding collision...");

        foreach (var h in hashes) {
            if(hash.CompareHashes(h)) {
                ConsoleDrawer.DrawText($"Collision found with {h.Path}");
                return;
            }
        }
        
        ConsoleDrawer.DrawError("No hashes found");
    }
}
