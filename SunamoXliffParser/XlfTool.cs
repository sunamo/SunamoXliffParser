namespace SunamoXliffParser;

/// <summary>
/// Represents the tool element of an XLIFF file header, containing metadata about the tool that created the file.
/// </summary>
public class XlfTool
{
    /// <summary>
    /// Gets the company name of the tool.
    /// </summary>
    public string Company { get; } = string.Empty;

    /// <summary>
    /// Gets the tool identifier.
    /// </summary>
    public string Id { get; } = string.Empty;

    /// <summary>
    /// Gets the name of the tool.
    /// </summary>
    public string Name { get; } = string.Empty;

    /// <summary>
    /// Gets the version of the tool.
    /// </summary>
    public string Version { get; } = string.Empty;
}
