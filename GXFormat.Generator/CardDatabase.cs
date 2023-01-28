using Microsoft.Data.Sqlite;

namespace GXFormat;

public class CardDatabase
{
    private string[] _databaseFilePaths;
    private List<SqliteConnection> _sqliteConnections;

    public CardDatabase(string[] databaseFilePaths)
	{
        _databaseFilePaths = databaseFilePaths;
        _sqliteConnections = new List<SqliteConnection>();

        foreach (var filePath in databaseFilePaths)
        {
            var connection = new SqliteConnection("DataSource=" + filePath);
            connection.Open();

            _sqliteConnections.Add(connection);
        }
    }

    public Dictionary<long, Card> GetCards(GameFormat gameFormat)
    {
        var ret = new Dictionary<long, Card>();

        foreach (var (cardId, cardLimit) in gameFormat.CardPool)
            ret[cardId] = GetCard(cardId);

        return ret;
    }

    public Card GetCard(long id, bool outputCdb = false)
    {
        var card = new Card();
        card.Id = id;

        for (int i = 0; i < _sqliteConnections.Count; i++)
        {
            SqliteConnection? connection = _sqliteConnections[i];

            var commandTexts = connection.CreateCommand();
            commandTexts.CommandText = @"SELECT name, desc FROM texts WHERE id = $id";
            commandTexts.Parameters.AddWithValue("$id", id);
            using var readerTexts = commandTexts.ExecuteReader();

            if (readerTexts.Read())
            {
                card.Title = readerTexts.GetString(0);
                card.Description = readerTexts.GetString(1);
            }

            var commandDatas = connection.CreateCommand();
            commandDatas.CommandText = @"SELECT type, atk, def, level, race, attribute FROM datas WHERE id = $id";
            commandDatas.Parameters.AddWithValue("$id", id);
            using var readerDatas = commandDatas.ExecuteReader();

            if (readerDatas.Read())
            {
                card.Category = (CardCategory)readerDatas.GetInt32(0);
                card.Atk = readerDatas.GetInt32(1);
                card.Def = readerDatas.GetInt32(2);
                card.Level = readerDatas.GetInt32(3) & 0xFFFF; // first 4 byes are pend scales (2 left, 2 right), next 4 bytes are the level
                card.Type = (CardType)readerDatas.GetInt32(4);
                card.Attribute = (CardAttribute)readerDatas.GetInt32(5);
            }
        }

        return card;
    }
}

