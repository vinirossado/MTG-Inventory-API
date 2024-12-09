using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MTG_Inventory.Models;

namespace MTG_Inventory.Helpers;

public static class FileHelper
{
    public static async Task<IList<Card>> ReadCardsFromTextFile(IFormFile file)
    {
        var cards = new List<Card>();
        using var reader = new StreamReader(file.OpenReadStream());

        while (reader.Peek() >= 0)
        {
            var line = await reader.ReadLineAsync();
            var parts = line?.Split(new[] { ' ' }, 2);
            if (parts?.Length == 2 && int.TryParse(parts[0], out var quantity))
            {
                cards.Add(new Card { Quantity = quantity, Name = parts[1].Trim() });
            }
        }

        return cards;
    }
    
    public static Task<IList<Card>> ReadCardsFromCsv(IFormFile file)
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

            return Task.FromResult<IList<Card>>(csv.GetRecords<Card>().ToList());
        }
        catch (Exception ex)
        {
            throw new Exception("Error reading CSV file", ex);
        }
    }
}