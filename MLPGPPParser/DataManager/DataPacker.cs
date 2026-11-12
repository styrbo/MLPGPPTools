using System.IO.Compression;

namespace MLPGPPParser.DataManager;

/// <summary>
/// Packs the content of numbered game folders into a single zip, and unpacks it back.
///
/// Pack rules:
///  - Pointless nested folders are skipped: starting from the game folder, we descend while a folder
///    has no files (except .hash) and exactly one subfolder. The first folder that doesn't match is the content root.
///  - A game is packed only if its content root holds more than one file (.hash files don't count).
///  - The zip is named after the content root folder and placed directly in the game folder.
///  - The zip contains the content root's files and empty subfolders, with paths relative to the root.
///  - The zip is verified before the original files and pointless folders are deleted.
///  - .hash files are never packed and never moved; they stay in the game folder.
///
/// Unpack rules:
///  - The zip in the game folder is extracted directly into the game folder (pointless folders are not recreated).
///  - Extraction is refused if it would overwrite anything. On failure, files created by this run are removed.
///  - The zip is deleted only after everything was extracted.
/// </summary>
public static class DataPacker {

    private const string HashExtension = ".hash";
    private const string ZipExtension = ".zip";

    public static void PackAll(string gamesPath) {
        var gameFolders = GetGameFolders(gamesPath);
        if (gameFolders.Length == 0) {
            Logger.LogInfo("No games found");
            return;
        }

        Logger.LogInfo($"Packing {gameFolders.Length} game folders");
        foreach (var gameFolder in gameFolders) {
            try {
                PackGame(gameFolder);
            } catch (Exception ex) {
                Logger.LogError($"Failed to pack {Path.GetFileName(gameFolder)}: {ex.Message}");
            }
        }
        Logger.LogInfo("Pack finished");
    }

    public static void UnpackAll(string gamesPath) {
        var gameFolders = GetGameFolders(gamesPath);
        if (gameFolders.Length == 0) {
            Logger.LogInfo("No games found");
            return;
        }

        Logger.LogInfo($"Unpacking {gameFolders.Length} game folders");
        foreach (var gameFolder in gameFolders) {
            try {
                UnpackGame(gameFolder);
            } catch (Exception ex) {
                Logger.LogError($"Failed to unpack {Path.GetFileName(gameFolder)}: {ex.Message}");
            }
        }
        Logger.LogInfo("Unpack finished");
    }

    private static string[] GetGameFolders(string gamesPath) {
        if (Directory.Exists(gamesPath) == false) {
            Logger.LogError($"Games folder not found: {gamesPath}");
            return [];
        }

        return Directory.GetDirectories(gamesPath)
            .Where(dir => uint.TryParse(Path.GetFileName(dir), out _))
            .OrderBy(dir => uint.Parse(Path.GetFileName(dir)))
            .ToArray();
    }

    private static void PackGame(string gameFolder) {
        var id = Path.GetFileName(gameFolder);
        var contentRoot = FindContentRoot(gameFolder);

        if (Directory.GetFiles(contentRoot, $"*{ZipExtension}").Length > 0) {
            Logger.LogWarning($"{id}: a zip already exists, skipping");
            return;
        }

        var allFiles = Directory.GetFiles(contentRoot, "*", SearchOption.AllDirectories);
        var hashFilesInsideRoot = allFiles.Where(IsHashFile).ToArray();
        if (contentRoot != gameFolder && hashFilesInsideRoot.Length > 0) {
            Logger.LogWarning($"{id}: hash file found inside a subfolder, skipping");
            return;
        }

        var filesToPack = allFiles.Where(file => IsHashFile(file) == false).ToArray();
        if (filesToPack.Length <= 1) {
            Logger.LogInfo($"{id}: {filesToPack.Length} file(s), nothing to pack");
            return;
        }

        var emptyFolders = Directory.GetDirectories(contentRoot, "*", SearchOption.AllDirectories)
            .Where(dir => Directory.GetFileSystemEntries(dir).Length == 0)
            .ToArray();

        var zipPath = Path.Combine(gameFolder, Path.GetFileName(contentRoot) + ZipExtension);
        if (File.Exists(zipPath)) {
            Logger.LogWarning($"{id}: {Path.GetFileName(zipPath)} already exists, skipping");
            return;
        }

        Logger.LogInfo($"{id}: packing {filesToPack.Length} files from {Path.GetRelativePath(gameFolder, contentRoot)}");
        CreateZip(zipPath, contentRoot, filesToPack, emptyFolders);
        VerifyZip(zipPath, contentRoot, filesToPack, emptyFolders);

        DeleteContent(contentRoot, filesToPack);
        DeletePointlessFolders(gameFolder, contentRoot);

        Logger.LogInfo($"{id}: packed into {Path.GetFileName(zipPath)}");
    }

