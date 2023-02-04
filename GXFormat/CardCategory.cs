
namespace GXFormat;

[Flags]
public enum CardCategory
{
    None = 0x0,
    Monster = 0x1,
    Spell = 0x2,
    Trap = 0x4,
    Normal = 0x10,
    Effect = 0x20,
    Fusion = 0x40,
    Ritual = 0x80,
    TrapMonster = 0x100,
    Spirit = 0x200,
    Union = 0x400,
    Gemini = 0x800,
    Tuner = 0x1000,
    Synchro = 0x2000,
    Token = 0x4000,
    Maxium = 0x8000,
    QuickPlay = 0x10000,
    Continuous = 0x20000,
    Equip = 0x40000,
    Field = 0x80000,
    Counter = 0x100000,
    Flip = 0x200000,
    Toon = 0x400000,
    Xyz = 0x800000,
    Pendulum = 0x1000000,
    SpecialSummon = 0x2000000,
    Link = 0x4000000
}

public static class CardCategoryExtensions
{
    public static bool IsMainDeckMonster(this CardCategory category)
        => category.HasFlag(CardCategory.Monster) && !category.HasFlag(CardCategory.Fusion);

    public static int GetScore(this CardCategory category)
    {
        if (category.IsMainDeckMonster())
            return category.HasFlag(CardCategory.Normal) ? 1 : 2;
        
        if (category.HasFlag(CardCategory.Fusion))
            return 3;

        if (category.HasFlag(CardCategory.Spell))
            return 4;
        
        return 5;
    }
}
