// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using MLPGPPParser;
using MLPGPPParser.DataManager;
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
    new DataManagerRunner(),
};

var commandArgs = GlobalOptions.Extract(args);
if (commandArgs == null)
    return -1;

if (commandArgs.Length > 0 && commandArgs[0] == "run") {
    await TryRunTask(commandArgs.Skip(1).ToArray());
} else {
    Logger.LogInfo("Waiting for the command");
    var line = Console.ReadLine();
    var lineArgs = GlobalOptions.Extract(line.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    if (lineArgs == null)
        return -1;
    await TryRunTask(lineArgs);
}

return 0;

async Task TryRunTask(string[] runArgs) {
    if (runArgs.Length == 0) {
        Logger.LogError("no task given");
        return;
    }

    var neededRunner = runArgs[0];
    var runner = runners.FirstOrDefault(runner => runner.TaskName == neededRunner);
    
    if(runner == null)
        return;
    
    await runner.Run(runArgs.Skip(1).ToArray());
}
