using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using MTG_Inventory.Dtos;
using MTG_Inventory.Models;
using MTG_Inventory.Service;

namespace MTG_Inventory.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardController(CardService cardService) : ControllerBase
{
    [HttpPost("/upload-cards")]
    public async Task<IActionResult> UploadCards([Required] IFormFile file)
    {
        if (file.Length == 0)
        {
            return BadRequest("No file was uploaded");
        }

        if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Invalid file type");
        }

        await cardService.ImportDatabase(file);

        return Ok();
    }

    [HttpPost("/want-list")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<(List<FilteredCard> foundCards, List<Card> missingCards)>> WantList([Required] IFormFile file, bool saveWantList = true)
    {
        if (file.Length == 0)
        {
            return BadRequest("No file was uploaded");
        }

        if (!file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Invalid file type");
        }

        var (foundCards, missingCards) = await cardService.CompareWantListWithDb(file);

        var response = new WantListResponse
        {
            FoundCards = foundCards,
            MissingCards = missingCards,
            MissingCardsFileUrl = GenerateMissingCardsFile(missingCards)
        };

        return Ok(response);
    }

    private static string GenerateMissingCardsFile(List<FilteredCard> missingCards)
    {
        var sb = new StringBuilder();
        foreach (var missingCard in missingCards)
        {
            sb.AppendLine($"{missingCard.Quantity} {missingCard.Name}");
        }

        var fileName = Path.Combine(Path.GetTempPath(), "missing-cards.txt");
        System.IO.File.WriteAllText(fileName, sb.ToString());

        return fileName;
    }
    
    [HttpGet("/sync")]
    public async Task<IActionResult> Sync()
    {
        await cardService.Sync();

        return Ok(new { message = "Cards has been sync!" });
    }

    [HttpGet("GetCardsWithPagination")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponseKeysetDto<CardResultDto>>> GetCardsWithPagination(
        int reference = 0, int pageSize = 10, [FromQuery] string? filters = null)
    {
        // if (pageSize <= 0)
        //     return BadRequest($"{nameof(pageSize)} size must be greater than 0.");

        CardFilterDto? filterDto = null;
        if (!string.IsNullOrEmpty(filters))
        {
            try
            {
                filterDto = JsonSerializer.Deserialize<CardFilterDto>(filters);
            }
            catch (JsonException)
            {
                return BadRequest("Invalid filters format.");
            }
        }

        var pagedCards = await cardService.GetCardsWithPagination(reference, pageSize, filterDto);

        var pagedCardsDto = pagedCards.Adapt<PagedResponseKeysetDto<CardResultDto>>();

        return Ok(pagedCardsDto);
    }
}