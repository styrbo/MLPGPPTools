namespace MLPGPPParser;

public interface ITaskRunner {
    
    string TaskName { get; }
    
    int Run(string[] args);
}
