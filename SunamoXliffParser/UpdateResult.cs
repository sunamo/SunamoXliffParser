namespace SunamoXliffParser;

public class UpdateResult
{
    public UpdateResult(IEnumerable<string> addedItems, IEnumerable<string> removedItems,
        IEnumerable<string> updatedItems)
    {
        AddedItems = addedItems;
        RemovedItems = removedItems;
        UpdatedItems = updatedItems;
    }

    public IEnumerable<string> AddedItems { get; set; }
    public IEnumerable<string> RemovedItems { get; set; }
    public IEnumerable<string> UpdatedItems { get; set; }

    /// <summary>
    /// Determines whether any items were added, removed, or updated.
    /// </summary>
    /// <returns>True if any items were changed; otherwise, false.</returns>
    public bool Any() => AddedItems.Any() || RemovedItems.Any() || UpdatedItems.Any();
}
