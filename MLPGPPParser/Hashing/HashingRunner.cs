namespace MLPGPPParser;

public class HashingRunner : ITaskRunner {

    public string TaskName => "hash";
    
    public async Task<int> Run(string[] args) {
        if (args.Length < 1) {
            Logger.LogError("no mode selected");
            return -1;
        }
        
        var mode = args[0];

        if (mode == "calc") {
            var path = GlobalOptions.ResolveGamesFolder(args.Length > 1 && args[1] != "base" ? args[1] : null);
            if (path == null)
                return -1;

            var isForceRecalc = args.Length > 2 && args[2] == "-f";

            var task = HashFileCreator.CalculateHashes(path, isForceRecalc);
            ConsoleDrawer.RunTask(task);
            await task;

        }
        else if (mode == "find") {
            var gamesPath = GlobalOptions.ResolveGamesFolder();
            if (gamesPath == null)
                return -1;

            await Find(args.Length > 1 ? args[1] : throw new ArgumentException("no file to check"), gamesPath);
        }
        else if (mode == "find_all") {

        }
        else if (mode == "coll") {
            var gamesPath = GlobalOptions.ResolveGamesFolder();
            if (gamesPath == null)
                return -1;

            var task = Task.Run(() => FindCollisions(gamesPath));
            ConsoleDrawer.RunTask(task, isLogProgress:false);
            await task;
        }

        return -1;
    }

    private async Task<HashFile[]> LoadHashes(string gamesPath) {
        var hashesTasks = Directory
            .GetDirectories(gamesPath)
            .SelectMany(dir => Directory.GetFiles(dir, "*.hash"))
            .Select(HashFile.LoadHashFile);
        
        HashFile[] hashes = await Task.WhenAll(hashesTasks);
        
        return hashes;
    }

    private async Task Find(string pathToCheck, string gamesPath) {

        Logger.LogInfo("Loading Hashes DB");

        var hashes = await LoadHashes(gamesPath);

        Logger.LogInfo("Finding collision...");

        string[] filesToCheck;
        if (File.Exists(pathToCheck)) {
            filesToCheck = new[] { pathToCheck };
        } else if (Directory.Exists(pathToCheck)) {
            filesToCheck = Directory.GetFiles(pathToCheck, "*", SearchOption.AllDirectories);
        } else {
            Logger.LogError("Invalid file or directory path");
            return;
        }
        
        //sort files by name
        filesToCheck = filesToCheck.OrderBy(f => f).ToArray();

        var results = new List<(bool, string)>();
        foreach (var fileToCheckPath in filesToCheck) {
            Logger.LogInfo($"Calculating hash for {fileToCheckPath}");
            var hash = await HashFile.Calculate(fileToCheckPath);
            Logger.LogInfo($"{hash}");

            var isCollisionFounded = false;
            foreach (var h in hashes) {
                if (hash.CompareHashes(h)) {
                    results.Add((true, $"✅ {fileToCheckPath} ({hash.Hashes[0].Value}) == {h.Path} ({h.Hashes[0].Value})"));
                    isCollisionFounded = true;
                    break;
                }
            }

            if (!isCollisionFounded) {
                results.Add((false, $"❌ {fileToCheckPath} ({hash.Hashes[0].Value})"));
            }
        }

        Logger.LogInfo("=== Results: ===");
        foreach (var (isCollision, message) in results) {
            if (isCollision) {
                Logger.LogInfo(message);
            } else {
                Logger.LogError(message);
            }
        }
    }

    private async Task FindCollisions(string gamesPath) {
        Logger.LogInfo("Loading Hashes DB");
        var hashes = await LoadHashes(gamesPath);
        Logger.LogInfo("Finding collisions...");

        var foundedCollisions = new List<(HashFile, HashFile)>();

        foreach (var currentHash in hashes) {
            if(currentHash == null)
                continue;

            foreach (var otherHash in hashes) {
                
                if(foundedCollisions.Contains((currentHash, otherHash)) ||
                   foundedCollisions.Contains((otherHash, currentHash))) {
                    continue;
                }
                
                if (currentHash.Path == otherHash.Path) {
                    continue;
                }
                
                if(currentHash.CompareHashes(otherHash)) {
                    Logger.LogError($"Collision found between {currentHash.Path} and {otherHash.Path}");
                    foundedCollisions.Add((currentHash, otherHash));
                }
            }
        }
        
        if (foundedCollisions.Count == 0) {
            Logger.LogError($"No collisions found!");
        } else {
            Logger.LogInfo($"Collisions found: {foundedCollisions.Count}");
        }
    }
}
