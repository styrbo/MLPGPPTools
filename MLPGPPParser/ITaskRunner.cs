namespace MLPGPPParser;

public interface ITaskRunner {
    
    string TaskName { get; }
    
    Task<int> Run(string[] args);
}
