namespace SunamoXliffParser;

/// <summary>
/// Contains the results of an XLIFF update operation, listing added, removed, and updated item identifiers.
/// </summary>
public class UpdateResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateResult"/> class.
    /// </summary>
    /// <param name="addedItems">The identifiers of items that were added.</param>
    /// <param name="removedItems">The identifiers of items that were removed.</param>
    /// <param name="updatedItems">The identifiers of items that were updated.</param>
    public UpdateResult(IEnumerable<string> addedItems, IEnumerable<string> removedItems,
        IEnumerable<string> updatedItems)
    {
        AddedItems = addedItems;
        RemovedItems = removedItems;
        UpdatedItems = updatedItems;
    }

    /// <summary>
    /// Gets or sets the identifiers of items that were added.
    /// </summary>
    public IEnumerable<string> AddedItems { get; set; }

    /// <summary>
    /// Gets or sets the identifiers of items that were removed.
    /// </summary>
    public IEnumerable<string> RemovedItems { get; set; }

    /// <summary>
    /// Gets or sets the identifiers of items that were updated.
    /// </summary>
    public IEnumerable<string> UpdatedItems { get; set; }

    /// <summary>
    /// Determines whether any items were added, removed, or updated.
    /// </summary>
    /// <returns>True if any items were changed; otherwise, false.</returns>
    public bool Any()
    {
        return AddedItems.Any() || RemovedItems.Any() || UpdatedItems.Any();
    }
}
