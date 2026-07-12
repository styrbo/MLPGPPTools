namespace MLPGPPParser;

public static class DBFormater {
    private const string ScreenshotsFolderLocalPath = @"/s/";

    private static readonly string[] ScreenshotsPlaceholders = [
        "/assets/gamarenopic1.jpg",
        "/assets/gamarenopic2.jpg",
        "/assets/gamarenopic3.jpg",
    ];

    private const string BuildLabelDownloadOption = "Download";
    private const string BuildLabelPlayOnOurEmulatorOption = "Play";
    private const DownloadServerType MainServer = DownloadServerType.MareBy;

    private static int _currentPlaceholderScreenshotIndex = 0;

    //"https://derpicdn.net/img/view/2023/10/17/3221228.png"; //"https://placehold.co/800x450/533483/eaeaea?text=Screenshot+1";

    private static string GetMainDownloadURL(uint buildID) {
        return $"https://games.mare.by/{buildID}";
    }

    private static string GetFlashEmulatorURL(uint buildID) {
        return $"https://flash.mare.by/g?id={buildID}";
    }

    private static string GetLabelText(Label label) {
        switch (label) {
            case Label.Genre:
                return "Genre";
            case Label.Tags:
                return "Other";
            case Label.Status:
                return "Status";
            case Label.Engine:
                return "Engine";
            case Label.Character:
                return "Character";
            case Label.Playtime:
                return "Playtime";
            case Label.AgeRestriction:
                return "warning";
            case Label.BlackTags:
                return "Black Tags";
            default:
                throw new ArgumentOutOfRangeException(nameof(label), label, null);
        }
    }

    private static string GetServerLabelText(DownloadServerType serverType) {
        switch (serverType) {
            case DownloadServerType.MareBy:
                return "mare.by";
            case DownloadServerType.Telegram:
                return "telegram";
            case DownloadServerType.PoneRs:
                return "pone.rs";
            case DownloadServerType.Scrogne:
                return "Scrogne";
            case DownloadServerType.Zana:
                return "Zana (IPv6)";
            default:
                throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
        }
    }

