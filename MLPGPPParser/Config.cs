namespace MLPGPPParser;

[Serializable]
public record Config(
    string APIToken,
    string sheetID,
    string dataRange,
    string localScreenshotsDBPath,
    string gamesJsonPath,
    string siteLocalPath,
    string logFolderPath);
