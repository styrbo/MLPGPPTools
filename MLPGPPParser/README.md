# MLPGPPParser — Command Reference

A small .NET 9 console tool (Windows) used to:

- compute and check MD5 "hash files" for game folders (`hash` task),
- build the games JSON database from a Google Sheet (`parser` task).

This document describes **everything the current code does**, including its quirks. Bugs found in the current code are listed in the [Bugs](#bugs) section.

---

## Contents

1. [Requirements](#requirements)
2. [Building and running](#building-and-running)
3. [Command input modes](#command-input-modes)
4. [Quick reference](#quick-reference)
5. [Task: `hash`](#task-hash)
6. [Task: `parser`](#task-parser)
7. [Hash file format](#hash-file-format)
8. [Config file](#config-file)
9. [Hard-coded paths](#hard-coded-paths)
10. [Logging](#logging)
11. [Inactive code (not reachable from the CLI)](#inactive-code-not-reachable-from-the-cli)
12. [Bugs](#bugs)
13. [Quoting tips for Windows](#quoting-tips-for-windows)
14. [Cheat sheet](#cheat-sheet)

---

## Requirements

- Windows (paths are Windows-style, e.g. `O:\MLGPP\g\`).
- .NET 9 SDK (`net9.0`). Verified with SDK `9.0.303`.
- NuGet package: `Google.Apis.Sheets.v4` 1.75.0.4178 (restored automatically).
- For the `parser` task only: a Google Sheets API key and a sheet ID in the config file.

---

## Building and running

Project file: `MLPGPPParser.csproj` (solution: `MLPGPPParser.sln`).

Build:

```powershell
dotnet build MLPGPPParser.csproj
```

Run through `dotnet run` (note the `--` separator; everything after it goes to the program):

```powershell
dotnet run --project MLPGPPParser.csproj -- run hash calc
```

Run the built output directly (after a build):

```powershell
dotnet bin\Debug\net9.0\MLPGPPParser.dll run hash calc
```

Run a single command in interactive mode from a pipe (useful for testing):

```powershell
echo hash calc | dotnet bin\Debug\net9.0\MLPGPPParser.dll
```

> The program always exits with code `0`, even when a command fails. Check the log file or console output for errors.

---

## Command input modes

The program has two ways to receive a command. The first word is the **task name**; the rest are that task's arguments.

### 1. Direct mode (`run`)

```text
MLPGPPParser run <task> [arguments...]
```

Example: `MLPGPPParser run hash calc base -f`

### 2. Interactive mode (no `run`)

If the first argument is not `run` (or there are no arguments at all), the program prints `Waiting for the command`, reads **one line** from stdin, splits it on spaces and runs it. It then exits. It does **not** loop.

```text
> MLPGPPParser
Waiting for the command
hash calc base -f
```

> Interactive mode splits on spaces only, so paths containing spaces will not work there. Use direct mode for those.

### Dispatch rules

- The first argument after `run` (or the first word of the line) is matched against the registered task names: `hash`, `parser`.
- If no task matches, **nothing is printed** and the program exits with code `0`.
- The task receives the remaining words as its arguments.

---

## Quick reference

| Command | What it does | Status |
|---|---|---|
| `hash calc [path\|base] [-f]` | Create `.<id>.in.hash` files for numbered game folders | Working (see [Bugs](#bugs)) |
| `hash find <file-or-folder>` | Compare files against all stored hash files | Working (uses fixed games folder) |
| `hash coll` | Report game folders whose stored hashes are identical | Working (uses fixed games folder) |
| `hash find_all` | Placeholder, does nothing | Stub |
| `parser` | Read Google Sheet, build games JSON database | Working (needs valid config) |
| `help` | Would show help | **Not registered**, does nothing |
| `folders` | Creates 10 000 empty folders in a hard-coded path | **Not registered**, cannot be run |

---

## Task: `hash`

Entry: `run hash <mode> ...`

If no mode is given, prints `error: no mode selected` and stops.

Any mode name not listed below (for example `hash foo`) is silently ignored.

### `hash calc [path|base] [-f]`

Creates a hash file inside every numbered game folder in the games folder.

| Part | Meaning |
|---|---|
| `path` | Games folder to scan. Omit it, or write `base`, to use the default `O:\MLGPP\g\`. |
| `-f` | Force recalculation: delete existing `.<id>.in.hash` files and create them again. **Must be the 3rd word** (after `path`/`base`). |

Examples:

```text
run hash calc                      -> default folder, skip existing hashes
run hash calc base -f              -> default folder, recalculate everything
run hash calc "O:\MLGPP\other"     -> custom folder, skip existing hashes
run hash calc "O:\MLGPP\other" -f  -> custom folder, recalculate everything
```

How it works:

1. Lists the immediate subfolders of the games folder whose names are unsigned integers (`11`, `12`, ...). Other folders are ignored.
2. Works out the lowest and highest id.
3. For each id in that range, in parallel:
   - If `<id>\.<id>.in.hash` already exists:
     - with `-f`: delete it and recalculate;
     - without `-f`: print a warning and skip.
   - Lists the **top-level files** of `<id>\` (subfolders are not searched).
   - If there are none: print a warning and skip.
   - Otherwise computes the **MD5 of the first file** in that list and writes the hash file.

Output lines: `Hashed File: <id> name <filename>` for each new hash.

> **Bugs in `calc`** (verified by running it against test folders, details in [Bugs](#bugs)):
> - The **highest-numbered folder is never hashed**. The range end is exclusive (`Parallel.ForAsync(games[0], games[^1])`). With folders `11, 12, 13` only `11` and `12` are hashed.
> - A **gap in numbering crashes the command**. With folders `11, 13, 14`, id `12` is in the range but has no folder, so the run throws `DirectoryNotFoundException`.
> - `hash calc -f` **does not work as expected**. `-f` is read as the path (`Could not find a part of the path '...\-f'`). Use `hash calc base -f`.
> - "First file" is whatever `Directory.GetFiles` returns first. Its order is not guaranteed by the API. If a game folder has several top-level files, the stored hash may be for a different file than you expect.

### `hash find <file-or-folder>`

Checks one file, or every file under a folder (recursively), against all stored hash files.

- Required argument. Without it the program crashes with an unhandled `ArgumentException` (see [Bugs](#bugs)).
- Loads every `*.hash` file found one level below the fixed games folder `O:\MLGPP\g\` (see [Hard-coded paths](#hard-coded-paths)).
- For each file, computes its full MD5 and compares it with each stored hash.
- Prints the list of matches at the end:
  - `✅ <file> (<md5>) == <hash file path> (<md5>)` if a stored hash matches (the first match is used);
  - `❌ <file> (<md5>)` if none matches (logged as an error).
- Files are sorted by full path before checking.

Examples:

```text
run hash find "C:\Downloads\some_game.exe"
run hash find "C:\Downloads\unpacked_game"
```

### `hash coll`

Looks for **collisions between game folders**: two different stored hash files with the same MD5.

- Uses the fixed games folder `O:\MLGPP\g\`.
- Prints `Collision found between <pathA> and <pathB>` for each pair, then a summary.
- If none is found, prints `No collisions found!` as an error line.

### `hash find_all`

Recognised but empty. It does nothing and prints nothing.

---

## Task: `parser`

Entry: `run parser`

Arguments are ignored.

Steps:

1. Reads the config from `<current directory>\Resources\config.json`.
   - **Important:** it uses the *current working directory*, not the location of the `.exe`. Run it from the folder that contains `Resources\`.
   - If the file is missing, a template config is created at that path and the command stops with an error. Fill it in and run again.
   - If the file contains a JSON `null`, the template is written over it and the command stops with an error. If the file is malformed JSON, the command crashes with an unhandled exception and the file is left alone. Back the file up before editing.
2. Loads the screenshots database from `localScreenshotsDBPath`.
3. Downloads the sheet range `dataRange` from `sheetID` using `APIToken` as a Google API key.
4. Parses the rows into games, formats them, and writes JSON to `gamesJsonPath` (indented).

Screenshot moving into `siteLocalPath` is commented out in the code and does not run.

Output: prints `parsed <N> games`, `Generating Json`, `Saving Json`.

---

## Hash file format

Each game folder can contain one hash file:

- **Location:** `<games folder>\<id>\.<id>.in.hash`, e.g. `O:\MLGPP\g\12\.12.in.hash`
- **Encoding:** UTF-8 text (written by `StreamWriter`)
- **Content:**

  ```text
  Memory Leak Slopity Check! 67 /)
  MD5 <32-character uppercase hex>
  ```

- The first line is a fixed marker. A file without it is treated as invalid and ignored.
- The line after it is `<ALGORITHM> <hex value>`. Only `MD5` is written today.

Folder `<id>` is the number in the folder name. The hash file name always uses the same id.

---

## Config file

Path: `<current directory>\Resources\config.json`

Created automatically with placeholder values by the `parser` task if missing. Shape (field names are matched case-insensitively):

```json
{
  "APIToken": "idk",
  "sheetID": "idk",
  "dataRange": "source!A3:X",
  "localScreenshotsDBPath": "O:/MLGPP/s",
  "gamesJsonPath": "O:/MLGPP/Pony.Games//data/games.json",
  "siteLocalPath": "O:\\MLGPP\\Pony.Games",
  "logFolderPath": "O:\\MLGPP\\logs"
}
```

| Field | Used by | Purpose |
|---|---|---|
| `APIToken` | `parser` | Google Sheets API key |
| `sheetID` | `parser` | Spreadsheet ID |
| `dataRange` | `parser` | Range to read, e.g. `source!A3:X` |
| `localScreenshotsDBPath` | `parser` | Screenshots database folder |
| `gamesJsonPath` | `parser` | Output JSON file |
| `siteLocalPath` | none (screenshot moving is disabled) | Target folder for screenshots |
| `logFolderPath` | **none** (see [Bugs](#bugs)) | Documented intent: log folder |

---

## Hard-coded paths

| Path | Used by | Notes |
|---|---|---|
| `O:\MLGPP\g\` | `hash calc` default, `hash find`, `hash coll` | Games folder. `calc` can be pointed elsewhere; `find` and `coll` cannot. |
| `O:\MLGPP\logs` | Logger (all runs) | Log folder. Config `logFolderPath` is ignored. |
| `<cwd>\Resources\config.json` | `parser` | Config file |
| `O:\MLGPP\Pony.Games\s` | `folders` (inactive) | Target of the unused folder generator |

---

## Logging

- Every run creates a new log file: `O:\MLGPP\logs\yyyy-MM-dd_HH-mm-ss.log`.
- Messages are written to the console and to the log file. Lines in the file get a timestamp prefix, except the progress lines (`Executing task for ...`), which are written without one.
- Colours on the console: green = info, yellow = warning, red = error.

---

## Inactive code (not reachable from the CLI)

These exist in the source but are not usable from the command line:

- **`help`** (`HelpRunner`): not in the runner list, and its `Run` body is empty. Typing `help` does nothing.
- **`folders`** (`FoldersManagerRunner`): not in the runner list. It creates 10 000 empty folders `0`..`9999` in `O:\MLGPP\Pony.Games\s`. It is a one-off helper and is commented out in `Program.cs`.

To make either one reachable, add it to the `runners` list in `Program.cs`.

---

## Bugs

Each bug states how it was found:

- **Verified**: reproduced by running the built program against throwaway test folders (not the real games folder).
- **Code reading**: found in the source, not reproduced by running.

### Bug 1: `hash calc` never hashes the highest-numbered folder *(verified)*

- **Repro:** folders `11`, `12`, `13`, each with one file. Run `hash calc "<path>"`.
- **Actual:** hash files are created for `11` and `12` only. `13` gets none.
- **Cause:** `Parallel.ForAsync(games[0], games[^1], ...)` treats the upper bound as exclusive (confirmed in the framework XML docs: `toExclusive`).
- **Expected:** all three folders are hashed.

### Bug 2: `hash calc` crashes when the numbering has a gap *(verified)*

- **Repro:** folders `11`, `13`, `14`. Run `hash calc "<path>"`.
- **Actual:** id `12` is inside the range but has no folder. `Directory.GetFiles` on the missing folder throws, and the run ends with an unhandled exception.
- **Expected:** missing ids are skipped.

### Bug 3: `hash calc -f` does not recognise the flag *(verified)*

- **Repro:** `hash calc -f`.
- **Actual:** `-f` is taken as the games folder path, giving `Could not find a part of the path '...\-f'`.
- **Expected:** `-f` is the force flag. Workaround: `hash calc base -f`.

### Bug 4: `hash find` without an argument crashes *(code reading)*

- **Repro:** `run hash find`.
- **Actual:** unhandled `ArgumentException` ("no file to check").
- **Expected:** a usage error message.

### Bug 5: `hash find` and `hash coll` ignore any path *(code reading)*

- **Repro:** change the games folder (e.g. to `D:\games`) and run `hash find` or `hash coll`.
- **Actual:** both always read `O:\MLGPP\g\`.
- **Expected:** they use the same games folder as `hash calc`.

### Bug 6: no task name crashes the program *(verified)*

- **Repro:** `run` on its own, or an empty line in interactive mode.
- **Actual:** unhandled `IndexOutOfRangeException`.
- **Expected:** a usage message.

### Bug 7: end of input in interactive mode crashes *(code reading)*

- **Repro:** interactive mode with stdin closed (no line at all).
- **Actual:** `Console.ReadLine()` returns `null`, and the code dereferences it (`Program.cs`, the `Split` call), causing a null reference exception.
- **Expected:** a clean exit.

### Bug 8: unknown task names are silently ignored *(verified)*

- **Repro:** `nope` in interactive mode.
- **Actual:** prints nothing after `Waiting for the command`.
- **Expected:** an error such as `unknown task 'nope'`.

### Bug 9: the exit code is always `0` *(verified)*

- **Repro:** `echo nope | MLPGPPParser.dll`, then check the exit code.
- **Actual:** exit code `0`, including after errors such as the ones above.
- **Expected:** a non-zero exit code on failure, so scripts can detect it.

### Bug 10: config `logFolderPath` is not used *(verified by search)*

- **Repro:** change `logFolderPath` in `config.json`.
- **Actual:** logs still go to `O:\MLGPP\logs`. `Logger.Initialize` is called with a hard-coded path in `Program.cs`, and nothing reads the config field.
- **Expected:** logs go to the configured folder.

### Bug 11: a `null` config is overwritten, malformed JSON crashes *(code reading)*

- **Repro:** put `null` into `Resources\config.json`, then run `parser`.
- **Actual:** the file is replaced with the placeholder template. Malformed JSON instead crashes the program and leaves the file alone.
- **Expected:** the user's file is not overwritten; both cases report a clear error.

### Bug 12: the stored hash may belong to an unexpected file *(code reading)*

- **Repro:** a game folder with several top-level files.
- **Actual:** the hash is computed for whichever file `Directory.GetFiles` returns first. The API does not guarantee that order.
- **Expected:** a defined file (e.g. the main executable, or sorted by name).

---

## Quoting tips for Windows

- Quote paths that contain spaces: `"O:\MLGPP\other folder"`.
- **Do not end a quoted path with a backslash.** `"O:\MLGPP\g\"` is parsed incorrectly by Windows argument parsing (the `\"` is read as an escaped quote). Write `"O:\MLGPP\g"` instead. The program adds the trailing `\` itself where it needs it.
- In PowerShell, put `--` before arguments when calling `dotnet run`, as shown above.

---

## Cheat sheet

```text
# Hash numbered folders in the default games folder (new hashes only)
run hash calc

# Recalculate every hash in the default games folder
run hash calc base -f

# Hash folders in another games folder
run hash calc "O:\MLGPP\other"

# Recalculate in another games folder
run hash calc "O:\MLGPP\other" -f

# Check one file against all stored hashes
run hash find "C:\path\to\file.exe"

# Check a whole folder (recursive)
run hash find "C:\path\to\folder"

# Find identical game folders
run hash coll

# Build the games JSON (run from the folder that has Resources\config.json)
run parser
```
