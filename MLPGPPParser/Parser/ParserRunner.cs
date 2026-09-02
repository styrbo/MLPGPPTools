using System.Text.Json;
using Google.Apis.Sheets.v4;

namespace MLPGPPParser.Parser;

public class ParserRunner : ITaskRunner {

    public string TaskName => "parser";

    public async Task<int> Run(string[] args) {
        var configPath = $@"{Environment.CurrentDirectory}\Resources\config.json";

        Logger.LogInfo("Loading config...");
        if (File.Exists(configPath) == false) {
            CreateEmptyConfig();
            Logger.LogError("Config file not found,  created new one");
            return -1;
        }

        var configJson = await File.ReadAllTextAsync(configPath);
        var config = JsonSerializer.Deserialize<Config>(configJson);
        if (config == null) {
            CreateEmptyConfig();
            Logger.LogError("Config file is invalid, created new one");
            return -1;
        }

        void CreateEmptyConfig() {
            var newConfig = new Config(
                "idk",
                "idk",
                "source!A3:X",
                "O:/MLGPP/s",
                "O:/MLGPP/Pony.Games//data/games.json",
                @"O:\MLGPP\Pony.Games",
                @"O:\MLGPP\logs");

            var newConfigJson = JsonSerializer.Serialize(newConfig);
            Directory.CreateDirectory(configPath.Substring(0, configPath.LastIndexOf('\\'))); // (Resources\config.json)
            File.WriteAllText(configPath, newConfigJson);
        }


        var initializer = new SheetsService.Initializer();
        initializer.ApiKey = config.APIToken;

        var api = new SheetsService(initializer);

        var request = api.Spreadsheets.Values.Get(config.sheetID, config.dataRange);

        Logger.LogInfo("Loading screenshots data...");
        var screenshotCollection =
            ConsoleDrawer.RunTask(Task.Run(() => ScreenshotsCollection.LoadCollection(config.localScreenshotsDBPath)));

        Logger.LogInfo("Loading sheet data...");
        var sheetData = ConsoleDrawer.RunTask(request.ExecuteAsync());

        Logger.LogInfo("Parsing data...");
        var parser = new Parser(screenshotCollection);

        var task = Task.Run(() => parser.ParseGames(sheetData.Values));
        var games = ConsoleDrawer.RunTask(task);
        await task;

        Logger.LogInfo($"parsed {games.Length} games");

        Logger.LogInfo("Generating Json");

        var formatedGames = DBFormater.FormatGames(config, games);

        var jsonOptions = new JsonSerializerOptions() {
            IncludeFields = true,
            WriteIndented = true,
        };
        var json = JsonSerializer.Serialize(formatedGames, jsonOptions);

        Logger.LogInfo("Saving Json");

        await File.WriteAllTextAsync(config.gamesJsonPath, json);

/* // no more screenshots moving
Logger.LogInfo("Moving Screenshots");

ConsoleDrawer.RunTask(Task.Run(() => screenshotCollection.Apply(config.siteLocalPath, games)));

Logger.LogInfo("Done!");
*/

        return 0;
    }
}
