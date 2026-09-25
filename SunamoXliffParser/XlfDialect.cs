namespace SunamoXliffParser;

/// <summary>
/// Specifies the dialect of an XLIFF document.
/// </summary>
public enum XlfDialect
{
    /// <summary>
    /// Standard XLIFF format.
    /// </summary>
    Standard = 0,

    /// <summary>
    /// RCWinTrans 11 dialect which uses the resname attribute for identification.
    /// </summary>
    RCWinTrans11 = 1,

    /// <summary>
    /// Microsoft Multilingual App Toolkit dialect which uses the Resx/ prefix for identifiers.
    /// </summary>
    MultilingualAppToolkit = 2
}