    private static string FormatTag(Tag tag) {
        switch (tag) {
            //Genre  
            case Tag.visual_novel:
                return "Visual Novel";
            case Tag.puzzle:
                return "Puzzle";
            case Tag.action:
                return "Action";
            case Tag.rpg:
                return "RPG";
            case Tag.platformer:
                return "Platformer";
            case Tag.jrpg:
                return "jRPG";
            case Tag.Shoot_em_Up:
                return "Shoot 'em Up";
            case Tag.arcade:
                return "Arcade";
            case Tag.pointANDclick:
                return "Point & Click";
            case Tag.tamagotchi:
                return "Tamagotchi";
            case Tag.adventure:
                return "Adventure";
            case Tag.shooter:
                return "Shooter";
            case Tag.clicker:
                return "Clicker";
            case Tag.karaoke:
                return "Karaoke";
            case Tag.horror:
                return "Horror";
            case Tag.detective:
                return "Detective";
            case Tag.racing:
                return "Racing";
            case Tag.open_world:
                return "Open World";
            case Tag.side_scroller:
                return "Side Scroller";
            case Tag.survival:
                return "Survival";
            case Tag.tower_defence:
                return "Tower Defence";
            case Tag.mmo:
                return "MMO";
            case Tag.fighting:
                return "Fighting";
            case Tag.casual:
                return "Casual";
            case Tag.sport:
                return "Sport";
            case Tag.strategy:
                return "Strategy";
            case Tag.humor:
                return "Humor";
            case Tag.quest:
                return "Quest";
            case Tag.sandbox:
                return "Sandbox";
            case Tag.simulation:
                return "Simulation";
            case Tag.dress_up:
                return "Dress Up";
            case Tag.social:
                return "Social";
            case Tag.stealth:
                return "Stealth";
            //Tags    
            case Tag._2d:
                return "2D";
            case Tag._3d:
                return "3D";
            case Tag.top_down:
                return "Top Down";
            case Tag.first_person:
                return "First Person";
            case Tag.thrid_person:
                return "Third Person";
            case Tag.cash_grab:
                return "Cash Grab";
            case Tag.official:
                return "Official";
            case Tag.coop:
                return "Co-Op";
            case Tag.online:
                return "Online";
            //Status         
            case Tag.Released:
                return "Released";
            case Tag.In_Development:
                return "In Development";
            //Engine
            case Tag.customORnone:
                return "Custom/None";
            case Tag.flash:
                return "Flash";
            case Tag.unity:
                return "Unity";
            case Tag.ue:
                return "UE";
            case Tag.godot:
                return "Godot";
            case Tag.gamemaker:
                return "Gamemaker";
            case Tag.construct:
                return "Construct";
            case Tag.rpgmaker:
                return "RPGMaker";
            case Tag.RenPy:
                return "Ren'Py";
            case Tag.blender:
                return "Blender";
            case Tag.roblox:
                return "Roblox";
            case Tag.html5:
                return "HTML5";
            case Tag.Java:
                return "Java";
            case Tag.Adventure_Game_Studio:
                return "Adventure Game Studio";
            case Tag.FM2K:
                return "FM2K";
            case Tag.Z_Engine:
                return "Z Engine";
            case Tag.LoveEngine:
                return "LÖVE";
            case Tag.ClickteamFusion:
                return "Clickteam Fusion";
            case Tag.GDevelop:
                return "GDevelop";

            // Character
            case Tag.M6:
                return "M6";
            case Tag.CMC:
                return "CMC";
            case Tag.Secondary_cast:
                return "Secondary Cast";
            case Tag.Background:
                return "Background";
            case Tag.OC:
                return "OC";
            //Playtime  
            case Tag.LESS_5_min:
                return "<5 min";
            case Tag.FROM_5m_TO_30m:
                return "5m-30m";
            case Tag.FROM_30m_TO_2h:
                return "30m-2h";
            case Tag.FROM_2h_TO_4h:
                return "2h-4h";
            case Tag.FROM_4h_TO_10h:
                return "4h-10h";
            case Tag.MORE_10h:
                return "10h+";
            //platforms
            case Tag.Platform_Web:
                return "Web";
            case Tag.Platform_Windows:
                return "Windows";
            case Tag.Platform_OSX:
                return "OSX";
            case Tag.Platform_WindowsX32:
                return "Windows";
            case Tag.Platform_Android:
                return "Android";
            case Tag.Platform_Linux:
                return "Linux";
            case Tag.Platform_Ios:
                return "IOS";
            case Tag.Platform_Ps4:
                return "Playstation 4";
            case Tag.Platform_Xboxone:
                return "XBox One";
            case Tag.Platform_Switch:
                return "Nintendo Switch";
            case Tag.Platform_SNES:
                return "SNES";
            case Tag.Platform_NintendoDS:
                return "Nintendo DS";
            case Tag.Platform_GBA:
                return "GBA";
            case Tag.Platform_GameBoy:
                return "Game Boy";
            case Tag.Platform_Roblox:
                return "Roblox";
            case Tag.Platform_Source_Code:
                return "Source code";
            case Tag.Platform_VR:
                return "VR";
            case Tag.Platform_Flash:
                return "Flash";

            case Tag.NSFW:
                return "NSFW";
            case Tag.Gore:
                return "Gore";

            case Tag.Anthro:
                return "Anthro";
            case Tag.Human:
                return "Human";
            case Tag.Pony_Inspired:
                return "Pony Inspired";

            default:
                throw new ArgumentOutOfRangeException(nameof(tag), tag, null);
        }
    }

