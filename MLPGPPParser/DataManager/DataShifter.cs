namespace MLPGPPParser.DataManager;

/// <summary>
/// Moves data from higher game ids into empty lower game folders, so the data sits right after the lowest ids.
///
/// Example:  11=game.zip, 12=(empty), 13=another_game.zip   ->   11=game.zip, 12=another_game.zip, 13=(empty)
///
/// Rules:
///  - Each empty folder (a "hole") is filled by the next non-empty folder above it. The order of games is kept.
///  - Only moves are done. Nothing is deleted.
///  - A folder's hash file (.{id}.in.hash) is renamed to match the new id.
///  - Junctions and symbolic links are never moved or used as a target.
///  - If a move fails, the moves already made in that step are reversed and the command stops.
///  - Game ids change with this command. Anything that stores ids (for example the games JSON) must be updated.
/// </summary>
public static class DataShifter {

    /// <returns>0 on success, -1 on error.</returns>
    public static int Shift(string gamesPath) {
        var entries = GetGameEntries(gamesPath);
        if (entries == null) {
            return -1;
        }

        if (entries.Count == 0) {
            Logger.LogInfo("No games found");
            return 0;
        }

        Logger.LogInfo($"Shifting data in {entries.Count} game folders");

        var movedFolders = 0;
        while (true) {
            var holeIndex = entries.FindIndex(e => IsEmpty(e.Folder));
            if (holeIndex < 0) {
                break;
            }

            var sourceIndex = -1;
            for (var i = holeIndex + 1; i < entries.Count; i++) {
                if (IsEmpty(entries[i].Folder) == false) {
                    sourceIndex = i;
                    break;
                }
            }

            // no data above the lowest hole: everything below is already packed together
            if (sourceIndex < 0) {
                break;
            }

            var source = entries[sourceIndex];
            var target = entries[holeIndex];

            try {
                MoveGame(source, target);
            } catch (Exception ex) {
                Logger.LogError($"Shift stopped: moving {source.Id} into {target.Id} failed: {ex.Message}. Completed moves in this step were reversed. Nothing was deleted.");
                return -1;
            }

            movedFolders++;
        }

        Logger.LogInfo($"Shift finished: {movedFolders} folders filled");
        return 0;
    }

    private record GameEntry(uint Id, string Folder);

    private static List<GameEntry>? GetGameEntries(string gamesPath) {
        if (Directory.Exists(gamesPath) == false) {
            Logger.LogError($"Games folder not found: {gamesPath}");
            return null;
        }

        var entries = new List<GameEntry>();
        var seen = new HashSet<uint>();

        foreach (var dir in Directory.GetDirectories(gamesPath)) {
            if (uint.TryParse(Path.GetFileName(dir), out var id) == false) {
                continue;
            }

            if (FileUtilities.IsReparsePoint(dir)) {
                Logger.LogWarning($"{id}: junction or symbolic link, not used by shift");
                continue;
            }

            if (seen.Add(id) == false) {
                Logger.LogError($"Two folders map to game id {id}. Nothing was moved.");
                return null;
            }

            entries.Add(new GameEntry(id, dir));
        }

        entries.Sort((a, b) => a.Id.CompareTo(b.Id));
        return entries;
    }

    private static bool IsEmpty(string folder) =>
        Directory.GetFileSystemEntries(folder).Length == 0;

    /// <summary>
    /// Moves every entry of the source folder into the (empty) target folder.
    /// The source folder itself stays, empty.
    /// </summary>
    private static void MoveGame(GameEntry source, GameEntry target) {
        var sourceHashName = HashFileName(source.Id);
        var targetHashName = HashFileName(target.Id);

        var moves = new List<(string From, string To)>();
        try {
            foreach (var file in Directory.GetFiles(source.Folder)) {
                var name = Path.GetFileName(file);
                if (name == sourceHashName) {
                    name = targetHashName;
                }

                var to = Path.Combine(target.Folder, name);
                File.Move(file, to);
                moves.Add((file, to));
            }

            foreach (var dir in Directory.GetDirectories(source.Folder)) {
                var to = Path.Combine(target.Folder, Path.GetFileName(dir));
                Directory.Move(dir, to);
                moves.Add((dir, to));
            }
        } catch (Exception) {
            foreach (var (from, to) in Enumerable.Reverse(moves)) {
                if (File.Exists(to)) {
                    File.Move(to, from);
                } else if (Directory.Exists(to)) {
                    Directory.Move(to, from);
                }
            }
            throw;
        }

        Logger.LogInfo($"{source.Id} -> {target.Id}: moved {moves.Count} entries");
    }

    private static string HashFileName(uint id) => $".{id}.in.hash";
}
