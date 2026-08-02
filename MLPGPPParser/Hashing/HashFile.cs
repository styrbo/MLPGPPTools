using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace MLPGPPParser;

public struct Hash {
    public HashAlgorithmType AlgorithmType;
    public string Value;
}

public class HashFile {

    public const string SecretPassPhrase = "Memory Leak Slopity Check! 67 /)";

    public string Path { get; private set; }
    public bool IsValid { get; private set; }
    
    public Hash[] Hashes { get; private set; }
    
    private HashFile() {
        
    }

    public static async Task<HashFile> LoadHashFile(string path) {
        var hashFile = new HashFile();
        hashFile.Path = path;

        try {
            using var stream = new StreamReader(path);
            var line = await stream.ReadLineAsync();
            
            if(line is not SecretPassPhrase) {
                hashFile.IsValid = false;
                return hashFile;
            }
            
            var hashes = new List<Hash>();
            while (true) {
                var hashLine = await stream.ReadLineAsync();
                
                if(hashLine is null) {
                    break;
                }
                
                hashFile.IsValid = true;
                
                var parts = hashLine.Split(" ", 2);
                var hash = new Hash {
                    AlgorithmType = Enum.Parse<HashAlgorithmType>(parts[0], ignoreCase:true),
                    Value = parts[1]
                };
                
                hashes.Add(hash);
            }
            
            hashFile.Hashes = hashes.ToArray();
            
        } catch (Exception e) {
            hashFile.IsValid = false;
            return hashFile;
        }
        
        return hashFile;
    }
    
    public async Task Save(string path) {
        using var stream = new StreamWriter(path);
        await stream.WriteLineAsync(SecretPassPhrase);
        foreach (var hash in Hashes) {
            await stream.WriteLineAsync($"{hash.AlgorithmType} {hash.Value}");
        }
    }

    public static async Task<HashFile> Calculate(string path) {
        if(File.Exists(path) == false) {
            ConsoleDrawer.DrawWarning($"no file found for {path}, skipping");
            return null;
        }

        await using var gameFileStream = new FileStream(path, FileMode.Open);

        var hashFile = new HashFile {
            Path = path,
            IsValid = true,
            Hashes = new Hash[1]
        };
        
        var hashBytes = await MD5.HashDataAsync(gameFileStream);
        hashFile.Hashes[0] = new Hash {
            AlgorithmType = HashAlgorithmType.Md5,
            Value = Convert.ToHexString(hashBytes)
        };
        
        return hashFile;
    }

    public bool CompareHashes(HashFile? other) {
        if (other == null)
            return false;
        
        if(IsValid == false || other.IsValid == false) {
            return false;
        }
        
        if (Hashes.Length != other.Hashes.Length) {
            return false;
        }

        for (int i = 0; i < Hashes.Length; i++) {
            if (Hashes[i].AlgorithmType != other.Hashes[i].AlgorithmType || Hashes[i].Value != other.Hashes[i].Value) {
                return false;
            }
        }

        return true;
    }

    public override string ToString() {
        var sb = new StringBuilder();
        sb.AppendLine($"Hash File: {Path}");
        sb.AppendLine($"Valid: {IsValid}");
        sb.AppendLine("Hashes:");
        foreach (var hash in Hashes) {
            sb.AppendLine($"  {hash.AlgorithmType}: {hash.Value}");
        }
        return sb.ToString();
    }
}
