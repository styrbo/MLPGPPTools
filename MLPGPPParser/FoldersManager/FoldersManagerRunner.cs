namespace MLPGPPParser.FoldersManager;

public class FoldersManagerRunner : ITaskRunner {

    public string TaskName => "folders";

    public Task<int> Run(string[] args) {
        Logger.LogInfo("Creating folder structure...");

        var path = @"O:\MLGPP\Pony.Games\s";
        var emptyDirsCount = 10000;
        for (int i = 0; i < emptyDirsCount; i++) {
            var dir = $"{path}\\{i}";
            if (Directory.Exists(dir)) continue;
            Directory.CreateDirectory(dir);
        }
        Logger.LogInfo("Folder structure created successfully");
        return Task.FromResult(0);
    }
}
