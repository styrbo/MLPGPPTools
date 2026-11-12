namespace MLPGPPParser;

public static class FileUtilities {
    public static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        var dir = new DirectoryInfo(sourceDir);
        Directory.CreateDirectory(destinationDir);

        // Copy files
        foreach (FileInfo file in dir.GetFiles())
            file.CopyTo(Path.Combine(destinationDir, file.Name), true);

        // Copy subdirectories recursively
        if (recursive)
            foreach (DirectoryInfo subDir in dir.GetDirectories())
                CopyDirectory(subDir.FullName, Path.Combine(destinationDir, subDir.Name), true);
    }

    /// <summary>
    /// True for junctions and symbolic links. Operations that delete or read through a folder must not follow them.
    /// </summary>
    public static bool IsReparsePoint(string path) =>
        (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0;

    /// <summary>
    /// True if the folder itself, or any folder inside it, is a junction or symbolic link.
    /// </summary>
    public static bool ContainsLinks(string folder) =>
        IsReparsePoint(folder)
        || Directory.GetDirectories(folder, "*", SearchOption.AllDirectories).Any(IsReparsePoint);
}
