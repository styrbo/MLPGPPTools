
namespace MLPGPPParser.Parser;

public class ScreenshotsCollection {

    public record class ScreenshotBucketData(uint screenshotsCount, string[] screenshotsFormats);

    public ScreenshotBucketData[] buckets;

    private string _dbPath;
    
    public Screenshot GetHeroScreenshot(uint bucketID) {
        return new Screenshot(bucketID, 0, buckets[bucketID].screenshotsFormats[0]);
    }

    public List<Screenshot> GetOtherScreenshots(uint bucketID) {
        var list = new List<Screenshot>();
        var counts = buckets[bucketID].screenshotsCount;

        for (uint i = 1; i < counts; i++) {
            var screenshot = new Screenshot(bucketID, i, buckets[bucketID].screenshotsFormats[i]);
            list.Add(screenshot);
        }

        return list;
    }

    public void Apply(string targetLocationRoot, GameData[] games) {
        var screenshotsOnSiteDirectory = @$"{targetLocationRoot}/s/";
        if(Directory.Exists(screenshotsOnSiteDirectory))
            Directory.Delete(screenshotsOnSiteDirectory, recursive:true);
        Directory.CreateDirectory(screenshotsOnSiteDirectory);

        var neededScreenshotsBuckets = games
            .Where(g => g.HeroScreenshot != null)
            .Select(g => g.HeroScreenshot.BucketId).Distinct();

        foreach (var screenshotsBucket in neededScreenshotsBuckets) {
            
            var targetBucketPath = $"{screenshotsOnSiteDirectory}{screenshotsBucket}";
            
            FileUtilities.CopyDirectory(
                $"{_dbPath}/{screenshotsBucket}",
                targetBucketPath, true);
            
            //fix screenshots namings

            var files = Directory.GetFiles(targetBucketPath);
            var filesNames = files.Select(Path.GetFileNameWithoutExtension).ToArray();
            var filesExtensions = files.Select(Path.GetExtension).ToArray();


            var filesIndexes = filesNames
                .Where(name => uint.TryParse(name, out _))
                .Select(uint.Parse)
                .OrderBy(id => id)
                .ToList();

            for (uint i = 0; i < files.Length; i++) {
                var fileName = filesNames[i];
                if (uint.TryParse(fileName, out var id) == false) {
                    ConsoleDrawer.DrawError($"Invalid screenshot name: {fileName} at {screenshotsBucket} bucket");
                }
                else if (filesIndexes.Contains(i) == false) {
                    ConsoleDrawer.DrawError($"Screenshot id {i} not found at {screenshotsBucket} bucket");
                }
            }
            
            /* my attempt to automatize fixing names: not worth it ngl
            var filteredFiles = new List<uint>(files.Length);
            for (int i = 0; i < files.Length; i++) {
                var fileName = filesNames[i];
                if (uint.TryParse(fileName, out var id) != false) continue;
                if(filteredFiles.Contains(id) == false) 
                    filteredFiles.Add(id);
            }

            for (int i = 0; i < filesNames.Length; i++) {
                var fileName = filesNames[i];
                if (int.TryParse(fileName, out var id) == false || id != i) {
                    ConsoleDrawer.DrawWarning($"Screenshot name is not follow our convention: " +
                                              $"for bucket {screenshotsBucket}," +
                                              $" current screenshot name is {fileName}{filesExtensions[i]}," +
                                              $" expected name is {i}{filesExtensions[i]}");
                    
                    var newName = $"{i}{filesExtensions[i]}";
                    var newPath = $"{targetBucketPath}/{newName}";
                    File.Move(files[i], newPath);
                    files[i] = newPath;
                }
            }
            
            */
        }
    }

    public static ScreenshotsCollection LoadCollection(string screenshotsDBPath) {
        List<ScreenshotBucketData> items = [];
        uint currentBucketID = 0;
        while (true) {
            var folderPath = Path.Combine(screenshotsDBPath, currentBucketID.ToString());

            if (Directory.Exists(folderPath) == false)
                break;
            
            var files = Directory.GetFiles(folderPath);

            var screenshots = ScreenshotsId(files)
                .Order()
                .ToArray();

            if (screenshots.Length == 0)
                break;
            if (screenshots[0].Item1 != 0) {
                ConsoleDrawer.DrawError($"screenshots id's not correct for: {folderPath}");
                continue;
            }
            
            var formats = screenshots.Select(s => s.Item2).ToArray();
            
            items.Add(new ScreenshotBucketData((uint) screenshots.Length, formats));
            currentBucketID++;
        }
        
        
        var result = new ScreenshotsCollection {
            buckets = items.ToArray(),
            _dbPath = screenshotsDBPath
        };
        
        ConsoleDrawer.DrawText($"Loaded {items.Count} screenshots buckets");

        return result;
    }

    private static IEnumerable<(uint, string)> ScreenshotsId(string[] files) {
        foreach (var file in files) {
            if (ValidateScreenshot(file, out var screenshotID, out var format))
                yield return (screenshotID, format);
        }
    }

    private static bool ValidateScreenshot(string filePath, out uint screenshotID, out string format) {
        screenshotID = 0;
        format = "";
        
        filePath = filePath.Replace('\\', '/');

        if (string.IsNullOrWhiteSpace(filePath))
            return false;

        var splittedPath = filePath.Split('/');

        if (splittedPath.Length < 2)
            return false;

        var splitedFile = splittedPath[^1].Split('.');

        if (splitedFile.Length > 2)
            return false;
        
        format = splitedFile[^1]; 

        return uint.TryParse(splitedFile[0], out screenshotID);
    }
}
