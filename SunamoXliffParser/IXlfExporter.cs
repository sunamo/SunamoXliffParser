namespace SunamoXliffParser;

/// <summary>
/// Defines a contract for exporting XLIFF translation units to a file.
/// </summary>
public interface IXlfExporter
{
    /// <summary>
    /// Exports translation units to the specified output file.
    /// </summary>
    /// <param name="filePath">The path of the output file.</param>
    /// <param name="units">The collection of translation units to export.</param>
    /// <param name="targetLanguage">The target language code.</param>
    /// <param name="dialect">The XLIFF dialect to use during export.</param>
    void ExportTranslationUnits(string filePath, IEnumerable<XlfTransUnit> units, string targetLanguage,
        XlfDialect dialect);
}
