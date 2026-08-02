using System.Text.Json;
using Google.Apis.Sheets.v4;

namespace MLPGPPParser.Parser;

public class ParserRunner : ITaskRunner {

    public string TaskName => "parser";

    public int Run(string[] args) {
        var configPath = $@"{Environment.CurrentDirectory}\Resources\config.json";

        ConsoleDrawer.DrawText("Loading config...");
        if (File.Exists(configPath) == false) {
            CreateEmptyConfig();
            return ConsoleDrawer.DrawError("Config file not found,  created new one");
        }

        var configJson = File.ReadAllText(configPath);
        var config = JsonSerializer.Deserialize<Config>(configJson);
        if (config == null) {
            CreateEmptyConfig();
            return ConsoleDrawer.DrawError("Config file is invalid, created new one");
        }

        void CreateEmptyConfig() {
            var newConfig = new Config(
                "idk",
                "idk",
                "source!A3:X",
                "O:/MLGPP/s",
                "O:/MLGPP/Pony.Games//data/games.json",
                @"O:\MLGPP\Pony.Games");

            var newConfigJson = JsonSerializer.Serialize(newConfig);
            Directory.CreateDirectory(configPath.Substring(0, configPath.LastIndexOf('\\'))); // (Resources\config.json)
            File.WriteAllText(configPath, newConfigJson);
        }


        var initializer = new SheetsService.Initializer();
        initializer.ApiKey = config.APIToken;

        var api = new SheetsService(initializer);

        var request = api.Spreadsheets.Values.Get(config.sheetID, config.dataRange);

        ConsoleDrawer.DrawText("Loading screenshots data...");
        var screenshotCollection =
            ConsoleDrawer.RunTask(Task.Run(() => ScreenshotsCollection.LoadCollection(config.localScreenshotsDBPath)));

        ConsoleDrawer.DrawText("Loading sheet data...");
        var sheetData = ConsoleDrawer.RunTask(request.ExecuteAsync());

        ConsoleDrawer.DrawText("Parsing data...");
        var parser = new Parser(screenshotCollection);

        var games = ConsoleDrawer.RunTask(Task.Run(() => parser.ParseGames(sheetData.Values)));

        ConsoleDrawer.DrawText($"parsed {games.Length} games");

        ConsoleDrawer.DrawText("Generating Json");

        var formatedGames = DBFormater.FormatGames(config, games);

        var jsonOptions = new JsonSerializerOptions() {
            IncludeFields = true,
            WriteIndented = true,
        };
        var json = JsonSerializer.Serialize(formatedGames, jsonOptions);

        ConsoleDrawer.DrawText("Saving Json");

        File.WriteAllText(config.gamesJsonPath, json);

/* // no more screenshots moving
ConsoleDrawer.DrawText("Moving Screenshots");

ConsoleDrawer.RunTask(Task.Run(() => screenshotCollection.Apply(config.siteLocalPath, games)));

ConsoleDrawer.DrawText("Done!");
*/

        return 0;
    }
}
