using System.Diagnostics;

namespace MLPGPPParser;

public static class ConsoleDrawer {
    public static int DrawWarning(string warningMessage) {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"warning: {warningMessage}");
        return 0;
    }

    public static int DrawError(string errorMessage) {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"error: {errorMessage}");
        return -1;
    }

    public static void DrawText(string text) {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    public static bool AskPermissionToContinue(string message) {
        while (true) {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{message} [Y/N]");
            Console.ResetColor();
            var key = Console.ReadKey();

            switch (key.Key) {
                case ConsoleKey.Y:
                    return true;
                case ConsoleKey.N:
                    return false;
            }
        }
    }

    private static TimeSpan _messagesDelay = TimeSpan.FromMilliseconds(500);
    private static TimeSpan _timeout = TimeSpan.FromSeconds(300); // 5 minutes

    public static void RunTask(Task task, string startMessage = "Start Task", TimeSpan? overrideTimeoutTime = null) {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(startMessage);

        Console.ForegroundColor = ConsoleColor.Blue;

        var nextMessageTime = _messagesDelay;
        var stopWatch = Stopwatch.StartNew();

        task.WaitAsync(overrideTimeoutTime ?? _timeout);

        while (task.IsCompleted == false) {
            if (stopWatch.Elapsed > nextMessageTime) {
                Console.Write($"\rExecuting task for {stopWatch.Elapsed:g} time");
                nextMessageTime += _messagesDelay;
            }
        }

        stopWatch.Stop();

        if (task.IsFaulted) {
            DrawError(task.Exception.Message);
            throw task.Exception;
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\rExecuted task for {stopWatch.Elapsed:g} time ");
    }

    public static T RunTask<T>(
        Task<T> task,
        string startMessage = "Start Task",
        TimeSpan? overrideTimeoutTime = null) {
        
        RunTask((Task)task, startMessage, overrideTimeoutTime);
        return task.Result;
    }
}
