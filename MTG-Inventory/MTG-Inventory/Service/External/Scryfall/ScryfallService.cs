using System.Net;
using System.Text.Json;
using MTG_Card_Checker.Repository.External.Scryfall.Model;
using MTG_Inventory.Models;

namespace MTG_Inventory.Service.External.Scryfall
{
    public class ScryfallService
    {
        private static readonly HttpClient client;
        private static readonly Dictionary<string, Root> _cache = new();
        private static readonly List<string> failedCards = new();

        static ScryfallService()
        {
            client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    { "User-Agent", "MTG-VRPersonal-Inventory" },
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
                        var scryfallResponse = await GetScryfallDataWithRetryAsync(url).ConfigureAwait(false);

                        if (scryfallResponse != null)
                        {
                            var deserializedCard = JsonSerializer.Deserialize<Root>(scryfallResponse);
                            if (deserializedCard != null)
                            {
                                _cache[card.Name] = deserializedCard;
                                ProcessScryfallResponse(card, deserializedCard);
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Failed to retrieve data for card: {card.Name}");
                            failedCards.Add(card.Name); 
                        }
                    }

                    await Task.Delay(130).ConfigureAwait(false);
                }

                // Optionally log failed cards at the end
                if (failedCards.Count > 0)
                {
                    Console.WriteLine($"The following cards failed to retrieve: {string.Join(", ", failedCards)}");
                    LogFailedCardsToFile(); 
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

        private static async Task<string?> GetScryfallDataWithRetryAsync(string url, int maxRetries = 3, int delayBetweenRetriesMs = 1000)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var response = await client.GetAsync(url).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    }

                    if (response.StatusCode >= HttpStatusCode.InternalServerError) // 5xx errors
                    {
                        Console.WriteLine($"Server error (attempt {attempt}/{maxRetries}): {response.StatusCode}");
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code {response.StatusCode}. Not retrying.");
                        break; // For non-retryable status codes, exit immediately.
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Request error (attempt {attempt}/{maxRetries}): {ex.Message}");
                }

                if (attempt < maxRetries)
                {
                    await Task.Delay(delayBetweenRetriesMs).ConfigureAwait(false);
                }
            }

            return null;
        }

        private static void LogFailedCardsToFile()
        {
            try
            {
                var filePath = "FailedCardsLog.txt";
                File.AppendAllLines(filePath, failedCards);
                Console.WriteLine($"Failed cards logged to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing failed cards to file: {ex.Message}");
            }
        }
    }
}
