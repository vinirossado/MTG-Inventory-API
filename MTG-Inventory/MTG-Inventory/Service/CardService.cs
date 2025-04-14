using System.Text.Json;
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
        // if (memoryCache.TryGetValue(CacheKey, out IList<Card> cachedCards)) return cachedCards;

        var allCardsInDatabase = await cardRepository.Get();

        var uniqueCards = new Dictionary<string, Card>();

        foreach (var card in allCardsInDatabase)
        {
            var key = card.Name.ToLowerInvariant();

            if (uniqueCards.ContainsKey(key))
            {
                uniqueCards[key].Quantity += card.Quantity;
            }
            else
            {
                uniqueCards[key] = card;
            }
        }

        var cachedCards = uniqueCards.Values.ToList();
        // memoryCache.Set(CacheKey, cachedCards);

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
        var filtersJson = JsonSerializer.Serialize(filters);
        // var filtersHash = GerarHash(filtersJson);
        // var cacheKey = $"Cards:Reference:{reference}:PageSize:{pageSize}:Filters:{filtersHash}";
        //
        // if (memoryCache.TryGetValue(cacheKey, out List<Card> cachedCards))
        // {
        //     return CreatePaginatedResponse(cachedCards);
        // }

        var allCards = await GetCardsFromCache();

        // memoryCache.Set(CacheKey, allCards);

        if (filters?.IsCommander is false)
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

        filters ??= new CardFilterDto();

        filters!.TypeLine = "Enchantment";

        var cardsFiltered = allCards
            .Where(card => filters?.Name == null || card.Name.Contains(filters?.Name.TrimEnd() ?? string.Empty, StringComparison.CurrentCultureIgnoreCase))
            .Where(card => filters?.IsCommander == null || card.IsCommander == filters?.IsCommander)
            .Where(card => filters?.ColorIdentity == null || card.ColorIdentity!.Equals(filters?.ColorIdentity, StringComparison.CurrentCultureIgnoreCase))
            // .Where(card => filters.CMC == null || card.CMC == filters.CMC)
            .Where(card => filters.TypeLine == null || card.TypeLine.Contains(filters.TypeLine, StringComparison.CurrentCultureIgnoreCase));

        var paginatedCards = cardsFiltered
            // .Skip(reference * pageSize)
            .OrderBy(card => card.Name)
            // .Take(pageSize)
            .ToList();

        // memoryCache.Set(cacheKey, paginatedCards);

        return CreatePaginatedResponse(paginatedCards);
    }

    private PagedResponseKeyset<Card> CreatePaginatedResponse(List<Card> cards)
    {
        return new PagedResponseKeyset<Card>
        (
            cards,
            cards.Count != 0 ? cards.Last().Id : 0
        );
    }

    [GeneratedRegex(@"Legendary\s*Creature|Summon\s*Legend", RegexOptions.IgnoreCase, "en-EE")]
    private static partial Regex MyRegex();

    private string GerarHash(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(hashBytes);
    }
}