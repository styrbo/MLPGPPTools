// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using MLPGPPParser;
using MLPGPPParser.FoldersManager;
using MLPGPPParser.Parser;

// Initialize Logger with default path
Logger.Initialize(@"O:\MLGPP\logs");

//new FoldersManagerRunner().Run(args);
//new ParserRunner().Run(args);

//return 0;

//help d

var runners = new List<ITaskRunner>
{
    new HashingRunner(),
    new ParserRunner(),
};

if (args.Length > 0 && args[0] == "run") {
    await TryRunTask(args.Skip(1).ToArray());
} else {
    Logger.LogInfo("Waiting for the command");
    var line = Console.ReadLine();
    args = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    await TryRunTask(args);
}

return 0;

async Task TryRunTask(string[] runArgs) {
    var neededRunner = runArgs[0];
    var runner = runners.FirstOrDefault(runner => runner.TaskName == neededRunner);
    
    if(runner == null)
        return;
    
    await runner.Run(runArgs.Skip(1).ToArray());
}
