namespace MTG_Inventory.Models;

public record PagedResponseKeyset<T>
{
    public int Reference { get; init; }
    public List<T> Data { get; init; }

    public PagedResponseKeyset(List<T> data, int reference)
    {
        Data = data;
        Reference = reference;
    }
    
}