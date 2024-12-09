using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

namespace MTG_Inventory.Dtos;

public class CardFilterDto
{
    // [OpenApiSchema("Card name to filter by")]
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    // [OpenApiSchema("Color Identity", 
    //     Enum = new[] { "W", "U", "B", "R", "G", "C", "M" })]
    [JsonPropertyName("color_identity")]
    [RegularExpression("^[WUBRGCM]$", ErrorMessage = "Invalid color identity")]
    public string? ColorIdentity { get; set; }
    
    // [OpenApiSchema("Converted Mana Cost (CMC)")]
    [JsonPropertyName("cmc")]
    [Range(0, 20, ErrorMessage = "CMC must be between 0 and 20")]
    public double? CMC { get; set; }
    
    // [OpenApiSchema("Card type line", 
    //     Enum = new[] { 
    //         "Legendary Creature", 
    //         "Creature", 
    //         "Instant", 
    //         "Sorcery", 
    //         "Enchantment", 
    //         "Artifact", 
    //         "Planeswalker", 
    //         "Land" 
    //     })]
    [JsonPropertyName("type_line")]
    public string? TypeLine { get; set; }
}