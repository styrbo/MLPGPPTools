namespace MLPGPPParser;

public class Parser {

    public const string UnknownSourceURL = "https://games.mare.by/-67";
    
    public enum ColumnType : int {
        Id = 0,
        Name = 1,
        Version = 2,
        Platform = 3,
        Status = 6,
        Engine = 7,
        Source = 8,
        SourcePlatform = 9,
        ArchiveBuildId = 10,
        Distributing = 11,
        AgeRestriction = 12,
        ScreenshotsBucketId = 14,
        ShortDescription = 15,
        GenresOrTags = 16,
        Creator = 17,
        ReleaseData = 18,
        RecordTypeData = 19,
        Characters = 20,
        Playtime = 21,
        LastUpdate = 22,
    }

    public enum RecordType {
        None = 0,
        Game,
        Mod,
        GameSource,
        Lost,
        BuildBroken,
        DLC,
    }

    public Parser(ScreenshotsCollection screenshotsCollection) {
        _screenshotsCollection = screenshotsCollection;
    }

    private ScreenshotsCollection _screenshotsCollection;

    private static RecordType GetRecordType(string type) {
        switch (type) {
            case "Game":
                return RecordType.Game;
            case "MOD":
                return RecordType.Mod;
            case "Game Source":
                return RecordType.GameSource;
            case "Lost":
                return RecordType.Lost;
            case "Build Broken":
                return RecordType.BuildBroken;
            case "DLC":
                return RecordType.DLC;
            default:
                return RecordType.None;
        }
    }

    private static Tag GetTag(string tag, ColumnType column) {
        tag = tag.ToLower().Replace(" ", "");

        //todo fix this shit
        if (tag == "roblox") {
            return column == ColumnType.Platform ? Tag.Platform_Roblox : Tag.roblox;
        }

        if (tag == "flash") {
            return column == ColumnType.Platform ? Tag.Platform_Flash : Tag.flash;
        }

        switch (tag) {
            case "visualnovel":
                return Tag.visual_novel;
            case "puzzle":
                return Tag.puzzle;
            case "action":
                return Tag.action;
            case "rpg":
                return Tag.rpg;
            case "platformer":
                return Tag.platformer;
            case "jrpg":
                return Tag.jrpg;
            case "shoot'emup":
                return Tag.Shoot_em_Up;
            case "arcade":
                return Tag.arcade;
            case "point&click":
                return Tag.pointANDclick;
            case "tamagotchi":
                return Tag.tamagotchi;
            case "adventure":
                return Tag.adventure;
            case "shooter":
                return Tag.shooter;
            case "clicker":
                return Tag.clicker;
            case "karaoke":
                return Tag.karaoke;
            case "horror":
                return Tag.horror;
            case "detective":
                return Tag.detective;
            case "racing":
                return Tag.racing;
            case "openworld":
                return Tag.open_world;
            case "sidescroller":
                return Tag.side_scroller;
            case "survival":
                return Tag.survival;
            case "towerdefence":
                return Tag.tower_defence;
            case "mmo":
                return Tag.mmo;
            case "fighting":
                return Tag.fighting;
            case "casual":
                return Tag.casual;
            case "sport":
                return Tag.sport;
            case "strategy":
                return Tag.strategy;
            case "humor":
                return Tag.humor;
            case "quest":
                return Tag.quest;
            case "sandbox":
                return Tag.sandbox;
            case "simulation":
                return Tag.simulation;
            case "dressup":
                return Tag.dress_up;
            case "social":
                return Tag.social;
            case "2d":
                return Tag._2d;
            case "3d":
                return Tag._3d;
            case "topdown":
                return Tag.top_down;
            case "firstperson":
                return Tag.first_person;
            case "thirdperson":
                return Tag.thrid_person;
            case "cashgrab":
                return Tag.cash_grab;
            case "official":
                return Tag.official;
            case "coop":
                return Tag.coop;
            case "online":
                return Tag.online;
            case "released":
                return Tag.Released;
            case "indevelopment":
                return Tag.In_Development;
            case "custom/none":
                return Tag.customORnone;
            case "unity":
                return Tag.unity;
            case "ue":
                return Tag.ue;
            case "godot":
                return Tag.godot;
            case "gamemaker":
                return Tag.gamemaker;
            case "construct":
                return Tag.construct;
            case "rpgmaker":
                return Tag.rpgmaker;
            case "ren'py":
                return Tag.RenPy;
            case "blender":
                return Tag.blender;
            case "html5":
                return Tag.html5;
            case "java":
                return Tag.Java;
            case "adventuregamestudio":
                return Tag.Adventure_Game_Studio;
            case "fm2k":
                return Tag.FM2K;
            case "z-engine":
                return Tag.Z_Engine;
            case "löve":
                return Tag.LoveEngine;
            case "clickteamfusion":
                return Tag.ClickteamFusion;
            case "m6":
                return Tag.M6;
            case "cmc":
                return Tag.CMC;
            case "secondarycast":
                return Tag.Secondary_cast;
            case "background":
                return Tag.Background;
            case "oc":
                return Tag.OC;
            case "<5min":
                return Tag.LESS_5_min;
            case "5m-30m":
                return Tag.FROM_5m_TO_30m;
            case "30m-2h":
                return Tag.FROM_30m_TO_2h;
            case "2h-4h":
                return Tag.FROM_2h_TO_4h;
            case "4h-10h":
                return Tag.FROM_4h_TO_10h;
            case "10h+":
                return Tag.MORE_10h;

            case "web":
                return Tag.Platform_Web;
            case "windows":
                return Tag.Platform_Windows;
            case "osx":
                return Tag.Platform_OSX;
            case "windows_x32":
                return Tag.Platform_WindowsX32;
            case "android":
                return Tag.Platform_Android;
            case "linux":
                return Tag.Platform_Linux;
            case "ios":
                return Tag.Platform_Ios;
            case "ps4":
                return Tag.Platform_Ps4;
            case "xboxone":
                return Tag.Platform_Xboxone;
            case "switch":
                return Tag.Platform_Switch;
            case "snes":
                return Tag.Platform_SNES;
            case "nintendods":
                return Tag.Platform_NintendoDS;
            case "gba":
                return Tag.Platform_GBA;
            case "gameboy":
                return Tag.Platform_GameBoy;
            case "sourcecode":
                return Tag.Platform_Source_Code;
            case "vr":
                return Tag.Platform_VR;
            
            case "sfw":
                return Tag.SFW;
            case "nsfw":
                return Tag.NSFW;
            
            //Black Tags
            case "anthro":
                return Tag.Anthro;
            case "human":
                return Tag.Human;
            case "ponyinspired":
                return Tag.Pony_Inspired;

            default:
                return Tag.None;
        }
    }

