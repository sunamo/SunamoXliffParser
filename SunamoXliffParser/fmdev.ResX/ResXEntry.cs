namespace SunamoXliffParser.fmdev.ResX;

/// <summary>
/// Represents a single entry in a ResX resource file, containing an identifier, value, and optional comment.
/// </summary>
public class ResXEntry : IComparable
{
    /// <summary>
    /// Gets or sets the resource identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource value.
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the resource comment.
    /// </summary>
    public string Comment { get; set; } = string.Empty;

    /// <summary>
    /// Compares this entry to another object by their identifiers.
    /// </summary>
    /// <param name="other">The object to compare with this entry.</param>
    /// <returns>A value indicating the relative order of the objects being compared.</returns>
    public int CompareTo(object? other)
    {
        return other is ResXEntry ? Id.CompareTo((other as ResXEntry)!.Id) : Id.CompareTo(other?.ToString());
    }
}
