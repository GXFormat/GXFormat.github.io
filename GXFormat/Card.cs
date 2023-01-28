
namespace GXFormat;

public struct Card
{
    public long Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public int Atk { get; set; }

    public int Def { get; set; }

    public CardCategory Category { get; set; }

    public CardAttribute Attribute { get; set; }

    public CardType Type { get; set; }

    public int Level { get; set; }

    public string AtkString => Category.HasFlag(CardCategory.Monster) ? (Atk >= 0 ? $"{Atk}" : "?") : "N/A";

    public string DefString => Category.HasFlag(CardCategory.Monster) ? (Def >= 0 ? $"{Def}" : "?") : "N/A";

    public string AttributeString => Category.HasFlag(CardCategory.Monster) ? $"{Attribute}" : "N/A";

    public string TypeString => Category.HasFlag(CardCategory.Monster) ? $"{Type}" : "N/A";

    public string LevelString => Category.HasFlag(CardCategory.Monster) ? $"{Level}" : "N/A";

    public override bool Equals(object? obj) => obj is Card card && card.Id == Id;

    public override int GetHashCode() => (int)Id;

    public static bool operator ==(Card lhs, Card rhs) => lhs.Equals(rhs);

    public static bool operator !=(Card lhs, Card rhs) => !lhs.Equals(rhs);
}