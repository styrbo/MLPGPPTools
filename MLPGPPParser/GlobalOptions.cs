namespace MLPGPPParser;

/// <summary>
/// Options that can be used with any command, e.g. <c>--g_path="O:\MLGPP\other"</c>.
/// They can appear anywhere in the command line and are removed before the task sees the arguments.
/// </summary>
public static class GlobalOptions {

    private const string GamesPathOption = "--g_path";

    /// <summary>
    /// Games folder used by commands that work on games. Defaults to <see cref="HashFileCreator.DefaultFolder"/>.
    /// </summary>
    public static string GamesPath { get; private set; } = HashFileCreator.DefaultFolder;

    /// <summary>
    /// Removes global options from <paramref name="args"/> and applies them.
    /// Returns the remaining arguments, or null if an option is invalid (the error is logged).
    /// </summary>
    public static string[]? Extract(string[] args) {
        var rest = new List<string>();

        for (var i = 0; i < args.Length; i++) {
            var arg = args[i];

            if (arg == GamesPathOption) {
                if (i + 1 >= args.Length) {
                    Logger.LogError($"{GamesPathOption} needs a value, e.g. {GamesPathOption}=\"O:\\MLGPP\\g\"");
                    return null;
                }
                if (TrySetGamesPath(args[++i]) == false) return null;
            } else if (arg.StartsWith(GamesPathOption + "=", StringComparison.Ordinal)) {
                if (TrySetGamesPath(arg[(GamesPathOption.Length + 1)..]) == false) return null;
            } else {
                rest.Add(arg);
            }
        }

        return rest.ToArray();
    }

    /// <summary>
    /// Returns the games folder to use: <paramref name="explicitPath"/> if given, otherwise <see cref="GamesPath"/>.
    /// Returns null (after logging an error) if that folder does not exist.
    /// </summary>
    public static string? ResolveGamesFolder(string? explicitPath = null) {
        var path = explicitPath ?? GamesPath;

        if (Directory.Exists(path) == false) {
            Logger.LogError($"Games folder not found: {path}");
            return null;
        }

        return path;
    }

    private static bool TrySetGamesPath(string rawValue) {
        // Windows argument parsing turns "O:\x\" into O:\x" — strip any stray quotes
        var value = rawValue.Trim().Trim('"').Trim();

        if (value.Length == 0) {
            Logger.LogError($"{GamesPathOption} is empty");
            return false;
        }

        GamesPath = Path.GetFullPath(value);
        return true;
    }
}
