using System.Text.Json;
using GXFormat;

var baseDir = AppDomain.CurrentDomain.BaseDirectory.ToString();
var outputDir = Path.Combine(baseDir, "Output");
var resourcesDir = Path.Combine(baseDir, "../../../BaseDecks");

#if DEBUG
var date = "2099.01.01";
#else
var date = DateTime.Now.ToString("yyyy.MM.dd");
#endif

var dataDir = Path.Combine(baseDir, "../../../../GXFormat.Website/wwwroot/data");
if (!Directory.Exists(dataDir))
    throw new Exception("Directory does not exist!!!");

if (!Directory.Exists(outputDir))
    Directory.CreateDirectory(outputDir);

// Generate the banlist

Console.WriteLine("Generating edopro banlist file...");

var basePool = new GameFormat();

var baseLflistFilePath = Path.Combine(Path.Combine(resourcesDir, "../"), "GXFormat_Base.lflist.conf");
basePool.Load(baseLflistFilePath, true);

var limitPool = new GameFormat();
var archtypeLists = new Dictionary<string, GameFormat>();

// Load Limits
foreach (var fileList in Directory.GetFiles(resourcesDir))
{
    var listName = Path.GetFileNameWithoutExtension(fileList);

    if (listName.StartsWith("GXFormat_"))
    {
        listName = listName.Substring(9);

        if (int.TryParse(listName, out int limit))
        {
            basePool.LoadExtraCards(limit, fileList);
            limitPool.LoadExtraCards(limit, fileList);
        }
    }
}

// Load Archtypes
foreach (var fileList in Directory.GetFiles(resourcesDir))
{
    var listName = Path.GetFileNameWithoutExtension(fileList);

    if (listName.StartsWith("GXFormat_"))
    {
        listName = listName.Substring(9);

        if (!int.TryParse(listName, out int limit))
        {
            var archtypeList = new GameFormat();
            archtypeList.LoadExtraCards(3, fileList);
            archtypeLists[listName] = archtypeList;

            basePool.LoadExtraCards(0, fileList);
        }
    }
}

// Remove banned cards
var hashSet = new HashSet<long>();
foreach (var card in basePool.CardPool)
    if (card.Value == 0 || card.Value == -1)
        hashSet.Add(card.Key);
foreach (var card in hashSet)
    basePool.RemoveCard(card);

foreach (var apair in archtypeLists)
{
    foreach (var bpair in basePool.CardPool)
    {
        apair.Value.AddCard(bpair.Key, bpair.Value);
    }

    foreach (var lpair in limitPool.CardPool)
    {
        if (apair.Value.CardPool.ContainsKey(lpair.Key))
        {
            if (lpair.Value > 0)
                apair.Value.AddCard(lpair.Key, lpair.Value);
            else
                apair.Value.RemoveCard(lpair.Key);
        }
    }

    var archtypeListName = $"GX Format {apair.Key} {date}";
    apair.Value.Save(archtypeListName, Path.Combine(outputDir, archtypeListName + ".lflist.conf"));
}

/*
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
*/