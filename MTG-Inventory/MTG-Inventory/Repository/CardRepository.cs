using Microsoft.EntityFrameworkCore;
using MTG_Inventory.Dtos;
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
        return await context.Card.AsNoTracking().ToListAsync();
    }
    
    public async Task<List<Card>> GetCardsWithNoImage()
    {
        return await context.Card.Where(x=> x.ImageUri == null).ToListAsync();
    }

    public async Task<List<Card>> GetMissingSyncCards()
    {
        return await context.Card.Where(x => x.TypeLine == null).ToListAsync();
    }

    public List<FilteredCard> GetMissingCards(IList<Card>? cardsFromFile, IList<Card> allCardsInDb)
    {
        if (cardsFromFile == null) return [];

        var missingCards = cardsFromFile
            .Where(card => allCardsInDb.All(dbCard => dbCard.Name != card.Name))
            .Select(card => new FilteredCard
            {
                Name = card.Name,
                Quantity = 1
            }).OrderBy(x => x.Name)
            .ToList();

        missingCards.AddRange(cardsFromFile
            .Where(card => allCardsInDb.Any(dbCard =>
                dbCard.Name == card.Name &&
                card.Quantity > dbCard.Quantity - dbCard.InUse))
            .Select(card =>
            {
                var dbCard = allCardsInDb.FirstOrDefault(dbCard => dbCard.Name == card.Name);
                return new FilteredCard
                {
                    Name = card.Name,
                    Quantity = (card.Quantity - ((dbCard?.Quantity ?? 0) - (dbCard?.InUse ?? 0)))!
                };
            }).OrderBy(x => x.Name)
            .ToList());

        return missingCards;
    }
    
    public List<FilteredCard> GetFoundCards(IList<Card> cardsFromFile, IList<Card> allCardsInDb)
    {
        return cardsFromFile
            .Where(card => allCardsInDb.Any(dbCard => dbCard.Name == card.Name && card.Quantity <= dbCard.Quantity - dbCard.InUse))
            .Select(card => new FilteredCard
            {
                Name = card.Name,
                Quantity = card.Quantity
            }).OrderBy(x => x.Name)
            .ToList();
    }
    
    public async Task<IList<Card>> GetMissingCardsToImport(IList<Card> cards)
    {
        var allCardsInDatabase = await context.Card
            .Select(dbCard => new Card
            {
                Id = dbCard.Id,
                Name = dbCard.Name.Trim().ToUpper(),
                Quantity = dbCard.Quantity,
                ExpansionName = dbCard.ExpansionName!.Trim().ToUpper()
            })
            .ToListAsync();
    
        var extraCards = cards.Where(x => !allCardsInDatabase.Any(dbCard =>
            dbCard.Name.Equals(x.Name.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
            dbCard.ExpansionName == x.ExpansionName?.Trim().ToUpper() &&
            dbCard.Quantity == x.Quantity
        )).ToList();
    
        Console.WriteLine($"Total no banco: {allCardsInDatabase.Count}, Não na lista: {extraCards.Count}");
    
        return extraCards;
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
    
    public async Task<PagedResponseKeyset<Card>> GetCardsWithPagination(int reference, int pageSize, CardFilterDto filters)
    {
        var query = context.Card.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filters?.Name))
            query = query.Where(card => card.Name.Contains(filters.Name));

        if (!string.IsNullOrWhiteSpace(filters?.ColorIdentity))
            query = query.Where(card => card.ColorIdentity == filters.ColorIdentity.ToUpper());
        
        if (!string.IsNullOrWhiteSpace(filters?.TypeLine))
            query = query.Where(card => card.TypeLine != null && card.TypeLine.ToUpper() == filters.TypeLine.ToUpper());
        
        if (filters?.CMC != null)
            query = query.Where(card => Equals(card.CMC, filters.CMC));
        
        var cards = await query
            .OrderBy(card => card.Id)
            .Where(p => p.Id > reference)
            .Take(pageSize)
            .ToListAsync();

        var newReference = cards.Count != 0 ? cards.Last().Id : 0;

        var pagedResponse = new PagedResponseKeyset<Card>(cards, newReference);

        return pagedResponse;
    }
}