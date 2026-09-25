namespace SunamoXliffParser;

public class XlfFile
{
    public enum AddMode
    {
        SkipExisting = 0,
        UpdateExisting = 1,
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

    public XlfFile(XElement node, XNamespace xmlNamespace)
    {
        this.node = node;
        this.xmlNamespace = xmlNamespace;
        Optional = new Optionals(node);
        if (node.Elements(xmlNamespace + ElementHeader).Any()) Header = new XlfHeader(node.Element(xmlNamespace + ElementHeader)!);
    }

    public XlfFile(XElement node, XNamespace xmlNamespace, string original, string dataType, string sourceLanguage)
        : this(node, xmlNamespace)
    {
        Original = original;
        DataType = dataType;
        SourceLang = sourceLanguage;
    }

    public string DataType
    {
        get => node.Attribute(AttributeDataType)!.Value;
        private set => node.SetAttributeValue(AttributeDataType, value);
    }

    public XlfHeader? Header { get; private set; }

    public Optionals Optional { get; }

    public string Original
    {
        get => node.Attribute(AttributeOriginal)!.Value;
        private set => node.SetAttributeValue(AttributeOriginal, value);
    }

    public string SourceLang
    {
        get => node.Attribute(AttributeSourceLanguage)!.Value;
        private set => node.SetAttributeValue(AttributeSourceLanguage, value);
    }

    public IEnumerable<XlfTransUnit> TransUnits =>
        node.Descendants(xmlNamespace + ElementTransUnit).Select(transUnitElement => new XlfTransUnit(transUnitElement, xmlNamespace));

    public XlfTransUnit AddOrUpdateTransUnit(string id, string source, string target, XlfDialect dialect)
    {
        return AddTransUnit(id, source, target, AddMode.UpdateExisting, dialect);
    }

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

    public XlfTransUnit GetTransUnit(string id, XlfDialect dialect)
    {
        return TransUnits.First(transUnit => transUnit.GetId(dialect) == id);
    }

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

    public void RemoveTransUnit(string attributeName, string attributeValue)
    {
        node.Descendants(xmlNamespace + ElementTransUnit).Where(element =>
        {
            var attribute = element.Attribute(attributeName);
            return attribute != null && attribute.Value == attributeValue;
        }).Remove();
    }

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

    public class Optionals
    {
        private const string AttributeBuildNum = "build-num";
        private const string AttributeProductName = "product-name";
        private const string AttributeProductVersion = "product-version";
        private const string AttributeTargetLanguage = "target-language";
        private const string AttributeToolId = "tool-id";
        private readonly XElement node;

        public Optionals(XElement node)
        {
            this.node = node;
        }

        public string BuildNum
        {
            get => GetAttributeIfExists(AttributeBuildNum);
            set => node.SetAttributeValue(AttributeBuildNum, value);
        }

        public string ProductName
        {
            get => GetAttributeIfExists(AttributeProductName);
            set => node.SetAttributeValue(AttributeProductName, value);
        }

        public string ProductVersion
        {
            get => GetAttributeIfExists(AttributeProductVersion);
            set => node.SetAttributeValue(AttributeProductVersion, value);
        }

        public string TargetLang
        {
            get => GetAttributeIfExists(AttributeTargetLanguage);
            set => node.SetAttributeValue(AttributeTargetLanguage, value);
        }

        public string ToolId
        {
            get => GetAttributeIfExists(AttributeToolId);
            set => node.SetAttributeValue(AttributeToolId, value);
        }

        public string GetAttributeIfExists(string name) => XmlUtil.GetAttributeIfExists(node, name);
    }
}
