namespace SunamoXliffParser.fmdev.ResX;

public static class ResXFile
{
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
