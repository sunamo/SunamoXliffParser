namespace SunamoXliffParser;

/// <summary>
/// Represents a file element within an XLIFF document.
/// </summary>
public class XlfFile
{
    /// <summary>
    /// Specifies the behavior when adding a translation unit that already exists.
    /// </summary>
    public enum AddMode
    {
        /// <summary>
        /// Skip the operation if a translation unit with the same identifier already exists.
        /// </summary>
        SkipExisting = 0,

        /// <summary>
        /// Update the existing translation unit with the new source and target values.
        /// </summary>
        UpdateExisting = 1,

        /// <summary>
        /// Throw an exception if a translation unit with the same identifier already exists.
        /// </summary>
        FailIfExists = 2
    }

    private const string ElementHeader = "header";
    private const string AttributeDataType = "datatype";
    private const string AttributeOriginal = "original";
    private const string AttributeSourceLanguage = "source-language";
    private const string ElementTransUnit = "trans-unit";
    private const string ElementBody = "body";
    private const string IdNone = "none";
    private const string AttributeId = "id";
    private const string AttributeResname = "resname";
    private readonly XElement node;
    private readonly XNamespace xmlNamespace;

    /// <summary>
    /// Initializes a new instance of the <see cref="XlfFile"/> class from an existing XML element.
    /// </summary>
    /// <param name="node">The XML element representing the file.</param>
    /// <param name="xmlNamespace">The XML namespace of the XLIFF document.</param>
    public XlfFile(XElement node, XNamespace xmlNamespace)
    {
        this.node = node;
        this.xmlNamespace = xmlNamespace;
        Optional = new Optionals(node);
        if (node.Elements(xmlNamespace + ElementHeader).Any()) Header = new XlfHeader(node.Element(xmlNamespace + ElementHeader)!);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XlfFile"/> class with specified attribute values.
    /// </summary>
    /// <param name="node">The XML element representing the file.</param>
    /// <param name="xmlNamespace">The XML namespace of the XLIFF document.</param>
    /// <param name="original">The original file path reference.</param>
    /// <param name="dataType">The data type of the file content.</param>
    /// <param name="sourceLanguage">The source language code.</param>
    public XlfFile(XElement node, XNamespace xmlNamespace, string original, string dataType, string sourceLanguage)
        : this(node, xmlNamespace)
    {
        Original = original;
        DataType = dataType;
        SourceLang = sourceLanguage;
    }

    /// <summary>
    /// Gets or sets the data type of the file content (e.g., xml, html).
    /// </summary>
    public string DataType
    {
        get => node.Attribute(AttributeDataType)!.Value;
        private set => node.SetAttributeValue(AttributeDataType, value);
    }

    /// <summary>
    /// Gets the header element of this file, if present.
    /// </summary>
    public XlfHeader? Header { get; private set; }

    /// <summary>
    /// Gets the optional attributes of this file element.
    /// </summary>
    public Optionals Optional { get; }

    /// <summary>
    /// Gets or sets the original file path reference.
    /// </summary>
    public string Original
    {
        get => node.Attribute(AttributeOriginal)!.Value;
        private set => node.SetAttributeValue(AttributeOriginal, value);
    }

    /// <summary>
    /// Gets or sets the source language code.
    /// </summary>
    public string SourceLang
    {
        get => node.Attribute(AttributeSourceLanguage)!.Value;
        private set => node.SetAttributeValue(AttributeSourceLanguage, value);
    }

    /// <summary>
    /// Gets all translation units contained in this file element.
    /// </summary>
    public IEnumerable<XlfTransUnit> TransUnits =>
        node.Descendants(xmlNamespace + ElementTransUnit).Select(transUnitElement => new XlfTransUnit(transUnitElement, xmlNamespace));

    /// <summary>
    /// Adds a new translation unit or updates an existing one.
    /// </summary>
    /// <param name="id">The identifier of the translation unit.</param>
    /// <param name="source">The source text.</param>
    /// <param name="target">The target text.</param>
    /// <param name="dialect">The XLIFF dialect to use for identifier resolution.</param>
    /// <returns>The added or updated <see cref="XlfTransUnit"/>.</returns>
    public XlfTransUnit AddOrUpdateTransUnit(string id, string source, string target, XlfDialect dialect)
    {
        return AddTransUnit(id, source, target, AddMode.UpdateExisting, dialect);
    }

    /// <summary>
    /// Adds a translation unit with the specified behavior for existing units.
    /// </summary>
    /// <param name="id">The identifier of the translation unit.</param>
    /// <param name="source">The source text.</param>
    /// <param name="target">The target text.</param>
    /// <param name="addMode">The behavior when a unit with the same identifier already exists.</param>
    /// <param name="dialect">The XLIFF dialect to use for identifier resolution.</param>
    /// <returns>The added or existing <see cref="XlfTransUnit"/>.</returns>
    public XlfTransUnit AddTransUnit(string id, string source, string target, AddMode addMode, XlfDialect dialect)
    {
        if (TryGetTransUnit(id, dialect, out var resultUnit) && resultUnit != null)
            switch (addMode)
            {
                case AddMode.FailIfExists:
                    throw new Exception($"There is already a trans-unit with id={id}");
                case AddMode.SkipExisting:
                    return resultUnit;
                default:
                case AddMode.UpdateExisting:
                    resultUnit.Source = source;
                    if (resultUnit.Target != null) resultUnit.Target = target;
                    return resultUnit;
            }

        var transUnitElement = new XElement(xmlNamespace + ElementTransUnit);
        var existingTransUnits = node.Descendants(xmlNamespace + ElementTransUnit).ToList();
        if (existingTransUnits.Any())
        {
            existingTransUnits.Last().AddAfterSelf(transUnitElement);
        }
        else
        {
            var bodyElements = node.Descendants(xmlNamespace + ElementBody).ToList();
            XElement body;
            if (bodyElements.Any())
            {
                body = bodyElements.First();
            }
            else
            {
                body = new XElement(xmlNamespace + ElementBody);
                node.Add(body);
            }

            body.Add(transUnitElement);
        }

        if (dialect == XlfDialect.RCWinTrans11)
        {
            var unit = new XlfTransUnit(transUnitElement, xmlNamespace, IdNone, source, target);
            unit.Optional.Resname = id;
            return unit;
        }

        if (dialect == XlfDialect.MultilingualAppToolkit)
            if (!id.StartsWith(XlfTransUnit.ResxPrefix, StringComparison.InvariantCultureIgnoreCase))
                return new XlfTransUnit(transUnitElement, xmlNamespace, XlfTransUnit.ResxPrefix + id, source, target);
        return new XlfTransUnit(transUnitElement, xmlNamespace, id, source, target);
    }

    /// <summary>
    /// Gets a translation unit by its identifier and dialect.
    /// </summary>
    /// <param name="id">The identifier of the translation unit.</param>
    /// <param name="dialect">The XLIFF dialect to use for identifier resolution.</param>
    /// <returns>The matching <see cref="XlfTransUnit"/>.</returns>
    public XlfTransUnit GetTransUnit(string id, XlfDialect dialect)
    {
        return TransUnits.First(transUnit => transUnit.GetId(dialect) == id);
    }

    /// <summary>
    /// Attempts to get a translation unit by its identifier and dialect.
    /// </summary>
    /// <param name="id">The identifier of the translation unit.</param>
    /// <param name="dialect">The XLIFF dialect to use for identifier resolution.</param>
    /// <param name="unit">When this method returns, contains the matching translation unit, or null if not found.</param>
    /// <returns>True if the translation unit was found; otherwise, false.</returns>
    public bool TryGetTransUnit(string id, XlfDialect dialect, out XlfTransUnit? unit)
    {
        try
        {
            unit = GetTransUnit(id, dialect);
            return true;
        }
        catch (InvalidOperationException)
        {
            unit = null;
            return false;
        }
        catch (NullReferenceException)
        {
            unit = null;
            return false;
        }
    }

    /// <summary>
    /// Removes a translation unit by its identifier and dialect.
    /// </summary>
    /// <param name="id">The identifier of the translation unit to remove.</param>
    /// <param name="dialect">The XLIFF dialect to use for identifier resolution.</param>
    public void RemoveTransUnit(string id, XlfDialect dialect)
    {
        switch (dialect)
        {
            case XlfDialect.RCWinTrans11:
                RemoveTransUnit(AttributeResname, id);
                break;
            case XlfDialect.MultilingualAppToolkit:
                RemoveTransUnit(AttributeId, XlfTransUnit.ResxPrefix + id);
                break;
            default:
                RemoveTransUnit(AttributeId, id);
                break;
        }
    }

    /// <summary>
    /// Removes translation units matching the specified attribute name and value.
    /// </summary>
    /// <param name="attributeName">The name of the attribute to match.</param>
    /// <param name="attributeValue">The value of the attribute to match.</param>
    public void RemoveTransUnit(string attributeName, string attributeValue)
    {
        node.Descendants(xmlNamespace + ElementTransUnit).Where(element =>
        {
            var attribute = element.Attribute(attributeName);
            return attribute != null && attribute.Value == attributeValue;
        }).Remove();
    }

    /// <summary>
    /// Exports filtered translation units using the specified exporter.
    /// </summary>
    /// <param name="outputFilePath">The path for the exported output file.</param>
    /// <param name="exporter">The exporter implementation to use.</param>
    /// <param name="stateFilter">A list of target states to include, or null/empty to include all.</param>
    /// <param name="resTypeFilter">A list of resource types to include, or null/empty to include all.</param>
    /// <param name="dialect">The XLIFF dialect to use during export.</param>
    public void Export(string outputFilePath, IXlfExporter exporter, List<string> stateFilter,
        List<string> resTypeFilter, XlfDialect dialect)
    {
        var units = stateFilter != null && stateFilter.Any()
            ? TransUnits.Where(transUnit => transUnit.Optional.TargetState != null && stateFilter.Contains(transUnit.Optional.TargetState))
            : TransUnits;
        units = resTypeFilter != null && resTypeFilter.Any()
            ? units.Where(transUnit => resTypeFilter.Contains(transUnit.Optional.Restype))
            : units;
        exporter.ExportTranslationUnits(outputFilePath, units, Optional.TargetLang, dialect);
    }

    /// <summary>
    /// Provides access to optional attributes of a file element.
    /// </summary>
    public class Optionals
    {
        private const string AttributeBuildNum = "build-num";
        private const string AttributeProductName = "product-name";
        private const string AttributeProductVersion = "product-version";
        private const string AttributeTargetLanguage = "target-language";
        private const string AttributeToolId = "tool-id";
        private readonly XElement node;

        /// <summary>
        /// Initializes a new instance of the <see cref="Optionals"/> class.
        /// </summary>
        /// <param name="node">The XML element containing optional attributes.</param>
        public Optionals(XElement node)
        {
            this.node = node;
        }

        /// <summary>
        /// Gets or sets the build number.
        /// </summary>
        public string BuildNum
        {
            get => GetAttributeIfExists(AttributeBuildNum);
            set => node.SetAttributeValue(AttributeBuildNum, value);
        }

        /// <summary>
        /// Gets or sets the product name.
        /// </summary>
        public string ProductName
        {
            get => GetAttributeIfExists(AttributeProductName);
            set => node.SetAttributeValue(AttributeProductName, value);
        }

        /// <summary>
        /// Gets or sets the product version.
        /// </summary>
        public string ProductVersion
        {
            get => GetAttributeIfExists(AttributeProductVersion);
            set => node.SetAttributeValue(AttributeProductVersion, value);
        }

        /// <summary>
        /// Gets or sets the target language code.
        /// </summary>
        public string TargetLang
        {
            get => GetAttributeIfExists(AttributeTargetLanguage);
            set => node.SetAttributeValue(AttributeTargetLanguage, value);
        }

        /// <summary>
        /// Gets or sets the tool identifier.
        /// </summary>
        public string ToolId
        {
            get => GetAttributeIfExists(AttributeToolId);
            set => node.SetAttributeValue(AttributeToolId, value);
        }

        /// <summary>
        /// Gets the value of the specified attribute if it exists on the file element.
        /// </summary>
        /// <param name="name">The name of the attribute to retrieve.</param>
        /// <returns>The attribute value, or an empty string if the attribute does not exist.</returns>
        public string GetAttributeIfExists(string name)
        {
            return XmlUtil.GetAttributeIfExists(node, name);
        }
    }
}
