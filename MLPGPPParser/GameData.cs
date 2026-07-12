namespace MLPGPPParser;

public enum Label {
    Genre,
    Tags,
    Status,
    Engine,
    Character,
    Playtime,

    AgeRestriction,

    BlackTags,
}

public enum Tag {
    None = 0,

    //Genre
    visual_novel,
    puzzle,
    action,
    rpg,
    platformer,
    jrpg,
    Shoot_em_Up,
    arcade,
    pointANDclick,
    tamagotchi,
    adventure,
    shooter,
    clicker,
    karaoke,
    horror,
    detective,
    racing,
    open_world,
    side_scroller,
    survival,
    tower_defence,
    mmo,
    fighting,
    casual,
    sport,
    strategy,
    humor,
    quest,
    sandbox,
    simulation,
    dress_up,
    social,
    stealth,

    //Tags
    _2d,
    _3d,
    top_down,
    first_person,
    thrid_person,
    cash_grab,
    official,
    coop,
    online,

    //Status
    Released,
    In_Development,

    //Engine
    customORnone,
    flash,
    unity,
    ue,
    godot,
    gamemaker,
    construct,
    rpgmaker,
    RenPy,
    blender,
    roblox,
    html5,
    Java,
    Adventure_Game_Studio,
    FM2K,
    Z_Engine,
    LoveEngine,
    ClickteamFusion,
    GDevelop,

    //Character
    M6,
    CMC,
    Secondary_cast,
    Background,
    OC,

    //Playtime
    LESS_5_min,
    FROM_5m_TO_30m,
    FROM_30m_TO_2h,
    FROM_2h_TO_4h,
    FROM_4h_TO_10h,
    MORE_10h,

    //Platform
    Platform_Web,
    Platform_Windows,
    Platform_OSX,
    Platform_WindowsX32,
    Platform_Android,
    Platform_Linux,
    Platform_Ios,
    Platform_Ps4,
    Platform_Xboxone,
    Platform_Switch,
    Platform_SNES,
    Platform_NintendoDS,
    Platform_GBA,
    Platform_GameBoy,
    Platform_Roblox,
    Platform_Source_Code,
    Platform_VR,
    Platform_Flash,

    //Age restriction
    SFW,
    NSFW,
    Gore,

    //Black Tags
    Human,
    Anthro,
    Pony_Inspired,
}

public enum DownloadServerType {
    None,
    MareBy,
    Scrogne,
    Zana,
    Telegram,
    PoneRs,
}

public class TagsCollection {
    public Dictionary<Label, List<Tag>> Tags;

    public TagsCollection(IEnumerable<Tag> tags) {
        var tagsCollection = new Dictionary<Label, List<Tag>>();

        foreach (var tag in tags) {
            if (tagsCollection.ContainsKey(GetLabelFromTag(tag)) == false) {
                tagsCollection.Add(GetLabelFromTag(tag), new List<Tag>());
            }

            tagsCollection[GetLabelFromTag(tag)].Add(tag);
        }

        Tags = tagsCollection;
    }

    public Label GetLabelFromTag(Tag tag) {
        switch (tag) {
            case Tag.visual_novel: return Label.Genre;
            case Tag.puzzle: return Label.Genre;
            case Tag.action: return Label.Genre;
            case Tag.rpg: return Label.Genre;
            case Tag.platformer: return Label.Genre;
            case Tag.jrpg: return Label.Genre;
            case Tag.Shoot_em_Up: return Label.Genre;
            case Tag.arcade: return Label.Genre;
            case Tag.pointANDclick: return Label.Genre;
            case Tag.tamagotchi: return Label.Genre;
            case Tag.adventure: return Label.Genre;
            case Tag.shooter: return Label.Genre;
            case Tag.clicker: return Label.Genre;
            case Tag.karaoke: return Label.Genre;
            case Tag.horror: return Label.Genre;
            case Tag.detective: return Label.Genre;
            case Tag.racing: return Label.Genre;
            case Tag.open_world: return Label.Genre;
            case Tag.side_scroller: return Label.Genre;
            case Tag.survival: return Label.Genre;
            case Tag.tower_defence: return Label.Genre;
            case Tag.mmo: return Label.Genre;
            case Tag.fighting: return Label.Genre;
            case Tag.casual: return Label.Genre;
            case Tag.sport: return Label.Genre;
            case Tag.strategy: return Label.Genre;
            case Tag.humor: return Label.Genre;
            case Tag.quest: return Label.Genre;
            case Tag.sandbox: return Label.Genre;
            case Tag.simulation: return Label.Genre;
            case Tag.dress_up: return Label.Genre;
            case Tag.social: return Label.Genre;
            case Tag.stealth: return Label.Genre;

            case Tag._2d: return Label.Tags;
            case Tag._3d: return Label.Tags;
            case Tag.top_down: return Label.Tags;
            case Tag.first_person: return Label.Tags;
            case Tag.thrid_person: return Label.Tags;
            case Tag.cash_grab: return Label.Tags;
            case Tag.official: return Label.Tags;
            case Tag.coop: return Label.Tags;
            case Tag.online: return Label.Tags;

            case Tag.Released: return Label.Status;
            case Tag.In_Development: return Label.Status;

            case Tag.customORnone: return Label.Engine;
            case Tag.flash: return Label.Engine;
            case Tag.unity: return Label.Engine;
            case Tag.ue: return Label.Engine;
            case Tag.godot: return Label.Engine;
            case Tag.gamemaker: return Label.Engine;
            case Tag.construct: return Label.Engine;
            case Tag.rpgmaker: return Label.Engine;
            case Tag.RenPy: return Label.Engine;
            case Tag.blender: return Label.Engine;
            case Tag.roblox: return Label.Engine;
            case Tag.html5: return Label.Engine;
            case Tag.Java: return Label.Engine;
            case Tag.Adventure_Game_Studio: return Label.Engine;
            case Tag.FM2K: return Label.Engine;
            case Tag.Z_Engine: return Label.Engine;
            case Tag.LoveEngine: return Label.Engine;
            case Tag.ClickteamFusion: return Label.Engine;
            case Tag.GDevelop: return Label.Engine;

            case Tag.M6: return Label.Character;
            case Tag.CMC: return Label.Character;
            case Tag.Secondary_cast: return Label.Character;
            case Tag.Background: return Label.Character;
            case Tag.OC: return Label.Character;

            case Tag.LESS_5_min: return Label.Playtime;
            case Tag.FROM_5m_TO_30m: return Label.Playtime;
            case Tag.FROM_30m_TO_2h: return Label.Playtime;
            case Tag.FROM_2h_TO_4h: return Label.Playtime;
            case Tag.FROM_4h_TO_10h: return Label.Playtime;
            case Tag.MORE_10h: return Label.Playtime;

            case Tag.NSFW: return Label.AgeRestriction;
            case Tag.SFW: return Label.AgeRestriction;
            case Tag.Gore: return Label.AgeRestriction;

            case Tag.Anthro: return Label.BlackTags;
            case Tag.Human: return Label.BlackTags;
            case Tag.Pony_Inspired: return Label.BlackTags;

            default: throw new Exception($"can't find label for {tag}");
        }
    }
}

