namespace SunamoXliffParser.fmdev.ResX;

/// <summary>
/// Provides methods for reading and writing ResX resource files.
/// </summary>
public static class ResXFile
{
    /// <summary>
    /// Reads all entries from the specified ResX file.
    /// </summary>
    /// <param name="filePath">The path to the ResX file to read.</param>
    /// <param name="options">Options controlling the read behavior.</param>
    /// <returns>A list of resource entries read from the file.</returns>
    public static List<ResXEntry> Read(string filePath, ResXOption options = ResXOption.None)
    {
        var result = new List<ResXEntry>();
        using (var reader = new ResXResourceReader(filePath))
        {
            reader.UseResXDataNodes = true;
            var enumerator = reader.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var dataNode = enumerator.Value as ResXDataNode;
                var comment = options.HasFlag(ResXOption.SkipComments)
                    ? string.Empty
                    : dataNode!.Comment.Replace("\r", string.Empty);
                result.Add(new ResXEntry
                {
                    Id = enumerator.Key as string ?? string.Empty,
                    Value = ((dataNode!.GetValue((ITypeResolutionService?)null) as string) ?? string.Empty).Replace("\r", string.Empty),
                    Comment = comment
                });
            }

            reader.Close();
        }

        return result;
    }

    /// <summary>
    /// Writes the specified entries to a ResX file.
    /// </summary>
    /// <param name="filePath">The path to the ResX file to write.</param>
    /// <param name="entries">The collection of resource entries to write.</param>
    /// <param name="options">Options controlling the write behavior.</param>
    public static void Write(string filePath, IEnumerable<ResXEntry> entries, ResXOption options = ResXOption.None)
    {
        using (var writer = new ResXResourceWriter(filePath))
        {
            foreach (var entry in entries)
            {
                var dataNode = new ResXDataNode(entry.Id,
                    entry.Value.Replace("\r", string.Empty).Replace("\n", Environment.NewLine));
                if (!options.HasFlag(ResXOption.SkipComments) && !string.IsNullOrWhiteSpace(entry.Comment))
                    dataNode.Comment = entry.Comment.Replace("\r", string.Empty).Replace("\n", Environment.NewLine);

                writer.AddResource(dataNode);
            }

            writer.Close();
        }
    }
}
