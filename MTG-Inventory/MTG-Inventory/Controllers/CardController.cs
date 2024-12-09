using System.ComponentModel.DataAnnotations;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using MTG_Inventory.Dtos;
using MTG_Inventory.Helpers;
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
            MissingCards = missingCards
        };

        return Ok(response);
    }

    [HttpGet("/sync")]
    public async Task<IActionResult> Sync()
    {
        await cardService.Sync();

        return Ok(new { message = "Commanders has been set!" });
    }

    [HttpGet("GetCardsWithPagination")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCardsWithPagination(
        int reference = 0, int pageSize = 10)
    {
        if (pageSize <= 0)
            return BadRequest($"{nameof(pageSize)} size must be greater than 0.");

        var pagedCards = await cardService.GetCardsWithPagination(reference, pageSize);

        var pagedCardsDto = pagedCards.Adapt<PagedResponseKeysetDto<CardResultDto>>();

        return Ok(pagedCardsDto);
    }
}