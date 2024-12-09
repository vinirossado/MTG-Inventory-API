namespace MTG_Inventory.Dtos;

public record CardResultDto
{
    public int Id{ get; init; }
    public string Name { get; init; }
    public string? ImageUri { get; set; }
    public string? ColorIdentity { get; set; } // W, U, B, R, G, C, M
    public bool? IsCommander { get; set; }
}