    public static string FormatTimeStamp(TimeStamp stamp) {
        switch (stamp.Format) {
            case TimeStamp.DateFormat.YearOnly:
                return stamp.DateTime.ToString("yyyy");
            case TimeStamp.DateFormat.ddMMyyyy:
                return stamp.DateTime.ToString("yyyy-MM-dd");   
            case TimeStamp.DateFormat.NoData:       
                return "";
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static SerializedGamesCollectionData FormatGames(Config cfg, GameData[] games) {
        var formatedGames = new GameSerializedData[games.Length];

        for (int i = 0; i < games.Length; i++) {
            var game = games[i];
            formatedGames[i] = FormatGame(game);
        }

        return new SerializedGamesCollectionData {
            games = formatedGames,
        };
    }

    private static GameSerializedData FormatGame(GameData game) {

        var releaseTimeStamp = game.GetReleaseData();
        var dateAddedTimeStamp = game.GetFirstAddedData();
        
        var releaseDate = FormatTimeStamp(releaseTimeStamp);
        var dateAdded = FormatTimeStamp(dateAddedTimeStamp);
        
        var gameData = new GameSerializedData {
            id = (int)game.Id,
            title = game.Name,
            author = game.Authors,
            thumbnail = GetScreenshotPath(game.HeroScreenshot),
            shortDescription = FormatShortDescription(game.Description),
            fullDescription = game.Description,
            tags = FormatTags(game.Tags),
            characters = FormatCharacters(game.Tags),
            playtime = FormatPlaytime(game.playtime),
            status = FormatStatus(game),
            engine = FormatEngine(game),
            platforms = FormatPlatforms(game),
            url = game.SourceURL,
            releaseDate = releaseDate,
            dateAdded = dateAdded,
            downloads = FormatDownloads(game),
            screenshots = GetScreenshotsPaths(game.OtherScreenshots)
        };

        return gameData;
    }

    private static SerializedDownloadData[] FormatDownloads(GameData game) {
        if (game.Builds == null || game.Builds.Count == 0) {
            ConsoleDrawer.DrawError($"no builds for {game.Id}");
            return null;
        }

        var result = game.Builds
            .Where(build => build != null)
            .SelectMany(build => {
                var list = new List<SerializedDownloadData>();

                list.Add(new SerializedDownloadData {
                        label = FormatTag(build.platform),
                        url = GetMainDownloadURL(build.Id),
                        version = build.Version,
                        buttonText = BuildLabelDownloadOption,
                        mirrors = FormatMirrors(build)
                    }
                );

                if (build.platform == Tag.Platform_Flash) {
                    list.Add(new SerializedDownloadData {
                            label = FormatTag(Tag.Platform_Flash),
                            url = GetFlashEmulatorURL(build.Id),
                            version = build.Version,
                            buttonText = BuildLabelPlayOnOurEmulatorOption,
                            mirrors = []
                        }
                    );
                }

                return list;
            })
            .ToArray();

        if (result.Length == 0)
            return null;

        return result;
    }

    private static BuildMirrorData[] FormatMirrors(GameBuild build) {
        
        var result = build.servers
            .Where(server => {
                if (string.IsNullOrEmpty(server.url))
                    return false;

                if (server.type == MainServer)
                    return false;

                return true;
            })
            .Select(server => new BuildMirrorData {
                label = GetServerLabelText(server.type),
                url = server.url
            });


        return result.ToArray();
    }

    private static string[] FormatPlatforms(GameData game) {
        if (game.Builds == null || game.Builds.Count == 0) {
            ConsoleDrawer.DrawError($"no builds for {game.Id}");
            return [];
        }

        return game.Builds
            .Where(build => build != null)
            .Select(build => build.platform)
            .Select(FormatTag)
            .Distinct()
            .ToArray();
    }
    
    
    private static string FormatShortDescription(string description) {
        const int maxCharacters = 50;
        
        if (description.Length <= maxCharacters)
            return description;
        
        return description.Substring(0, maxCharacters) + "...";
    }

    private static string FormatEngine(GameData game) {
        if (game.Tags.Tags.TryGetValue(Label.Engine, out var engineTags)) {
            var engine = engineTags[0];
            return FormatTag(engine);
        }

        //ConsoleDrawer.DrawError($"unknown engine: for {game.Id}");
        return "Unknown";
    }

    private static string FormatStatus(GameData game) {
        if (game.Tags.Tags.TryGetValue(Label.Status, out var values) == false)
            return FormatTag(Tag.Released);

        var value = values[0];
        var status = FormatTag(value);
        return status;
    }

    private static string[] FormatCharacters(TagsCollection collection) {
        if (collection.Tags.TryGetValue(Label.Character, out var tags) == false)
            return ["Unknown"];

        var characters = tags
            .Select(FormatTag)
            .ToArray();

        return characters;
    }

    private static string FormatPlaytime(Tag tag) {
        if (tag == Tag.None)
            return "Unknown";

        var playtime = FormatTag(tag);
        return playtime;
    }

    private static string[] GetScreenshotsPaths(List<Screenshot>? screenshots) {
        if (screenshots == null) {
            return [];
        }

        var screenshotsPaths = new string[screenshots.Count];
        var index = 0;

        foreach (var screenshot in screenshots) {
            screenshotsPaths[index] = GetScreenshotPath(screenshot);
            index++;
        }

        return screenshotsPaths;
    }

    private static string GetScreenshotPath(Screenshot? screenshot) {
        if (screenshot == null) {
            _currentPlaceholderScreenshotIndex =
                (_currentPlaceholderScreenshotIndex + 1) % ScreenshotsPlaceholders.Length;

            return ScreenshotsPlaceholders[_currentPlaceholderScreenshotIndex];
        }

        var pathToScreenshot = $"{ScreenshotsFolderLocalPath}{screenshot.BucketId}/{screenshot.Id}.{screenshot.format}";

        return pathToScreenshot;
    }

    private static SerializedTagData[] FormatTags(TagsCollection collection) {
        var labelsForUse = new[] {
            Label.Genre,
            Label.Tags,
            Label.AgeRestriction,
            Label.BlackTags,
        };

        var tagsForUse = collection.Tags
            .Where(keypair => labelsForUse.Contains(keypair.Key))
            .ToDictionary();


        var formattedTags = new List<SerializedTagData>();

        foreach (var keyPair in tagsForUse) {
            foreach (var tag in keyPair.Value) {
                if (tag == Tag.SFW)
                    continue;

                formattedTags.Add(new SerializedTagData {
                    label = FormatTag(tag),
                    type = GetLabelText(keyPair.Key),
                });
            }
        }

        return formattedTags.ToArray();
    }
}
