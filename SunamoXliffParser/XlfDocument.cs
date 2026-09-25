namespace SunamoXliffParser;

/// <summary>
/// Represents an XLIFF document and provides methods for reading, modifying, and exporting translation data.
/// </summary>
public class XlfDocument
{
    /// <summary>
    /// Specifies options for saving an XLIFF document as a ResX file.
    /// </summary>
    [Flags]
    public enum ResXSaveOption
    {
        /// <summary>
        /// No special options.
        /// </summary>
        None = 0,

        /// <summary>
        /// Sort entries alphabetically by identifier.
        /// </summary>
        SortEntries = 1,

        /// <summary>
        /// Include comments from translation unit notes.
        /// </summary>
        IncludeComments = 2
    }

    private const string AttributeOriginal = "original";
    private const string ElementFile = "file";
    private const string AttributeVersion = "version";

    /// <summary>
    /// Gets the runtime type of <see cref="XlfDocument"/>.
    /// </summary>
    public static Type DocumentType { get; } = typeof(XlfDocument);

    private XDocument document = null!;

    /// <summary>
    /// Gets the file name of the loaded XLIFF document.
    /// </summary>
    public string? FileName { get; }

    /// <summary>
    /// Gets all file elements contained in this XLIFF document.
    /// </summary>
    public IEnumerable<XlfFile> Files
    {
        get
        {
            var xmlNamespace = document.Root!.Name.Namespace;
            return document.Descendants(xmlNamespace + ElementFile).Select(fileElement => new XlfFile(fileElement, xmlNamespace));
        }
    }

    /// <summary>
    /// Gets or sets the XLIFF version of this document.
    /// </summary>
    public string Version
    {
        get => document.Root!.Attribute(AttributeVersion)!.Value;
        set => document.Root!.SetAttributeValue(AttributeVersion, value);
    }

    /// <summary>
    /// Gets or sets the XLIFF dialect used by this document.
    /// </summary>
    public XlfDialect Dialect { get; set; }

    /// <summary>
    /// Adds a new file element to this XLIFF document.
    /// </summary>
    /// <param name="original">The original file path reference.</param>
    /// <param name="dataType">The data type of the file content (e.g., "xml", "html").</param>
    /// <param name="sourceLanguage">The source language code.</param>
    /// <returns>The newly created <see cref="XlfFile"/> instance.</returns>
    public XlfFile AddFile(string original, string dataType, string sourceLanguage)
    {
        var xmlNamespace = document.Root!.Name.Namespace;
        var fileElement = new XElement(xmlNamespace + ElementFile);
        document.Descendants(xmlNamespace + ElementFile).Last().AddAfterSelf(fileElement);
        return new XlfFile(fileElement, xmlNamespace, original, dataType, sourceLanguage);
    }

    /// <summary>
    /// Removes a file element with the specified original attribute value.
    /// </summary>
    /// <param name="original">The original attribute value identifying the file to remove.</param>
    public void RemoveFile(string original)
    {
        var xmlNamespace = document.Root!.Name.Namespace;
        document.Descendants(xmlNamespace + ElementFile).Where(element =>
        {
            var attribute = element.Attribute(AttributeOriginal);
            return attribute != null && attribute.Value == original;
        }).Remove();
    }

    /// <summary>
    /// Saves this XLIFF document as a ResX file without any special options.
    /// </summary>
    /// <param name="filePath">The output file path for the ResX file.</param>
    public void SaveAsResX(string filePath)
    {
        SaveAsResX(filePath, ResXSaveOption.None);
    }

    /// <summary>
    /// Saves this XLIFF document as a ResX file with the specified options.
    /// </summary>
    /// <param name="filePath">The output file path for the ResX file.</param>
    /// <param name="options">Options controlling the save behavior.</param>
    public void SaveAsResX(string filePath, ResXSaveOption options)
    {
        var entries = new List<ResXEntry>();
        foreach (var file in Files)
        foreach (var transUnit in file.TransUnits)
        {
            var entry = new ResXEntry { Id = transUnit.GetId(Dialect), Value = transUnit.Target ?? string.Empty };
            if (options.HasFlag(ResXSaveOption.IncludeComments) && transUnit.Optional.Notes.Count() > 0)
                entry.Comment = transUnit.Optional.Notes.First().Value;
            entries.Add(entry);
        }

        if (options.HasFlag(ResXSaveOption.SortEntries)) entries.Sort();
        ResXFile.Write(filePath, entries,
            options.HasFlag(ResXSaveOption.IncludeComments) ? ResXOption.None : ResXOption.SkipComments);
    }

    /// <summary>
    /// Updates translation data from the associated source file using default state strings based on the XLIFF version.
    /// </summary>
    /// <returns>An <see cref="UpdateResult"/> containing the identifiers of added, removed, and updated items.</returns>
    public UpdateResult UpdateFromSource()
    {
        switch (Version)
        {
            default:
            case "1.1":
            case "1.2":
                return UpdateFromSource("new", "new");
            case "2.0":
                return UpdateFromSource("initial", "initial");
        }
    }

