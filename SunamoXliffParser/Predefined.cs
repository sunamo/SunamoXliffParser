namespace SunamoXliffParser;

/// <summary>
/// Defines the predefined state values for XLIFF translation units as specified by the XLIFF standard.
/// </summary>
public enum Predefined
{
    /// <summary>
    /// Indicates the terminating state.
    /// </summary>
    Final,

    /// <summary>
    /// Indicates only non-textual information needs adaptation.
    /// </summary>
    NeedsAdaptation,

    /// <summary>
    /// Indicates both text and non-textual information needs adaptation.
    /// </summary>
    NeedsL10n,

    /// <summary>
    /// Indicates only non-textual information needs review.
    /// </summary>
    NeedsReviewAdaptation,

    /// <summary>
    /// Indicates both text and non-textual information needs review.
    /// </summary>
    NeedsReviewL10n,

    /// <summary>
    /// Indicates that only the text of the item needs to be reviewed.
    /// </summary>
    NeedsReviewTranslation,

    /// <summary>
    /// Indicates that the item needs to be translated.
    /// </summary>
    NeedsTranslation,

    /// <summary>
    /// Indicates that the item is new, for example translation units not in a previous version.
    /// </summary>
    New,

    /// <summary>
    /// Indicates that changes are reviewed and approved.
    /// </summary>
    SignedOff,

    /// <summary>
    /// Indicates that the item has been translated.
    /// </summary>
    Translated
}