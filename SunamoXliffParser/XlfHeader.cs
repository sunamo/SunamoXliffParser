namespace SunamoXliffParser;

/// <summary>
/// Represents the header element of an XLIFF file.
/// </summary>
public class XlfHeader
{
    private readonly XElement node;

    /// <summary>
    /// Initializes a new instance of the <see cref="XlfHeader"/> class.
    /// </summary>
    /// <param name="node">The XML element representing the header.</param>
    public XlfHeader(XElement node)
    {
        this.node = node;
    }
}
