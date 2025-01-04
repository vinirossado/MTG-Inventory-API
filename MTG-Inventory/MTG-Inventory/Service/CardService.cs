using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using MTG_Inventory.Dtos;
using MTG_Inventory.Helpers;
using MTG_Inventory.Models;
using MTG_Inventory.Repository;
using MTG_Inventory.Service.External.Scryfall;

namespace MTG_Inventory.Service;

public partial class CardService(CardRepository cardRepository, ScryfallService scryfallService, IMemoryCache memoryCache)
{
    private const string CacheKey = "AllCardsCache";

    public async Task ImportDatabase(IFormFile file)
    {
        var duplicatedCardsToBeRemoved = await cardRepository.DuplicatedCardsToBeRemoved();
        if(duplicatedCardsToBeRemoved.Count > 0)
            cardRepository.Remove(duplicatedCardsToBeRemoved);
        var cards = await FileHelper.ReadCardsFromCsv(file);

        var missingCardsToImport = await cardRepository.GetMissingCardsToImport(cards);

        await scryfallService.GetCard(missingCardsToImport);

        await cardRepository.CardsToBeRemovedFromDb(cards);
        await SetCommander(missingCardsToImport);
        await cardRepository.Add(missingCardsToImport);

        await RefreshCache();
    }

    private Task SetCommander(IList<Card> cards)
    {
        foreach (var card in cards)
        {
            if (card.TypeLine == null) continue;

            card.IsCommander = MyRegex().IsMatch(card.TypeLine);
        }

        return Task.CompletedTask;
    }

    public async Task Sync()
    {
        var cards = await cardRepository.GetCardsWithNoImage();
        await scryfallService.GetCard(cards);

        await cardRepository.Update(cards);
        memoryCache.Set(CacheKey, cards);
    }

    public async Task<(List<FilteredCard> foundCards, List<FilteredCard> missingCards)> CompareWantListWithDb(IFormFile file)
    {
        var cardsFromFile = await FileHelper.ReadCardsFromTextFile(file);
        var allCardsInDb = await GetCardsFromCache();

        var missingCards = cardRepository.GetMissingCards(cardsFromFile, allCardsInDb);
        var foundCards = cardRepository.GetFoundCards(cardsFromFile, allCardsInDb);

        return (foundCards, missingCards);
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

    private async Task<IList<Card>> GetCardsFromCache()
    {
        if (memoryCache.TryGetValue(CacheKey, out IList<Card> cachedCards)) return cachedCards;

        cachedCards = await cardRepository.Get();
        memoryCache.Set(CacheKey, cachedCards);

        return cachedCards;
    }

    private async Task RefreshCache()
    {
        var updatedCards = await cardRepository.Get();
        memoryCache.Set(CacheKey, updatedCards);
    }
    
    public async Task<PagedResponseKeyset<Card>> GetCardsWithPagination(
        int reference, int pageSize, CardFilterDto filters)
    {
        var allCards = await GetCardsFromCache();
        if(allCards.Count <= 0)
        {
            allCards = await cardRepository.Get();
            memoryCache.Set(CacheKey, allCards);
        }
        
        if(filters?.IsCommander is false)
        {
            filters.IsCommander = null;
        }

        if (filters?.ColorIdentity == "")
        {
            filters.ColorIdentity = null;
        }
        
        if (filters?.ColorIdentity == "Colorless")
        {
            filters.ColorIdentity = "";
        }
        
        var cardsFiltered = allCards
            .Where(card => filters?.Name == null || card.Name.Contains(filters?.Name.TrimEnd() ?? string.Empty, StringComparison.CurrentCultureIgnoreCase))
            .Where(card => filters?.IsCommander == null || card.IsCommander == filters?.IsCommander )
            .Where(card => filters?.ColorIdentity == null || card.ColorIdentity.Contains(filters?.ColorIdentity, StringComparison.CurrentCultureIgnoreCase))
            // .Where(card => filters.CMC == null || card.CMC == filters.CMC)
            // .Where(card => filters.TypeLine == null || card.TypeLine.Contains(filters.TypeLine, StringComparison.CurrentCultureIgnoreCase))
            // .Skip(reference)
            // .Take(pageSize)
            .ToList();
        
        // var cards =  await cardRepository.GetCardsWithPagination(
        //     reference, pageSize, filters);

        var pagedResponse = new PagedResponseKeyset<Card>
        (
            cardsFiltered,
            cardsFiltered.Any() ? cardsFiltered.Last().Id : 0
        );
        
        return pagedResponse;
    }

    [GeneratedRegex(@"Legendary\s*Creature|Summon\s*Legend", RegexOptions.IgnoreCase, "en-EE")]
    private static partial Regex MyRegex();
}