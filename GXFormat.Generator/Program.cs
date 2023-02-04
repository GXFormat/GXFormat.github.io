using System.Text.Json;
using GXFormat;

// Clear existing output
var baseDir = AppDomain.CurrentDomain.BaseDirectory.ToString();
var outDir = Path.Combine(baseDir, "data");

if (Directory.Exists(outDir))
    Directory.Delete(outDir, true);
Directory.CreateDirectory(outDir);

// Generate the banlist

Console.WriteLine("Generating edopro banlist file...");

var resourcesDir = Path.Combine(baseDir, "Resources");
var gameFormat = new GameFormat();

var baseLflistFilePath = Path.Combine(resourcesDir, "GXFormat_Base.lflist.conf");
gameFormat.Load(baseLflistFilePath, true);

for (int i = -1; i <= 3; i++)
{
    var ydkFilePath = Path.Combine(resourcesDir, $"GXFormat_Limit{i}.ydk");
    gameFormat.LoadExtraCards(i, ydkFilePath);
}

#if DEBUG
var date = "2099.01.01";
#else
var date = DateTime.Now.ToString("yyyy.MM.dd");
#endif

var formatName = $"GX Format {date}";
var formatNameFileName = $"GXFormat_{date}";
var outputPath = Path.Combine(outDir, $"{formatNameFileName}.lflist.conf");
gameFormat.Save(formatName, outputPath);

var dataDir = Path.Combine(baseDir, "../../../../GXFormat.Website/wwwroot/data");
if (!Directory.Exists(dataDir))
    throw new Exception("Directory does not exist!!!");

File.Copy(outputPath, Path.Combine(dataDir, Path.GetFileName(outputPath)), true);

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

var outputMetadataPath = Path.Combine(outDir, $"carddatabase.json");
var outputMetadataData = JsonSerializer.Serialize(cards, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true });

File.WriteAllText(outputMetadataPath, outputMetadataData);
File.Copy(outputMetadataPath, Path.Combine(dataDir, "carddatabase.json"), true);

// Regenerate the formats file as its easier to do it from here

Console.WriteLine("Generating formats json...");

var formats = new List<string>();
foreach (var filePath in lflistFiles)
    formats.Add(Path.GetFileName(filePath).Replace(".lflist.conf", ""));
formats.Sort((s1, s2) => string.Compare(s2, s1, StringComparison.Ordinal));
File.WriteAllText(Path.Combine(dataDir, "formats.json"), JsonSerializer.Serialize(formats));
