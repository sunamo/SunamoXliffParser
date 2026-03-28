namespace SunamoXliffParser;

/// <summary>
/// Represents a note element in an XLIFF document used to add localization-related comments.
/// The content may be instructions from developers, comments from translators,
/// or any comment from anyone involved in processing the XLIFF file.
/// </summary>
public class XlfNote
{
    private readonly XElement node;

    /// <summary>
    /// Initializes a new instance of the <see cref="XlfNote"/> class.
    /// </summary>
    /// <param name="node">The XML element representing the note.</param>
    public XlfNote(XElement node)
    {
        this.node = node;
        Optional = new Optionals(this.node);
    }

    /// <summary>
    /// Gets the optional attributes of this note.
    /// </summary>
    public Optionals Optional { get; }

    /// <summary>
    /// Gets or sets the text content of the note.
    /// </summary>
    public string Value
    {
        get => node.Value;
        set => node.Value = value;
    }

    /// <summary>
    /// Gets the underlying XML element of this note.
    /// </summary>
    /// <returns>The XML element representing this note.</returns>
    public XElement GetNode()
    {
        return node;
    }

    /// <summary>
    /// Provides access to optional attributes of a note element.
    /// </summary>
    public class Optionals
    {
        private const string AttributeAnnotates = "annotates";
        private const string AttributeFrom = "from";
        private const string AttributePriority = "priority";
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
        /// Gets or sets the annotates attribute indicating if the note is general
        /// or pertains specifically to the source or the target element.
        /// </summary>
        public string Annotates
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeAnnotates);
            set => node.SetAttributeValue(AttributeAnnotates, value);
        }

        /// <summary>
        /// Gets or sets the from attribute indicating who entered the note.
        /// </summary>
        public string From
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeFrom);
            set => node.SetAttributeValue(AttributeFrom, value);
        }

        /// <summary>
        /// Gets or sets the language of the note content.
        /// </summary>
        public string Lang
        {
            get => XmlUtil.GetAttributeIfExists(node, "xml:lang");
            set => node.SetAttributeValue("xml:lang", value);
        }

        /// <summary>
        /// Gets or sets the priority from 1 (high) to 10 (low) assigned to the note.
        /// </summary>
        public int Priority
        {
            get => XmlUtil.GetIntAttributeIfExists(node, AttributePriority);
            set => node.SetAttributeValue(AttributePriority, value);
        }
    }
}
