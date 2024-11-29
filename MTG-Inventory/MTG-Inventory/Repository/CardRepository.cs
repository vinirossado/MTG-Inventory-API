using Microsoft.EntityFrameworkCore;
using MTG_Inventory.Models;

namespace MTG_Inventory.Repository;

public class CardRepository(AppDbContext context)
{
    public async Task Add(IList<Card> cards)
    {
        await context.Card.AddRangeAsync(cards);
        await context.SaveChangesAsync();
    }

    public async Task Add(Card card)
    {
        await context.Card.AddAsync(card);
        await context.SaveChangesAsync();
    }

    public async Task<List<Card>> Get()
    {
        return await context.Card.ToListAsync();
    }

    public async Task<List<Card>> GetMissingSyncCards()
    {
        return await context.Card.Where(x => x.TypeLine == null).ToListAsync();
    }

    public async Task<(IList<Card>, IList<int>)>  GetMissingCards(IList<Card> cards)
    {
        var cardNames = cards.Select(c => c.Name).ToList();

        var existingCardNames = await context.Card
            .Where(dbCard => cardNames.Contains(dbCard.Name))
            .Select(dbCard => dbCard.Name)
            .ToListAsync();
        
        var existingCardIds = await context.Card
            .Where(dbCard => cardNames.Contains(dbCard.Name))
            .Select(dbCard => dbCard.Id)
            .ToListAsync();

        var missingCards = cards
            .Where(c => !existingCardNames.Contains(c.Name))
            .ToList();

        return (missingCards, existingCardIds);
    }

    public async Task Update(Card card)
    {
        context.Card.Update(card);
        await context.SaveChangesAsync();
    }

    public async Task Update(IList<Card> cards)
    {
        context.Card.UpdateRange(cards);
        await context.SaveChangesAsync();
    }

    public async Task<List<Card>> SelectXCards(int amount)
    {
        return await context.Card.Take(amount).ToListAsync();
    }
}