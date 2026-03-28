namespace SunamoXliffParser;

/// <summary>
/// Represents a translation unit (trans-unit) element in an XLIFF document.
/// </summary>
public class XlfTransUnit
{
    /// <summary>
    /// The prefix used for resource identifiers in the Multilingual App Toolkit dialect.
    /// </summary>
    public const string ResxPrefix = "Resx/";

    private const string AttributeId = "id";
    private const string ElementSource = "source";
    private const string ElementTarget = "target";
    private readonly XElement node;
    private readonly XNamespace xmlNamespace;

    /// <summary>
    /// Initializes a new instance of the <see cref="XlfTransUnit"/> class from an existing XML element.
    /// </summary>
    /// <param name="node">The XML element representing the translation unit.</param>
    /// <param name="xmlNamespace">The XML namespace of the XLIFF document.</param>
    public XlfTransUnit(XElement node, XNamespace xmlNamespace)
    {
        this.node = node;
        this.xmlNamespace = xmlNamespace;

        Optional = new Optionals(this.node, this.xmlNamespace);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XlfTransUnit"/> class with specified values.
    /// </summary>
    /// <param name="node">The XML element representing the translation unit.</param>
    /// <param name="xmlNamespace">The XML namespace of the XLIFF document.</param>
    /// <param name="id">The identifier of the translation unit.</param>
    /// <param name="source">The source text.</param>
    /// <param name="target">The target text.</param>
    public XlfTransUnit(XElement node, XNamespace xmlNamespace, string id, string source, string target)
        : this(node, xmlNamespace)
    {
        Id = id;
        Source = source;

        if (!string.IsNullOrWhiteSpace(target)) Target = target;
    }

    /// <summary>
    /// Gets the identifier of this translation unit.
    /// </summary>
    public string Id
    {
        get => node.Attribute(AttributeId)!.Value;
        private set => node.SetAttributeValue(AttributeId, value);
    }

    /// <summary>
    /// Gets the optional attributes of this translation unit.
    /// </summary>
    public Optionals Optional { get; }

    /// <summary>
    /// Gets or sets the source text of this translation unit.
    /// </summary>
    public string Source
    {
        get => node.Element(xmlNamespace + ElementSource)!.Value;
        set => node.SetElementValue(xmlNamespace + ElementSource, value);
    }

    /// <summary>
    /// Gets or sets the value of the target element. May be null if the element does not exist.
    /// Allowed are zero or one target elements.
    /// </summary>
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

    /// <summary>
    /// Gets the effective identifier for this translation unit based on the specified dialect.
    /// </summary>
    /// <param name="dialect">The XLIFF dialect to use for identifier resolution.</param>
    /// <returns>The resolved identifier string.</returns>
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

    /// <summary>
    /// Provides access to optional attributes of a translation unit element.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="Optionals"/> class.
        /// </summary>
        /// <param name="node">The XML element containing optional attributes.</param>
        /// <param name="xmlNamespace">The XML namespace of the XLIFF document.</param>
        public Optionals(XElement node, XNamespace xmlNamespace)
        {
            this.node = node;
            this.xmlNamespace = xmlNamespace;
        }

        /// <summary>
        /// Gets or sets the approved attribute which indicates whether a translation is final or has passed its final review.
        /// </summary>
        public string Approved
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeApproved);
            set => node.SetAttributeValue(AttributeApproved, value);
        }

        /// <summary>
        /// Gets or sets the datatype attribute specifying the kind of text contained in the element.
        /// </summary>
        public string DataType
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeDataType);
            set => node.SetAttributeValue(AttributeDataType, value);
        }

        /// <summary>
        /// Gets the collection of note elements associated with this translation unit.
        /// </summary>
        public IEnumerable<XlfNote> Notes => node.Descendants(xmlNamespace + ElementNote).Select(noteElement => new XlfNote(noteElement));

        /// <summary>
        /// Gets or sets the resname attribute which is the resource name or identifier of an item.
        /// </summary>
        public string Resname
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeResName);
            set => node.SetAttributeValue(AttributeResName, value);
        }

        /// <summary>
        /// Gets or sets the restype attribute which indicates the resource type of the container element.
        /// </summary>
        public string Restype
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeResType);
            set => node.SetAttributeValue(AttributeResType, value);
        }

        /// <summary>
        /// Gets or sets the status of a particular translation in a target or bin-target element.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the translate attribute which indicates whether or not the text should be translated.
        /// </summary>
        public string Translate
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeTranslate);
            set => node.SetAttributeValue(AttributeTranslate, value);
        }

        /// <summary>
        /// Adds a note with the specified comment and optional author.
        /// </summary>
        /// <param name="comment">The text content of the note.</param>
        /// <param name="from">The author of the note.</param>
        public void AddNote(string comment, string from)
        {
            var note = new XlfNote(new XElement(xmlNamespace + ElementNote, comment));
            if (!string.IsNullOrWhiteSpace(from)) note.Optional.From = from;

            node.Add(note.GetNode());
        }

        /// <summary>
        /// Adds a note with the specified comment.
        /// </summary>
        /// <param name="comment">The text content of the note.</param>
        public void AddNote(string comment)
        {
            AddNote(comment, string.Empty);
        }

        /// <summary>
        /// Sets or updates the first note from a ResX comment, or adds a new note if none exists.
        /// </summary>
        /// <param name="comment">The comment text from the ResX source.</param>
        public void SetCommentFromResx(string comment)
        {
            if (Notes.Any())
                Notes.First().Value = comment;
            else
                AddNote(comment);
        }

        /// <summary>
        /// Removes all notes that have the specified attribute with the specified value.
        /// </summary>
        /// <param name="attributeName">The name of the attribute to match.</param>
        /// <param name="value">The value of the attribute to match.</param>
        public void RemoveNotes(string attributeName, string value)
        {
            node.Descendants(xmlNamespace + ElementNote).Where(noteElement =>
            {
                var attribute = noteElement.Attribute(attributeName);
                return attribute != null && attribute.Value == value;
            }).Remove();
        }

        /// <summary>
        /// Returns the XML string representation of this translation unit's optional attributes.
        /// </summary>
        /// <returns>The XML string of the node.</returns>
        public override string ToString()
        {
            return node.ToString();
        }
    }
}
