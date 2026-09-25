namespace SunamoXliffParser;

/// <summary>
/// Provides utility methods for working with XML elements and attributes.
/// </summary>
public static class XmlUtil
{
    /// <summary>
    /// Gets the value of an attribute if it exists on the specified element.
    /// </summary>
    /// <param name="node">The XML element to inspect.</param>
    /// <param name="name">The name of the attribute to retrieve.</param>
    /// <returns>The attribute value, or an empty string if the attribute does not exist.</returns>
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

    /// <summary>
    /// Gets the integer value of an attribute if it exists on the specified element.
    /// </summary>
    /// <param name="node">The XML element to inspect.</param>
    /// <param name="name">The name of the attribute to retrieve.</param>
    /// <returns>The parsed integer value, or 0 if the attribute does not exist.</returns>
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

    /// <summary>
    /// Normalizes line breaks by removing carriage return characters.
    /// </summary>
    /// <param name="text">The text to normalize.</param>
    /// <returns>The text with carriage returns removed, or an empty string if the input is null or whitespace.</returns>
    public static string NormalizeLineBreaks(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? string.Empty : text.Replace("\r", string.Empty);
    }

    /// <summary>
    /// Restores standard Windows line breaks by replacing lone newlines with carriage return + newline pairs.
    /// </summary>
    /// <param name="text">The text to de-normalize.</param>
    /// <returns>The text with Windows-style line breaks, or an empty string if the input is null or whitespace.</returns>
    public static string DeNormalizeLineBreaks(string text)
    {
        return string.IsNullOrWhiteSpace(text) ? string.Empty : NormalizeLineBreaks(text).Replace("\n", "\r\n");
    }
}
