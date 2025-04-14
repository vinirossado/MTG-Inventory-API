namespace MTG_Inventory.Dtos;

public record PagedResponseKeysetDto<T>
{
    public int Reference { get; init; }
    public List<T> Data { get; init; }
}