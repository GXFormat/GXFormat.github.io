namespace GXFormat.Website;

public static class WebsiteExtensions
{
    private static int Sort(this CardSort sortOrder, Card card1, Card card2)
    {
        var compareResult = sortOrder switch
        {
            CardSort.CategoryAscending => card2.Category.GetScore() - card1.Category.GetScore(),
            CardSort.CategoryDescending => card1.Category.GetScore() - card2.Category.GetScore(),
            CardSort.NameAscending => string.Compare(card2.Title, card1.Title, StringComparison.Ordinal),
            CardSort.NameDescending => string.Compare(card1.Title, card2.Title, StringComparison.Ordinal),
            CardSort.LevelAscending => card1.Level - card2.Level,
            CardSort.LevelDescending => card2.Level - card1.Level,
            CardSort.AtkAscending => card1.Atk - card2.Atk,
            CardSort.AtkDescending => card2.Atk - card1.Atk,
            CardSort.DefAscending => card1.Def - card2.Def,
            CardSort.DefDescending => card2.Def - card1.Def,
            _ => 0
        };

        return compareResult;
    }
    
    public static void SortCards<T>(this List<T> list, CardSort sortOrder, Func<T, Card> getCard)
    {
        list.Sort((s1, s2) =>
        {
            var card1 = getCard(s1);
            var card2 = getCard(s2);
            var sort = sortOrder.Sort(card1, card2);

            if (sort == 0)
                sort = CardSort.CategoryDescending.Sort(card1, card2);

            if (sort == 0)
                sort = CardSort.NameDescending.Sort(card1, card2);
            
            return sort;
        });
    }

    public static long GetYgoProDeckCardID(this Card card) => card.Id switch
    {
        511003055L => 67159705, // Armored Cybern (Pre-Errata)
        65240394L => 65240384, // Big Shield Gardna (Pre-Errata)
        511000228L => 95727991, // Catapult Turtle (Pre-Errata)
        511001039L => 40737112, // Dark Magician of Chaos (Pre-Errata)
        511003116L => 56570271, // Destiny HERO - Disk Commander (Pre-Errata)
        511003006L => 45894482, // Gilasaurus (Pre-Errata)
        511002851L => 23265594, // Heavy Mech Support Platform (Pre-Errata)
        21593987L => 21593977, // Makyura the Destructor (Pre-Errata)
        511002992L => 14878871, // Rescue Cat (Pre-Errata)
        511003007L => 21502796, // Ryko, Lightsworn Hunter (Pre-Errata)
        511002631L => 26202165, // Sangan (Pre-Errata)
        511000818L => 8131171, // Sinister Serpent (Pre-Errata)
        511000868L => 43586926, // Twin-Headed Behemoth (Pre-Errata)
        511003009L => 15894048, // Ultimate Tyranno (Pre-Errata)
        511002901L => 96300057, // W-Wing Catapult (Pre-Errata)
        511003012L => 78010363, // Witch of the Black Forest (Pre-Errata)
        511003020L => 58996430, // Wulf, Lightsworn Beast (Pre-Errata)
        511002852L => 65622692, // Y-Dragon Head (Pre-Errata)
        511002853L => 64500000, // Z-Metal Tank (Pre-Errata)
        511002995L => 87910978, // Brain Control (Pre-Errata)
        511002997L => 77565204, // Future Fusion (Pre-Errata)
        511002998L => 47355498, // Necrovalley (Pre-Errata)
        511003008L => 12923641, // Swords of Concealing Light (Pre-Errata)
        511000824L => 83555667, // Ring of Destruction (Pre-Errata)
        511000825L => 83555667, // Ring of Destruction (Pre-Errata)
        511003022L => 13955608, // Stronghold the Moving Fortress (Pre-Errata)
        _ => card.Id
    };
}

