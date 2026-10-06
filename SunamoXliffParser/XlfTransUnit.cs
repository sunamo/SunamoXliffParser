namespace SunamoXliffParser;

public class XlfTransUnit
{
    public const string ResxPrefix = "Resx/";

    private const string AttributeId = "id";
    private const string ElementSource = "source";
    private const string ElementTarget = "target";
    private readonly XElement node;
    private readonly XNamespace xmlNamespace;

    public XlfTransUnit(XElement node, XNamespace xmlNamespace)
    {
        this.node = node;
        this.xmlNamespace = xmlNamespace;

        Optional = new Optionals(this.node, this.xmlNamespace);
    }

    public XlfTransUnit(XElement node, XNamespace xmlNamespace, string id, string source, string target)
        : this(node, xmlNamespace)
    {
        Id = id;
        Source = source;

        if (!string.IsNullOrWhiteSpace(target)) Target = target;
    }

    public string Id
    {
        get => node.Attribute(AttributeId)!.Value;
        private set => node.SetAttributeValue(AttributeId, value);
    }

    public Optionals Optional { get; }

    public string Source
    {
        get => node.Element(xmlNamespace + ElementSource)!.Value;
        set => node.SetElementValue(xmlNamespace + ElementSource, value);
    }

    public string? Target
    {
        get
        {
            var targets = node.Elements(xmlNamespace + ElementTarget);
            return !targets.Any() ? null : targets.First().Value;
        }

        set
        {
            if (Target == null)
            {
                var targetNode = new XElement(xmlNamespace + ElementTarget, value);
                node.Element(xmlNamespace + ElementSource)!.AddAfterSelf(targetNode);
            }
            else
            {
                node.SetElementValue(xmlNamespace + ElementTarget, value);
            }
        }
    }

    public string GetId(XlfDialect dialect)
    {
        var id = Id;
        switch (dialect)
        {
            case XlfDialect.RCWinTrans11:
                id = Optional?.Resname ?? Id;
                break;

            case XlfDialect.MultilingualAppToolkit:
                if (Id.StartsWith(ResxPrefix, StringComparison.InvariantCultureIgnoreCase))
                    id = Id.Substring(ResxPrefix.Length);

                break;
        }

        return id;
    }

    public class Optionals
    {
        private const string AttributeApproved = "approved";
        private const string AttributeDataType = "datatype";
        private const string ElementNote = "note";
        private const string AttributeResName = "resname";
        private const string AttributeResType = "restype";
        private const string AttributeState = "state";
        private const string AttributeTranslate = "translate";
        private readonly XElement node;
        private readonly XNamespace xmlNamespace;

        public Optionals(XElement node, XNamespace xmlNamespace)
        {
            this.node = node;
            this.xmlNamespace = xmlNamespace;
        }

        public string Approved
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeApproved);
            set => node.SetAttributeValue(AttributeApproved, value);
        }

        public string DataType
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeDataType);
            set => node.SetAttributeValue(AttributeDataType, value);
        }

        public IEnumerable<XlfNote> Notes => node.Descendants(xmlNamespace + ElementNote).Select(noteElement => new XlfNote(noteElement));

        public string Resname
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeResName);
            set => node.SetAttributeValue(AttributeResName, value);
        }

        public string Restype
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeResType);
            set => node.SetAttributeValue(AttributeResType, value);
        }

        public string? TargetState
        {
            get => !node.Elements(xmlNamespace + ElementTarget).Any()
                ? null
                : XmlUtil.GetAttributeIfExists(node.Element(xmlNamespace + ElementTarget)!, AttributeState);

            set
            {
                if (node.Elements(xmlNamespace + ElementTarget).Any())
                    node.Element(xmlNamespace + ElementTarget)!.SetAttributeValue(AttributeState, value);
            }
        }

        public string Translate
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeTranslate);
            set => node.SetAttributeValue(AttributeTranslate, value);
        }

        public void AddNote(string comment, string from)
        {
            var note = new XlfNote(new XElement(xmlNamespace + ElementNote, comment));
            if (!string.IsNullOrWhiteSpace(from)) note.Optional.From = from;

            node.Add(note.GetNode());
        }

        public void AddNote(string comment)
        {
            AddNote(comment, string.Empty);
        }

        public void SetCommentFromResx(string comment)
        {
            if (Notes.Any())
                Notes.First().Value = comment;
            else
                AddNote(comment);
        }

        public void RemoveNotes(string attributeName, string value)
        {
            node.Descendants(xmlNamespace + ElementNote).Where(noteElement =>
            {
                var attribute = noteElement.Attribute(attributeName);
                return attribute != null && attribute.Value == value;
            }).Remove();
        }

        public override string ToString()
        {
            return node.ToString();
        }
    }
}