    // op auto conversion from int to column type

    private static bool IsCellHasContent(string line) {
        if (string.IsNullOrWhiteSpace(line) == false && line != "?" && line != "~")
            return true;

        return false;
    }

    private static string GetDataFromCell(IList<object> line, ColumnType columnType) {
        if (line.Count <= (int)columnType)
            return string.Empty;

        var obj = line[(int)columnType];

        return (string)obj;
    }

    private static IEnumerable<Tag> GetTags(string lane, char separator = ',') {
        var tagsText = lane.Split(separator);
        foreach (var tagText in tagsText) {
            var tag = GetTag(tagText, ColumnType.GenresOrTags);

            if (tag != Tag.None) {
                yield return tag;
            }
        }
    }

    private static Tag GetDevelopmentStatusTag(string status) {
        switch (status) {
            case "released":
                return Tag.Released;
            default:
                return Tag.In_Development;
        }
    }

    private static string[] ParseAuthors(string lane) {
        if (IsCellHasContent(lane) == false)
            return ["?"];
        
        return lane
            .Replace("\\", "/")
            .Split('/')
            .SelectMany(x => x.Split(','))
            .Select(x => x.Trim())
            .Where(x => string.IsNullOrWhiteSpace(x) == false)
            .ToArray();
    }

    public GameData[] ParseGames(IList<IList<object>> sheetData) {
        var games = new Dictionary<uint, GameData>();

        for (var index = 0; index < sheetData.Count; index++) {
            var row = sheetData[index];

            if (TryParseLine(games, row, index, out var errorMessage) == false) {
                ConsoleDrawer.DrawError($"can't parse row {index} {errorMessage}");

                continue;
            } else {
                //ConsoleDrawer.DrawText($"parsed row {index}");
            }
        }

        return games.Values.ToArray();
    }

    public static bool IsValidGameEntry(IList<object> line, int lineIndex, out string errorMessage) {
        if (line.All(o => string.IsNullOrWhiteSpace(o.ToString()))) {
            errorMessage = "empty lane";
            return false;
        }

        var idData = GetDataFromCell(line, ColumnType.Id);

        if (uint.TryParse(idData, out var id) == false) {
            errorMessage = "id is not a number";
            return false;
        }

        var name = GetDataFromCell(line, ColumnType.Name);
        if (string.IsNullOrWhiteSpace(name)) {
            errorMessage = "name is empty";
            return false;
        }

        var buildIdText = GetDataFromCell(line, ColumnType.ArchiveBuildId);
        var sourceLink = GetDataFromCell(line, ColumnType.Source);
        if (IsCellHasContent(buildIdText) == false && IsCellHasContent(sourceLink) == false) {
            errorMessage = "no valid build id or source";
            return false;
        }

        var recordTypeText = GetDataFromCell(line, ColumnType.RecordTypeData);
        var recordType = GetRecordType(recordTypeText);
        if (recordType != RecordType.Game && recordType != RecordType.GameSource) {
            errorMessage = "not a game/source";
            return false;
        }

        errorMessage = "";
        return true;
    }

