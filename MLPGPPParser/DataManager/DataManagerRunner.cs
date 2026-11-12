namespace MLPGPPParser.DataManager;

public class DataManagerRunner : ITaskRunner {

    public string TaskName => "data";

    // run data pack [path|base]
    // run data unpack [path|base]
    public Task<int> Run(string[] args) {
        if (args.Length < 1) {
            Logger.LogError("no mode selected (pack | unpack)");
            return Task.FromResult(-1);
        }

        var mode = args[0];

        switch (mode) {
            case "pack": {
                var gamesPath = GlobalOptions.ResolveGamesFolder(args.Length > 1 && args[1] != "base" ? args[1] : null);
                if (gamesPath == null)
                    return Task.FromResult(-1);

                DataPacker.PackAll(gamesPath);
                return Task.FromResult(0);
            }
            case "unpack": {
                var gamesPath = GlobalOptions.ResolveGamesFolder(args.Length > 1 && args[1] != "base" ? args[1] : null);
                if (gamesPath == null)
                    return Task.FromResult(-1);

                DataPacker.UnpackAll(gamesPath);
                return Task.FromResult(0);
            }
            default:
                Logger.LogError($"unknown data mode '{mode}' (expected pack | unpack)");
                return Task.FromResult(-1);
        }
    }
}
