namespace SunamoXliffParser.fmdev.ResX;

/// <summary>
/// Options for reading and writing ResX files.
/// </summary>
public enum ResXOption
{
    /// <summary>
    /// No special options.
    /// </summary>
    None = 0,

    /// <summary>
    /// Skip reading or writing comments in the ResX file.
    /// </summary>
    SkipComments = 1
}