    public bool TryParseLine(IDictionary<uint, GameData> games, IList<object> line, int lineIndex, out string errorMessage) {
        if (IsValidGameEntry(line, lineIndex, out errorMessage) == false) {
            return false;
        }

        var gameIdText = GetDataFromCell(line, ColumnType.Id);
        var gameID = uint.Parse(gameIdText);

        if (games.TryGetValue(gameID, out var gameData) == false) {
            var gameName = GetDataFromCell(line, ColumnType.Name);
            var shortDescription = GetDataFromCell(line, ColumnType.ShortDescription);
            var genreOrTags = GetDataFromCell(line, ColumnType.GenresOrTags);
            var statusData = GetDataFromCell(line, ColumnType.Status);
            var engineData = GetDataFromCell(line, ColumnType.Engine);
            var characterData = GetDataFromCell(line, ColumnType.Characters);
            var playtimeData = GetDataFromCell(line, ColumnType.Playtime);
            var authorsLane = GetDataFromCell(line, ColumnType.Creator);
            var ageRestriction = GetDataFromCell(line, ColumnType.AgeRestriction);

            var releaseData = GetDataFromCell(line, ColumnType.ReleaseData);

            var screenshotsBucketId = GetDataFromCell(line, ColumnType.ScreenshotsBucketId);
            
            var authors = ParseAuthors(authorsLane);

            var tags = GetTags(genreOrTags);

            if (IsCellHasContent(statusData)) {
                var statusTag = GetDevelopmentStatusTag(statusData);
                tags = tags.Append(statusTag);
            }

            if (IsCellHasContent(engineData)) {
                var engine = GetTag(engineData, ColumnType.Engine);
                tags = tags.Append(engine);
            }

            if (IsCellHasContent(characterData)) {
                var characters = GetTags(characterData);
                tags = tags.Concat(characters);
            } else {
                ConsoleDrawer.DrawWarning($"row {lineIndex} has no characters");
            }

            var playtime = Tag.None;
            if (IsCellHasContent(playtimeData)) {
                playtime = GetTag(playtimeData, ColumnType.Playtime);
                tags = tags.Append(playtime);
            } else {
                ConsoleDrawer.DrawWarning($"row {lineIndex} has no playtime");
            }

            var ageRestrictionTag = Tag.None;
            if(IsCellHasContent(ageRestriction)) {
                ageRestrictionTag = GetTag(ageRestriction, ColumnType.AgeRestriction);
            } else {
                ConsoleDrawer.DrawWarning($"row {lineIndex} has no age restriction, using NSFW");
                ageRestrictionTag = Tag.NSFW;
            }
            tags = tags.Append(ageRestrictionTag);

            Screenshot heroScreenshot = null;
            List<Screenshot> otherScreenshots = null;
            if (IsCellHasContent(screenshotsBucketId)
                && uint.TryParse(screenshotsBucketId, out var screenshotsBuckedId)) {
                heroScreenshot = _screenshotsCollection.GetHeroScreenshot(screenshotsBuckedId);
                otherScreenshots = _screenshotsCollection.GetOtherScreenshots(screenshotsBuckedId);
            }

            if (TryParseGameBuild(line, out var build, out var buildErrorMessage) == false) {
                errorMessage = buildErrorMessage;
                return false;
            }

            var sourceURL = GetDataFromCell(line, ColumnType.Source);
            if(IsCellHasContent(sourceURL) == false) {
                ConsoleDrawer.DrawWarning($"row {lineIndex} has no source url, using 404 page");
                sourceURL = UnknownSourceURL;
            }

            if (tags.Contains(Tag.None)) {
                ConsoleDrawer.DrawWarning($"row {lineIndex} has no tags");
            }

            gameData = new GameData(
                Id: gameID,
                Name: gameName,
                ShortDescription: shortDescription,
                Tags: new TagsCollection(tags),
                ReleaseDate: releaseData,
                SourceURL: sourceURL,
                Authors: authors,
                playtime: playtime,
                HeroScreenshot: heroScreenshot,
                OtherScreenshots: otherScreenshots,
                new List<GameBuild> {
                    build
                }
            );

            games[gameID] = gameData;
            errorMessage = "";
            return true;
        }

        if (TryParseGameBuild(line, out var build1, out var buildErrorMessage1) == false) {
            errorMessage = buildErrorMessage1;
            return false;
        }

        gameData.Builds.Add(build1);
        errorMessage = "";
        return true;
    }

    private bool TryParseGameBuild(IList<object> line, out GameBuild? build, out string errorMessage) {
        var buildIdText = GetDataFromCell(line, ColumnType.ArchiveBuildId);
        var version = GetDataFromCell(line, ColumnType.Version);
        var releaseDate = GetDataFromCell(line, ColumnType.ReleaseData);
        var platformText = GetDataFromCell(line, ColumnType.Platform);

        if (IsCellHasContent(buildIdText) == false ||
            uint.TryParse(buildIdText, out var buildId) == false) {
            build = null;
            errorMessage = "";
            return true;
        }

        if (IsCellHasContent(platformText) == false) {
            build = null;
            errorMessage = "no platform info";
            return false;
        }

        var platform = GetTag(platformText, ColumnType.Platform);

        build = new GameBuild(buildId, version, releaseDate, platform);
        errorMessage = "";
        return true;
    }
}
