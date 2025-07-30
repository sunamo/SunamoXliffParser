namespace SunamoXliffParser.fmdev.ResX;

public class ResXEntry : IComparable
{
    public string Id { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public int CompareTo(object obj)
    {
        return obj is ResXEntry ? Id.CompareTo((obj as ResXEntry).Id) : Id.CompareTo(obj.ToString());
    }
}