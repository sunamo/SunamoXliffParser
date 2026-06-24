namespace SunamoXliffParser;

public static class XmlUtil
{
    public static string GetAttributeIfExists(XElement node, string name)
    {
        if (node == null)
        {
            ThrowEx.IsNull(nameof(node));
            return string.Empty;
        }

        var attribute = node.Attribute(name);
        return attribute != null ? attribute.Value : string.Empty;
    }

    public static int GetIntAttributeIfExists(XElement node, string name)
    {
        if (node == null)
        {
            ThrowEx.IsNull(nameof(node));
            return 0;
        }

        var attribute = node.Attribute(name);
        return attribute != null ? int.Parse(attribute.Value) : 0;
    }

    public static string NormalizeLineBreaks(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? string.Empty : text.Replace("\r", string.Empty);
    }

    public static string DeNormalizeLineBreaks(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? string.Empty : NormalizeLineBreaks(text).Replace("\n", "\r\n");
    }
}
