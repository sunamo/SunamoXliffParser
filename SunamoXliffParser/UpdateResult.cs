// EN: Variable names have been checked and replaced with self-descriptive names
// CZ: Názvy proměnných byly zkontrolovány a nahrazeny samopopisnými názvy
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

    public bool Any()
    {
        return AddedItems.Any() || RemovedItems.Any() || UpdatedItems.Any();
    }
}