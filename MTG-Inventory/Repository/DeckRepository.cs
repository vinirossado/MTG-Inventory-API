using Microsoft.EntityFrameworkCore;
using MTG_Inventory.Models;

namespace MTG_Inventory.Repository;

public class DeckRepository (AppDbContext context)
{
    
    public async Task<List<Deck>> GetDecks()
    {
        return await context.Deck.ToListAsync();
    }
    
    public async Task<Deck?> GetDeckById(int id)
    {
        return await context.Deck.FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task UpdateDeck(Deck deck)
    {
        context.Deck.Update(deck);
        await context.SaveChangesAsync();
    }
    
    public async Task AddDeck(Deck deck)
    {
        await context.Deck.AddAsync(deck);
        await context.SaveChangesAsync();
    }
}