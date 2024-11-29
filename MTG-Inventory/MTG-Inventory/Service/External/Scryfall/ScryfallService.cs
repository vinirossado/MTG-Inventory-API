using System.Text.Json;
using MTG_Card_Checker.Repository.External.Scryfall.Model;
using MTG_Inventory.Models;

namespace MTG_Inventory.Service.External.Scryfall
{
    public class ScryfallService
    {
        private static readonly HttpClient client;
        private static readonly Dictionary<string, Root> _cache = new();

        static ScryfallService()
        {
            client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    { "User-Agent", "MTG-Lifecounter" },
                    { "Accept", "*/*" }
                }
            };
        }

        public async Task GetCard(IList<Card> cards)
        {
            try
            {
                foreach (var card in cards)
                {
                    Console.WriteLine($"Processing card: {card.Name}");
                    var url = $"https://api.scryfall.com/cards/search?q={Uri.EscapeDataString(card.Name)}";

                    if (_cache.ContainsKey(card.Name))
                    {
                        var cachedCardData = _cache[card.Name];
                        ProcessScryfallResponse(card, cachedCardData);
                    }
                    else
                    {
                        var scryfallResponse = await GetScryfallDataAsync(url).ConfigureAwait(false);

                        var deserializedCard = JsonSerializer.Deserialize<Root>(scryfallResponse);
                        if (deserializedCard != null)
                        {
                            _cache[card.Name] = deserializedCard;
                        }

                        ProcessScryfallResponse(card, deserializedCard);
                    }

                    await Task.Delay(100).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        
        private static void ProcessScryfallResponse(Card card, Root deserializedCard)
        {
            if (deserializedCard.Data.Count <= 0) return;
            
            var data = deserializedCard.Data[0];
            card.CMC = data.Cmc;
            card.TypeLine = data.TypeLine;
            card.ColorIdentity = string.Join(",", data.ColorIdentity);
            card.OracleId = data.Id;
            card.ImageUri = data.ImageUris?.Large;
            card.Price = data.Prices?.Eur;
            card.Rarity = data.Rarity.ToUpper();
            card.IsCommander = data.TypeLine.Contains("Legendary Creature") || data.TypeLine.Contains("Summon Legend");
        }

        private static async Task<string> GetScryfallDataAsync(string url)
        {
            var response = await client.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        }
    }
}
