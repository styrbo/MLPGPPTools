namespace MLPGPPParser.FoldersManager;

public class FoldersManagerRunner : ITaskRunner {

    public string TaskName => "folders";

    public int Run(string[] args) {
        Console.WriteLine("Hello, World!");

        var path = @"O:\MLGPP\g";
        var emptyDirsCount = 10000;
        for (int i = 0; i < emptyDirsCount; i++) {
            var dir = $"{path}\\{i}";
            if (Directory.Exists(dir)) continue;
            Directory.CreateDirectory(dir);
        }
        return 0;
    }
}
