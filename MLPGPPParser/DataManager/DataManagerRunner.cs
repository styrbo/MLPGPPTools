namespace MLPGPPParser.DataManager;

public class DataManagerRunner : ITaskRunner {

    public string TaskName => "data";

    // run data pack [path|base]
    // run data unpack [path|base]
    // run data remove <list-file> [-y]
    // run data shift [path|base]
    public Task<int> Run(string[] args) {
        if (args.Length < 1) {
            Logger.LogError("no mode selected (pack | unpack | remove | shift)");
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
            case "remove": {
                if (args.Length < 2) {
                    Logger.LogError("usage: data remove <list-file> [-y]");
                    return Task.FromResult(-1);
                }

                var extra = args.Skip(2).Where(arg => arg != "-y").ToArray();
                if (extra.Length > 0) {
                    Logger.LogError($"unknown argument '{extra[0]}' (usage: data remove <list-file> [-y])");
                    return Task.FromResult(-1);
                }

                var gamesPath = GlobalOptions.ResolveGamesFolder();
                if (gamesPath == null)
                    return Task.FromResult(-1);

                var skipConfirmation = args.Skip(2).Contains("-y");
                return Task.FromResult(DataRemover.Remove(args[1], gamesPath, skipConfirmation));
            }
            case "shift": {
                var gamesPath = GlobalOptions.ResolveGamesFolder(args.Length > 1 && args[1] != "base" ? args[1] : null);
                if (gamesPath == null)
                    return Task.FromResult(-1);

                return Task.FromResult(DataShifter.Shift(gamesPath));
            }
            default:
                Logger.LogError($"unknown data mode '{mode}' (expected pack | unpack | remove | shift)");
                return Task.FromResult(-1);
        }
    }
}