public class TimeStamp {
    public enum DateFormat {
        YearOnly,
        ddMMyyyy,
        NoData
    }

    public DateFormat Format;
    public DateTime DateTime;
}

public record Screenshot(uint BucketId, uint Id, string format);

public record GameBuild(
    uint Id,
    string Version,
    TimeStamp ReleaseDate,
    TimeStamp DateAdded,
    Tag releaseStatus,
    Tag platform,
    DownloadServer[] servers);

public record DownloadServer(DownloadServerType type, string url);

public record GameData(
    uint Id,
    string Name,
    string Description,
    TagsCollection Tags,
    string SourceURL,
    string[] Authors,
    Tag playtime,
    Screenshot HeroScreenshot,
    List<Screenshot> OtherScreenshots,
    List<GameBuild?>? Builds) {
    
    public TimeStamp GetReleaseData() {
        if (Builds == null || Builds.Count == 0) {
            return new TimeStamp() { Format = TimeStamp.DateFormat.NoData };
        }

        var firstReleaseBuild = Builds
            .Where(b => b != null)
            .OrderBy(b => {
                
                var isBuildIsRelease = b.releaseStatus == Tag.Released;
                
                // if build is release then it should count as lower in this list
                
                if(b.ReleaseDate.Format == TimeStamp.DateFormat.NoData) {
                    return new DateTime();
                }
                
                return b.ReleaseDate.DateTime - (isBuildIsRelease ? TimeSpan.Zero : TimeSpan.FromDays(365 * 100));
            })
            .FirstOrDefault();
        
        if(firstReleaseBuild == null)
            return new TimeStamp { Format = TimeStamp.DateFormat.NoData };

        return firstReleaseBuild.ReleaseDate;
    }

    public TimeStamp GetFirstAddedData() {
        if (Builds == null || Builds.Count == 0) {
            return new TimeStamp() { Format = TimeStamp.DateFormat.NoData };
        }
        
        var firstAddedBuild = Builds
            .Where(b => b != null)
            .OrderBy(b => b.DateAdded.DateTime)
            .FirstOrDefault();
        
        return firstAddedBuild?.DateAdded ?? new TimeStamp { Format = TimeStamp.DateFormat.NoData };
    }
}

[Serializable]
public class SerializedGamesCollectionData {
    public GameSerializedData[] games;
}

[Serializable]
public class SerializedTagData {
    public string label;
    public string type;
}

[Serializable]
public class SerializedDownloadData {
    public string label;
    public string version; // can be empty
    public string url;
    public string buttonText;
    public BuildMirrorData[] mirrors;
}

[Serializable]
public class BuildMirrorData {
    public string url;
    public string label;
}

[Serializable]
public record GameSerializedData {
    public int id;
    public string title;
    public string[] author;
    public string thumbnail;
    public string shortDescription;
    public string fullDescription;
    public SerializedTagData[] tags;
    public string[] characters;
    public string playtime;

    public string status; //new
    public string engine; //new
    public string[] platforms;

    public string url;
    
    public string releaseDate;
    public string dateAdded;

    public SerializedDownloadData[] downloads; //new

    public string[] screenshots;
}
