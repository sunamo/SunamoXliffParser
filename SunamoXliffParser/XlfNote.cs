namespace SunamoXliffParser;

public class XlfNote
{
    private readonly XElement node;

    public XlfNote(XElement node)
    {
        this.node = node;
        Optional = new Optionals(this.node);
    }

    public Optionals Optional { get; }

    public string Value
    {
        get => node.Value;
        set => node.Value = value;
    }

    public XElement GetNode() => node;

    public class Optionals
    {
        private const string AttributeAnnotates = "annotates";
        private const string AttributeFrom = "from";
        private const string AttributePriority = "priority";
        private readonly XElement node;

        public Optionals(XElement node)
        {
            this.node = node;
        }

        public string Annotates
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeAnnotates);
            set => node.SetAttributeValue(AttributeAnnotates, value);
        }

        public string From
        {
            get => XmlUtil.GetAttributeIfExists(node, AttributeFrom);
            set => node.SetAttributeValue(AttributeFrom, value);
        }

        public string Lang
        {
            get => XmlUtil.GetAttributeIfExists(node, "xml:lang");
            set => node.SetAttributeValue("xml:lang", value);
        }

        public int Priority
        {
            get => XmlUtil.GetIntAttributeIfExists(node, AttributePriority);
            set => node.SetAttributeValue(AttributePriority, value);
        }
    }
}
