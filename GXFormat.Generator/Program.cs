using System.Text.Json;
using GXFormat;

var baseDir = AppDomain.CurrentDomain.BaseDirectory.ToString();
var resourcesDir = Path.Combine(baseDir, "../../../Resources");

var dataDir = Path.Combine(baseDir, "../../../../GXFormat.Website/wwwroot/data");
if (!Directory.Exists(dataDir))
    throw new Exception("Directory does not exist!!!");

// Generate the banlist

Console.WriteLine("Generating edopro banlist file...");

var baseLflistFilePath = Path.Combine(resourcesDir, "Whitelist 2008.09 GX.lflist.conf");

var gameFormat = new GameFormat();
gameFormat.Load(baseLflistFilePath, true);
gameFormat.Delete( Path.Combine(resourcesDir, "Whitelist 2005.04 DM.lflist.conf"));

for (int i = -1; i <= 3; i++)
{
    var ydkFilePath = Path.Combine(resourcesDir, $"GXFormat_Limit{i}.ydk");
    gameFormat.LoadExtraCards(i, ydkFilePath);
}

var debugFilePath = Path.Combine(dataDir, $"GXFormat_2099.01.01.lflist.conf");
if (File.Exists(debugFilePath))
    File.Delete(debugFilePath);

#if DEBUG
var date = "2099.01.01";
#else
var date = DateTime.Now.ToString("yyyy.MM.dd");
#endif

var formatName = $"GX Format {date}";
var outputPath = Path.Combine(dataDir, $"GXFormat_{date}.lflist.conf");
gameFormat.Save(formatName, outputPath);

// Save all the information about the banlist for the website

Console.WriteLine("Generating carddatabase json...");

var lflistFiles = Directory.GetFiles(dataDir, "*.lflist.conf");
var cardDatabaseGameFormat = new GameFormat();
cardDatabaseGameFormat.Load(baseLflistFilePath, true);
foreach (var lflistFile in lflistFiles)
    cardDatabaseGameFormat.Load(lflistFile, true);

var cardDatabasePaths = Directory.GetFiles(Path.Combine(baseDir, "../../../../ThirdParty/DeltaUtopia/"), "*.cdb");
var cardDatabase = new CardDatabase(cardDatabasePaths);
var cards = cardDatabase.GetCards(cardDatabaseGameFormat);

foreach (var (cardId, card) in cards)
{
    if (string.IsNullOrEmpty(card.Title))
    {
        Console.WriteLine($"Error: Missing database entry for {cardId}");
    }
}

var outputMetadataPath = Path.Combine(dataDir, $"carddatabase.json");
var outputMetadataData = JsonSerializer.Serialize(cards, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true });

File.WriteAllText(outputMetadataPath, outputMetadataData);

// Regenerate the formats file as its easier to do it from here

Console.WriteLine("Generating formats json...");

var formats = new List<string>();
foreach (var filePath in lflistFiles)
    formats.Add(Path.GetFileName(filePath).Replace(".lflist.conf", ""));
formats.Sort((s1, s2) => string.Compare(s2, s1, StringComparison.Ordinal));
File.WriteAllText(Path.Combine(dataDir, "formats.json"), JsonSerializer.Serialize(formats));
