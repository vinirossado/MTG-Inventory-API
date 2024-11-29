using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MTG_Inventory.Service;

namespace MTG_Inventory.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController(CardService cardService)  : ControllerBase
{
    [HttpPost("/upload-cards")]
    [Consumes("multipart/form-data")]
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

        return Ok(new { message = "Importação iniciada! O processo está em andamento." });
    }
}