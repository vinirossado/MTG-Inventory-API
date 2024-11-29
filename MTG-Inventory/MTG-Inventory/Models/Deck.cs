namespace MTG_Inventory.Models;

public class Deck
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Strategy { get; set; }
    public short PowerLevel { get; set; }
    
    public List<CardDeck> CardDecks { get; set; } = [];
    
}