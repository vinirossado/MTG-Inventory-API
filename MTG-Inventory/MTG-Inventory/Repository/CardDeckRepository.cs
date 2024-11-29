
using MTG_Inventory.Models;

namespace MTG_Inventory.Repository;

public class CardDeckRepository (AppDbContext context)
{
    public async Task AddCardsToDeck(List<CardDeck> cards)
    {
        await context.CardDeck.AddRangeAsync(cards);
        await context.SaveChangesAsync();
    }
}