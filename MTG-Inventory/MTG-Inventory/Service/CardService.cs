using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Caching.Memory;
using MTG_Inventory.Models;
using MTG_Inventory.Repository;
using MTG_Inventory.Service.External.Scryfall;

namespace MTG_Inventory.Service;

public class CardService(CardRepository cardRepository, ScryfallService scryfallService, IMemoryCache memoryCache)
{
    private const string CacheKey = "AllCardsCache";

    public async Task ImportDatabase(IFormFile file)
    {
        var cards = await ReadCardsFromCsv(file);
        
        await scryfallService.GetCard(cards);
        await cardRepository.Add(cards);
        
        await RefreshCache();
    }
    
    public async Task SetCommander()
    {
        var cards = await cardRepository.Get();

        foreach (var card in cards)
        {
            if(card.TypeLine == null) continue;
            
            card.IsCommander = card.TypeLine.Contains("Legendary Creature") || card.TypeLine.Contains("Summon Legend");
        }
        
        await cardRepository.Update(cards);
    }
    
    public async Task Sync()
    {
        var cards = await cardRepository.GetMissingSyncCards();
        await scryfallService.GetCard(cards);
        
        await cardRepository.Update(cards);
        
        await RefreshCache();
    }

    public async Task<(List<FilteredCard> foundCards, List<FilteredCard> missingCards)> CompareWantListWithDb(IFormFile file)
    {
        // var cardsFromFile = await ReadCardsFromTextFile(file);
        // var allCardsInDb = await GetCardsFromCache();
        //
        // var missingCards = GetMissingCards(cardsFromFile, allCardsInDb);
        // var foundCards = GetFoundCards(cardsFromFile, allCardsInDb);

        // return (foundCards, missingCards);
        return (null, null);
    }

    public async Task Update(Card card)
    {
        await cardRepository.Update(card);
        await RefreshCache();
    }

    public async Task Add(Card card)
    {
        await cardRepository.Add(card);
        await RefreshCache();
    }
    
    public async Task Add(IList<Card> card)
    {
        await cardRepository.Add(card);
        await RefreshCache();
    }

    public async Task<List<Card>> SelectXCards(int amount)
    {
        return await cardRepository.SelectXCards(amount);
    }

    private async Task<IList<Card>> GetCardsFromCache()
    {
        if (memoryCache.TryGetValue(CacheKey, out IList<Card> cachedCards)) return cachedCards;

        cachedCards = await cardRepository.Get();
        memoryCache.Set(CacheKey, cachedCards);

        return cachedCards;
    }

    public async Task RefreshCache()
    {
        var updatedCards = await cardRepository.Get();
        memoryCache.Set(CacheKey, updatedCards);
    }

    private async Task<IList<Card>> ReadCardsFromCsv(IFormFile file)
    {
        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HeaderValidated = null,
                PrepareHeaderForMatch = args => args.Header.Trim(),
                BadDataFound = null
            });

            return csv.GetRecords<Card>().ToList();
        }
        catch (Exception ex)
        {
            // Log or handle exception
            throw new Exception("Error reading CSV file", ex);
        }
    }

    private List<FilteredCard> GetMissingCards(IList<Card>? cardsFromFile, IList<Card> allCardsInDb)
    {
        // var missingCards = cardsFromFile
        //     .Where(card => allCardsInDb.All(dbCard => dbCard.Name != card.Name))
        //     .Select(card => new FilteredCard
        //     {
        //         Name = card.Name,
        //         Quantity = 1
        //     }).OrderBy(x => x.Name)
        //     .ToList();
        //
        // missingCards.AddRange(cardsFromFile
        //     .Where(card => allCardsInDb.Any(dbCard =>
        //         dbCard.Name == card.Name &&
        //         card.Quantity > dbCard.Quantity - dbCard.InUse))
        //     .Select(card =>
        //     {
        //         var dbCard = allCardsInDb.FirstOrDefault(dbCard => dbCard.Name == card.Name);
        //         return new FilteredCard
        //         {
        //             Name = card.Name,
        //             Quantity = (int)(card.Quantity - ((dbCard?.Quantity ?? 0) - (dbCard?.InUse ?? 0)))!
        //         };
        //     }).OrderBy(x => x.Name)
        //     .ToList());

        //return missingCards;
        return null;
    }

    private List<FilteredCard> GetFoundCards(IList<Card> cardsFromFile, IList<Card> allCardsInDb)
    {
        // return cardsFromFile
        //     .Where(card => allCardsInDb.Any(dbCard => dbCard.Name == card.Name && card.Quantity <= dbCard.Quantity - dbCard.InUse))
        //     .Select(card => new FilteredCard
        //     {
        //         Name = card.Name,
        //         Quantity = (int)card.Quantity
        //     }).OrderBy(x => x.Name)
        //     .ToList();
        return null;
    }
    
    public async Task<(IList<Card>, IList<int>)> GetMissingCards(IList<Card> cards)
    {
        return await cardRepository.GetMissingCards(cards);
    }
}