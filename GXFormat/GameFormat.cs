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

    public void Delete(string filePath)
    {
        var fileContents = File.ReadAllText(filePath);
        DeleteData(fileContents);
    }

    public void DeleteData(string fileContents)
    {
        foreach (var match in LflistRegex.Matches(fileContents).Cast<Match>())
        {
            if (long.TryParse(match.Groups["cardId"].ToString(), out var cardId) &&
                int.TryParse(match.Groups["cardLimit"].ToString(), out var limit))
            {
                RemoveCard(cardId);
            }
        }
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
                if (baseData)
                {
                    if (limit == -1)
                    {
                        RemoveCard(cardId);
                    }
                    else
                    {
                        AddCard(cardId);
                    }
                }
                else
                {
                    switch (limit)
                    {
                        case -1:
                            RemoveCard(cardId);
                            break;
                        case 0:
                            AddCard(cardId, 3);
                            break;
                        case 3:
                            AddCard(cardId);
                            break;
                        default:
                            AddCard(cardId, limit);
                            break;
                    }
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
            var limit = cardPair.Value;

            if (limit == 3)
                limit = 0;
            else if (limit == -1)
                limit = 3;
            
            outputLines.Add($"{cardPair.Key} {limit}");
        }

        File.WriteAllLines(filePath, outputLines);
    }

    public void AddCard(long cardId)
        => _cardPool[cardId] = -1;

    public void RemoveCard(long cardId)
        => _cardPool.Remove(cardId);

    public void AddCard(long cardId, int limit)
    {
        if (limit == 0)
            RemoveCard(cardId);
        else
            _cardPool[cardId] = limit;
    }
}

