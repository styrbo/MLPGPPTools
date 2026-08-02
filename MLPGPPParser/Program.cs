// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using MLPGPPParser;
using MLPGPPParser.Parser;

var runners = new List<ITaskRunner>
{
    new HashingRunner(),
};

if (args[0] == "run") {
    var neededRunner = args[1];
    
    runners.FirstOrDefault(runner => runner.TaskName == neededRunner)?.Run(args.Skip(2).ToArray());
}

return 0;
