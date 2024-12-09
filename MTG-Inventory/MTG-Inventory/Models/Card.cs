using CsvHelper.Configuration.Attributes;

namespace MTG_Inventory.Models;

public class Card
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? ImageUri { get; set; }
    public string? Price { get; set; }
    public double? CMC { get; set; }
    public string? TypeLine { get; set; } // Legendary Creature, Creature, Instant, Sorcery, Enchantment, Artifact, Planeswalker, Land
    public string? ColorIdentity { get; set; } // W, U, B, R, G, C, M
    public string? OracleId { get; set; } // Unique identifier for the card in the Scryfall Db
    public bool? IsCommander { get; set; }
    public string? Rarity { get; set; }
    public int Quantity { get; set; }
    public int? CardNumber { get; set; }
    [Name("Expansion Code")] public string? ExpansionCode { get; set; }
    [Name("Expansion Name")] public string? ExpansionName { get; set; }
    public bool? Foil { get; set; }
    public string? Condition { get; set; }
    public string? Language { get; set; }
    public int InUse { get; set; }
    
    public List<CardDeck> CardDecks { get; set; } = [];
}

public class FilteredCard
{
    public string Name { get; set; }
    public int Quantity { get; set; }
}

public class WantListResponse
{
    public int CardsFound => FoundCards.Count;
    public List<FilteredCard> FoundCards { get; set; } = [];
    public int CardsMissing => MissingCards.Count;
    public List<FilteredCard> MissingCards { get; set; } = [];
}