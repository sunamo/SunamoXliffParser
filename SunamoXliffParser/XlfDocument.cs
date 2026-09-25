namespace SunamoXliffParser;

public class XlfDocument
{
    [Flags]
    public enum ResXSaveOption
    {
        None = 0,
        SortEntries = 1,
        IncludeComments = 2
    }

    private const string AttributeOriginal = "original";
    private const string ElementFile = "file";
    private const string AttributeVersion = "version";

    public static Type DocumentType { get; } = typeof(XlfDocument);

    private XDocument document = null!;

    public string? FileName { get; }

    public IEnumerable<XlfFile> Files
    {
        get
        {
            var xmlNamespace = document.Root!.Name.Namespace;
            return document.Descendants(xmlNamespace + ElementFile).Select(fileElement => new XlfFile(fileElement, xmlNamespace));
        }
    }

    public string Version
    {
        get => document.Root!.Attribute(AttributeVersion)!.Value;
        set => document.Root!.SetAttributeValue(AttributeVersion, value);
    }

    public XlfDialect Dialect { get; set; }

    public XlfFile AddFile(string original, string dataType, string sourceLanguage)
    {
        var xmlNamespace = document.Root!.Name.Namespace;
        var fileElement = new XElement(xmlNamespace + ElementFile);
        document.Descendants(xmlNamespace + ElementFile).Last().AddAfterSelf(fileElement);
        return new XlfFile(fileElement, xmlNamespace, original, dataType, sourceLanguage);
    }

    public void RemoveFile(string original)
    {
        var xmlNamespace = document.Root!.Name.Namespace;
        document.Descendants(xmlNamespace + ElementFile).Where(element =>
        {
            var attribute = element.Attribute(AttributeOriginal);
            return attribute != null && attribute.Value == original;
        }).Remove();
    }

    public void SaveAsResX(string filePath)
    {
        SaveAsResX(filePath, ResXSaveOption.None);
    }

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

    public UpdateResult UpdateFromSource(string updatedResourceStateString, string addedResourceStateString)
    {
        var sourceFilePath = Path.Combine(Path.GetDirectoryName(FileName)!, Files.Single().Original);
        return Update(sourceFilePath, updatedResourceStateString, addedResourceStateString);
    }

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

    public XlfDocument(string filePath)
    {
        FileName = filePath;
        if (FileName != null)
        {
            document = XDocument.Load(FileName);
            Dialect = DetermineDialect();
        }
    }

    public XlfDocument()
    {
    }

    public void LoadXml(string xml)
    {
        var xmlBytes = Encoding.UTF8.GetBytes(xml);
        LoadXml(xmlBytes);
    }

    public void LoadXml(byte[] xmlBytes)
    {
        using (var xmlStream = new MemoryStream(xmlBytes))
        {
            document = XDocument.Load(xmlStream);
        }

        Dialect = DetermineDialect();
    }

    public void Save()
    {
        if (FileName != null)
            document!.Save(FileName);
        else
            ThrowEx.IsNull("FileName");
    }
}