    /// <summary>
    /// Updates translation data from the associated source file using the specified state strings.
    /// </summary>
    /// <param name="updatedResourceStateString">The state string to assign to updated items.</param>
    /// <param name="addedResourceStateString">The state string to assign to added items.</param>
    /// <returns>An <see cref="UpdateResult"/> containing the identifiers of added, removed, and updated items.</returns>
    public UpdateResult UpdateFromSource(string updatedResourceStateString, string addedResourceStateString)
    {
        var sourceFilePath = Path.Combine(Path.GetDirectoryName(FileName)!, Files.Single().Original);
        return Update(sourceFilePath, updatedResourceStateString, addedResourceStateString);
    }

    /// <summary>
    /// Updates the XLIFF data from the provided ResX source file.
    /// </summary>
    /// <param name="sourceFilePath">The path to the source ResX file.</param>
    /// <param name="updatedResourceStateString">The state string to assign to updated items.</param>
    /// <param name="addedResourceStateString">The state string to assign to added items.</param>
    /// <returns>An <see cref="UpdateResult"/> containing the identifiers of added, removed, and updated items.</returns>
    public UpdateResult Update(string sourceFilePath, string updatedResourceStateString,
        string addedResourceStateString)
    {
        var resxData = new Dictionary<string, ResXEntry>();
        foreach (var entry in ResXFile.Read(sourceFilePath)) resxData.Add(entry.Id, entry);
        var updatedItems = new List<string>();
        var addedItems = new List<string>();
        var removedItems = new List<string>();
        foreach (var file in Files)
        {
            foreach (var transUnit in file.TransUnits)
            {
                var key = transUnit.GetId(Dialect);
                if (resxData.ContainsKey(key))
                {
                    if (XmlUtil.NormalizeLineBreaks(transUnit.Source) != XmlUtil.NormalizeLineBreaks(resxData[key].Value))
                    {
                        transUnit.Source = resxData[key].Value;
                        transUnit.Optional.TargetState = updatedResourceStateString;
                        transUnit.Optional.SetCommentFromResx(resxData[key].Comment);
                        updatedItems.Add(key);
                    }
                }
                else
                {
                    removedItems.Add(key);
                }

                resxData.Remove(key);
            }

            foreach (var id in removedItems) file.RemoveTransUnit(id, Dialect);
            foreach (var resxEntry in resxData)
            {
                var unit = file.AddTransUnit(resxEntry.Key, resxEntry.Value.Value, resxEntry.Value.Value, XlfFile.AddMode.FailIfExists,
                    Dialect);
                unit.Optional.TargetState = addedResourceStateString;
                unit.Optional.SetCommentFromResx(resxEntry.Value.Comment);
                addedItems.Add(resxEntry.Key);
            }
        }

        return new UpdateResult(addedItems, removedItems, updatedItems);
    }

    private XlfDialect DetermineDialect()
    {
        return Files.First().Optional.ToolId == "MultilingualAppToolkit"
            ? XlfDialect.MultilingualAppToolkit
            : document.Root!.GetNamespaceOfPrefix("rwt") == "http://www.schaudin.com/xmlns/rwt11"
                ? XlfDialect.RCWinTrans11
                : XlfDialect.Standard;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XlfDocument"/> class from the specified file.
    /// If the file name is null, <see cref="LoadXml(string)"/> must be called before using the document.
    /// </summary>
    /// <param name="filePath">The path to the XLIFF file to load, or null to create an unloaded document.</param>
    public XlfDocument(string filePath)
    {
        FileName = filePath;
        if (FileName != null)
        {
            document = XDocument.Load(FileName);
            Dialect = DetermineDialect();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XlfDocument"/> class without loading any file.
    /// Call <see cref="LoadXml(string)"/> to load XML content.
    /// </summary>
    public XlfDocument()
    {
    }

    /// <summary>
    /// Loads XLIFF content from the specified XML string.
    /// </summary>
    /// <param name="xml">The XML string to load.</param>
    public void LoadXml(string xml)
    {
        var xmlBytes = Encoding.UTF8.GetBytes(xml);
        LoadXml(xmlBytes);
    }

    /// <summary>
    /// Loads XLIFF content from the specified byte array.
    /// </summary>
    /// <param name="xmlBytes">The byte array containing the XML content.</param>
    public void LoadXml(byte[] xmlBytes)
    {
        using (var xmlStream = new MemoryStream(xmlBytes))
        {
            document = XDocument.Load(xmlStream);
        }

        Dialect = DetermineDialect();
    }

    /// <summary>
    /// Saves the document to the file specified by <see cref="FileName"/>.
    /// </summary>
    public void Save()
    {
        if (FileName != null)
            document!.Save(FileName);
        else
            ThrowEx.IsNull("FileName");
    }
}