    private static void UnpackGame(string gameFolder) {
        var id = Path.GetFileName(gameFolder);
        var zips = Directory.GetFiles(gameFolder, $"*{ZipExtension}");

        if (zips.Length == 0) {
            return;
        }

        if (zips.Length > 1) {
            Logger.LogWarning($"{id}: more than one zip found, skipping");
            return;
        }

        var zipPath = zips[0];
        var createdFiles = new List<string>();
        var createdFolders = new List<string>();

        try {
            using var archive = ZipFile.OpenRead(zipPath);
            var fullGameFolder = Path.GetFullPath(gameFolder) + Path.DirectorySeparatorChar;

            var targets = archive.Entries
                .Select(entry => (Entry: entry, Target: Path.GetFullPath(Path.Combine(gameFolder, entry.FullName))))
                .ToArray();

            foreach (var (entry, target) in targets) {
                if (target.StartsWith(fullGameFolder, StringComparison.OrdinalIgnoreCase) == false) {
                    Logger.LogWarning($"{id}: zip entry '{entry.FullName}' points outside the game folder, skipping");
                    return;
                }

                var isDirectoryEntry = entry.FullName.EndsWith('/');
                if (isDirectoryEntry == false && (File.Exists(target) || Directory.Exists(target))) {
                    Logger.LogWarning($"{id}: '{entry.FullName}' already exists, skipping (nothing was changed)");
                    return;
                }
                if (isDirectoryEntry && File.Exists(target)) {
                    Logger.LogWarning($"{id}: '{entry.FullName}' conflicts with an existing file, skipping (nothing was changed)");
                    return;
                }
            }

            Logger.LogInfo($"{id}: unpacking {Path.GetFileName(zipPath)}");

            foreach (var (entry, target) in targets) {
                if (entry.FullName.EndsWith('/')) {
                    if (Directory.Exists(target) == false) {
                        Directory.CreateDirectory(target);
                        createdFolders.Add(target);
                    }
                    continue;
                }

                var targetFolder = Path.GetDirectoryName(target)!;
                if (Directory.Exists(targetFolder) == false) {
                    Directory.CreateDirectory(targetFolder);
                    createdFolders.Add(targetFolder);
                }

                entry.ExtractToFile(target, overwrite: false);
                createdFiles.Add(target);

                if (new FileInfo(target).Length != entry.Length) {
                    throw new InvalidDataException($"size mismatch for '{entry.FullName}'");
                }
            }
        } catch (Exception) {
            // roll back what this run created, so the zip stays the only copy
            foreach (var file in createdFiles.Where(File.Exists)) {
                File.Delete(file);
            }
            foreach (var folder in createdFolders.OrderByDescending(f => f.Length)) {
                if (Directory.Exists(folder) && Directory.GetFileSystemEntries(folder).Length == 0) {
                    Directory.Delete(folder);
                }
            }
            throw;
        }

        File.Delete(zipPath);
        Logger.LogInfo($"{id}: unpacked, zip deleted");
    }

    /// <summary>
    /// Descends through folders that have no files (except .hash) and exactly one subfolder.
    /// </summary>
    private static string FindContentRoot(string gameFolder) {
        var current = gameFolder;
        while (true) {
            var hasFiles = Directory.GetFiles(current).Any(file => IsHashFile(file) == false);
            var subFolders = Directory.GetDirectories(current);

            if (hasFiles || subFolders.Length != 1) {
                return current;
            }

            current = subFolders[0];
        }
    }

    private static void CreateZip(string zipPath, string contentRoot, string[] files, string[] emptyFolders) {
        using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create);

        foreach (var folder in emptyFolders) {
            archive.CreateEntry(EntryName(contentRoot, folder) + "/");
        }
        foreach (var file in files) {
            archive.CreateEntryFromFile(file, EntryName(contentRoot, file), CompressionLevel.Optimal);
        }
    }

    private static void VerifyZip(string zipPath, string contentRoot, string[] files, string[] emptyFolders) {
        using var archive = ZipFile.OpenRead(zipPath);

        var expectedFiles = files.ToDictionary(file => EntryName(contentRoot, file), file => new FileInfo(file).Length);
        var expectedFolders = emptyFolders.Select(folder => EntryName(contentRoot, folder) + "/").ToHashSet();

        var actualFiles = archive.Entries.Where(e => e.FullName.EndsWith('/') == false).ToArray();
        var actualFolders = archive.Entries.Where(e => e.FullName.EndsWith('/')).Select(e => e.FullName).ToHashSet();

        var ok = actualFiles.Length == expectedFiles.Count
                 && actualFiles.All(e => expectedFiles.TryGetValue(e.FullName, out var size) && size == e.Length)
                 && expectedFolders.SetEquals(actualFolders);

        if (ok == false) {
            archive.Dispose();
            File.Delete(zipPath);
            throw new InvalidDataException($"zip verification failed for {Path.GetFileName(zipPath)}, originals were kept");
        }
    }

    private static void DeleteContent(string contentRoot, string[] files) {
        foreach (var file in files) {
            File.Delete(file);
        }

        // deepest folders first
        foreach (var folder in Directory.GetDirectories(contentRoot, "*", SearchOption.AllDirectories)
                     .OrderByDescending(f => f.Length)) {
            Directory.Delete(folder);
        }
    }

    /// <summary>
    /// Deletes the (now empty) folders between the game folder and the content root, and the content root itself.
    /// The game folder itself is never deleted.
    /// </summary>
    private static void DeletePointlessFolders(string gameFolder, string contentRoot) {
        var current = contentRoot;
        while (current != gameFolder) {
            Directory.Delete(current); // non-recursive: fails loudly if something is still inside
            current = Path.GetDirectoryName(current)!;
        }
    }

    private static string EntryName(string root, string path) =>
        Path.GetRelativePath(root, path).Replace('\\', '/');

    private static bool IsHashFile(string path) =>
        string.Equals(Path.GetExtension(path), HashExtension, StringComparison.OrdinalIgnoreCase);
}
