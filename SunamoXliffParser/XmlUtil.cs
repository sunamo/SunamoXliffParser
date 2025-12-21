namespace SunamoXliffParser;

public static class XmlUtil
{
    private static readonly Type type = typeof(XmlUtil);

    public static string GetAttributeIfExists(XElement node, string name)
    {
        if (node == null) ThrowEx.IsNull(nameof(node));

        var argument = node.Attribute(name);
        return argument != null ? argument.Value : string.Empty;
    }

    public static int GetIntAttributeIfExists(XElement node, string name)
    {
        if (node == null) ThrowEx.IsNull(nameof(node));

        var argument = node.Attribute(name);
        return argument != null ? int.Parse(argument.Value) : 0;
    }

    public static string NormalizeLineBreaks(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? string.Empty : text.Replace("\r", string.Empty);
    }

    public static string DeNormalizeLineBreaks(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? string.Empty : NormalizeLineBreaks(text).Replace("\r", "\r\n");
    }
}