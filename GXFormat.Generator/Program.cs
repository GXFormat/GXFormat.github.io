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

var formatName = $"GX Format {DateTime.Now.ToString("yyyy.MM.dd")}";
var formatNameFileName = $"GXFormat_{DateTime.Now.ToString("yyyy.MM.dd")}";
var outputPath = Path.Combine(outDir, $"{formatNameFileName}.lflist.conf");
gameFormat.Save(formatName, outputPath);

// Save all the information about the banlist for the website

Console.WriteLine("Generating carddatabase json...");

var dataDir = Path.Combine(baseDir, "../../../../GXFormat.Website/wwwroot/data");
if (!Directory.Exists(dataDir))
    throw new Exception("Directory does not exist!!!");

var lflistFiles = Directory.GetFiles(dataDir, "*.lflist.conf");
var cardDatabaseGameFormat = new GameFormat();
cardDatabaseGameFormat.Load(baseLflistFilePath, true);
foreach (var lflistFile in lflistFiles)
    cardDatabaseGameFormat.Load(lflistFile, true);

var cardDatabasePaths = Directory.GetFiles("../../../../ThirdParty/DeltaUtopia/", "*.cdb");
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

File.Copy(outputPath, Path.Combine(dataDir, "GXFormat_2099.01.01.lflist.conf"), true);

var formats = new List<string>();
foreach (var filePath in lflistFiles)
    formats.Add(Path.GetFileName(filePath).Replace(".lflist.conf", ""));
File.WriteAllText(Path.Combine(dataDir, "formats.json"), JsonSerializer.Serialize(formats));
