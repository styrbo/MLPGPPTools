namespace MLPGPPParser.DataManager;

/// <summary>
/// Deletes everything inside the selected game folders, keeping the (now empty) folders themselves.
/// Selected games come from a text file with one game id per line.
/// </summary>
public static class DataRemover {

    /// <returns>0 on success or when cancelled, -1 on error.</returns>
    public static int Remove(string listPath, string gamesPath, bool skipConfirmation) {
        if (File.Exists(listPath) == false) {
            Logger.LogError($"List file not found: {listPath}");
            return -1;
        }

        // Validate the whole list before touching anything
        var ids = ReadIds(listPath);
        if (ids == null) {
            return -1;
        }

        var folders = GetGameFoldersById(gamesPath);
        if (folders == null) {
            return -1;
        }

        var targets = new List<(uint Id, string Folder)>();
        foreach (var id in ids) {
            if (folders.TryGetValue(id, out var folder)) {
                if (FileUtilities.IsReparsePoint(folder)) {
                    // the game folder is a link: deleting "inside" it would delete the link's target
                    Logger.LogWarning($"{id}: game folder is a junction or symbolic link, skipping");
                    continue;
                }
                targets.Add((id, folder));
            } else {
                Logger.LogWarning($"{id}: game folder not found, skipping");
            }
        }

        if (targets.Count == 0) {
            Logger.LogInfo("Nothing to remove");
            return 0;
        }

        Logger.LogInfo($"Selected {targets.Count} game folders: {string.Join(", ", targets.Select(t => t.Id))}");
        Logger.LogWarning("This deletes ALL files and subfolders (including .hash files) inside them. The folders themselves are kept.");

        if (skipConfirmation == false) {
            if (Console.IsInputRedirected) {
                Logger.LogError("confirmation needed; run again with -y to skip it");
                return -1;
            }

            if (ConsoleDrawer.AskPermissionToContinue("Delete the data in these game folders?") == false) {
                Logger.LogInfo("Cancelled, nothing was deleted");
                return 0;
            }
        }

        var failed = 0;
        foreach (var (id, folder) in targets) {
            try {
                var (files, folderCount) = DeleteContents(folder);
                Logger.LogInfo($"{id}: removed {files} files and {folderCount} subfolders, folder kept");
            } catch (Exception ex) {
                failed++;
                Logger.LogError($"{id}: failed to remove data: {ex.Message}");
            }
        }

        Logger.LogInfo($"Remove finished: {targets.Count - failed} done, {failed} failed");
        return failed == 0 ? 0 : -1;
    }

    private static SortedSet<uint>? ReadIds(string listPath) {
        var ids = new SortedSet<uint>();
        var lines = File.ReadAllLines(listPath);

        for (var i = 0; i < lines.Length; i++) {
            var line = lines[i].Trim();
            if (line.Length == 0) {
                continue;
            }

            if (uint.TryParse(line, out var id) == false) {
                Logger.LogError($"{Path.GetFileName(listPath)} line {i + 1}: '{line}' is not a game id. Nothing was deleted.");
                return null;
            }

            ids.Add(id);
        }

        if (ids.Count == 0) {
            Logger.LogError("The list file contains no game ids. Nothing was deleted.");
            return null;
        }

        return ids;
    }

    private static Dictionary<uint, string>? GetGameFoldersById(string gamesPath) {
        var folders = new Dictionary<uint, string>();

        foreach (var dir in Directory.GetDirectories(gamesPath)) {
            if (uint.TryParse(Path.GetFileName(dir), out var id) == false) {
                continue;
            }

            if (folders.ContainsKey(id)) {
                Logger.LogError($"Two folders map to game id {id} ({Path.GetFileName(folders[id])}, {Path.GetFileName(dir)}). Nothing was deleted.");
                return null;
            }

            folders[id] = dir;
        }

        return folders;
    }

    /// <summary>
    /// Deletes all files and subfolders inside <paramref name="folder"/>, but not the folder itself.
    /// </summary>
    private static (int Files, int Folders) DeleteContents(string folder) {
        var files = 0;
        var folders = 0;

        foreach (var sub in Directory.GetDirectories(folder)) {
            if (FileUtilities.IsReparsePoint(sub)) {
                // junction or symlink: remove the link only, never follow it into other data
                Directory.Delete(sub, recursive: false);
            } else {
                var (subFiles, subFolders) = DeleteContents(sub);
                files += subFiles;
                folders += subFolders;
                Directory.Delete(sub, recursive: false);
            }
            folders++;
        }

        foreach (var file in Directory.GetFiles(folder)) {
            File.SetAttributes(file, FileAttributes.Normal); // clear read-only so it can be deleted
            File.Delete(file);
            files++;
        }

        return (files, folders);
    }
}
