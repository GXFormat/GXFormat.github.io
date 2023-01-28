using System.Net.Http.Json;

namespace GXFormat.Website;

public class CardDatabase
{
    private readonly Dictionary<long, Card> _database;
    
    private CardDatabase(Dictionary<long, Card> database)
    {
        _database = database;
    }
    
    public static async Task<CardDatabase> LoadAsync(HttpClient httpClient)
    {
        var database = await httpClient.GetFromJsonAsync<Dictionary<long, Card>?>($"data/carddatabase.json");
        return new CardDatabase(database ?? new Dictionary<long, Card>());
    }

    public Card GetCard(long id)
        => _database.TryGetValue(id, out var card) ? card : new Card() { Id = id};
}
