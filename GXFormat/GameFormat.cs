using System.Text.RegularExpressions;

namespace GXFormat;

public class GameFormat
{
    private readonly Regex LflistRegex = new(@"(?<cardId>\d+) (?<cardLimit>(-|)?\d+)(.+)?", RegexOptions.Multiline | RegexOptions.Compiled);
    private readonly Regex YdkRegex = new(@"(?<cardId>\d+)", RegexOptions.Multiline | RegexOptions.Compiled);

    private Dictionary<long, int> _cardPool;

	public GameFormat()
	{
        _cardPool = new Dictionary<long, int>();
    }

    public Dictionary<long, int> CardPool => _cardPool;

    public void Clear()
    {
        _cardPool.Clear();
    }

    public void Load(string filePath, bool baseData = false)
    {
        var fileContents = File.ReadAllText(filePath);
        LoadData(fileContents, baseData);
    }

    public void LoadData(string fileContents, bool baseData = false)
    {
        foreach (var match in LflistRegex.Matches(fileContents).Cast<Match>())
        {
            if (long.TryParse(match.Groups["cardId"].ToString(), out var cardId) &&
                int.TryParse(match.Groups["cardLimit"].ToString(), out var limit))
            {
                if (limit > 0)
                {
                    AddCard(cardId, 3);
                }
            }
        }
    }

    public void LoadExtraCards(int limit, string filePath)
    {
        var fileContents = File.ReadAllText(filePath);

        foreach (var match in YdkRegex.Matches(fileContents).Cast<Match>())
        {
            if (long.TryParse(match.Groups["cardId"].ToString(), out var cardId))
            {
                AddCard(cardId, limit);
            }
        }
    }

    public void Save(string formatName, string filePath)
    {
        var outputLines = new List<string>
        {
            $"#[{formatName}]",
            $"!{formatName}",
            "$whitelist"
        };

        foreach (var cardPair in _cardPool)
        {
            outputLines.Add($"{cardPair.Key} {cardPair.Value}");
        }

        File.WriteAllLines(filePath, outputLines);
    }

    public void RemoveCard(long cardId)
        => _cardPool.Remove(cardId);

    public void AddCard(long cardId, int limit)
    {
        _cardPool[cardId] = limit;
    }
}

