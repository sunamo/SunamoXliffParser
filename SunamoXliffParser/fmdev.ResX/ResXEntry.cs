namespace SunamoXliffParser.fmdev.ResX;

public class ResXEntry : IComparable
{
    public string Id { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public int CompareTo(object? other)
    {
        return other is ResXEntry ? Id.CompareTo((other as ResXEntry)!.Id) : Id.CompareTo(other?.ToString());
    }
}
