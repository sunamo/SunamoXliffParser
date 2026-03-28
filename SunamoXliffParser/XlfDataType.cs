namespace SunamoXliffParser;

/// <summary>
/// Defines the data types for XLIFF file elements as specified in the XLIFF 1.2 standard.
/// User-defined datatypes are allowed but must begin with "x-".
/// See http://docs.oasis-open.org/xliff/v1.2/os/xliff-core.html#datatype
/// </summary>
public enum XlfDataType
{
#pragma warning disable SA1300 // Element must begin with upper-case letter
    /// <summary>
    /// Indicates Active Server Page data.
    /// </summary>
    asp,

    /// <summary>
    /// Indicates C source file data.
    /// </summary>
    c,

    /// <summary>
    /// Indicates Channel Definition Format (CDF) data.
    /// </summary>
    cdf,

    /// <summary>
    /// Indicates ColdFusion data.
    /// </summary>
    cfm,

    /// <summary>
    /// Indicates C++ source file data.
    /// </summary>
    cpp,

    /// <summary>
    /// Indicates C-Sharp data.
    /// </summary>
    csharp,

    /// <summary>
    /// Indicates strings from C, ASM, and driver files data.
    /// </summary>
    cstring,

    /// <summary>
    /// Indicates comma-separated values data.
    /// </summary>
    csv,

    /// <summary>
    /// Indicates database data.
    /// </summary>
    database,

    /// <summary>
    /// Indicates portions of document that follows data and contains metadata.
    /// </summary>
    documentfooter,

    /// <summary>
    /// Indicates portions of document that precedes data and contains metadata.
    /// </summary>
    documentheader,

    /// <summary>
    /// Indicates data from standard UI file operations dialogs (e.g., Open, Save, Save As, Export, Import).
    /// </summary>
    filedialog,

    /// <summary>
    /// Indicates standard user input screen data.
    /// </summary>
    form,

    /// <summary>
    /// Indicates HyperText Markup Language (HTML) data - document instance.
    /// </summary>
    html,

    /// <summary>
    /// Indicates content within an HTML document's body element.
    /// </summary>
    htmlbody,

    /// <summary>
    /// Indicates Windows INI file data.
    /// </summary>
    ini,

    /// <summary>
    /// Indicates Interleaf data.
    /// </summary>
    interleaf,

    /// <summary>
    /// Indicates Java source file data (extension '.java').
    /// </summary>
    javaclass,

    /// <summary>
    /// Indicates Java property resource bundle data.
    /// </summary>
    javapropertyresourcebundle,

    /// <summary>
    /// Indicates Java list resource bundle data.
    /// </summary>
    javalistresourcebundle,

    /// <summary>
    /// Indicates JavaScript source file data.
    /// </summary>
    javascript,

    /// <summary>
    /// Indicates JScript source file data.
    /// </summary>
    jscript,

    /// <summary>
    /// Indicates information relating to formatting.
    /// </summary>
    layout,

    /// <summary>
    /// Indicates LISP source file data.
    /// </summary>
    lisp,

    /// <summary>
    /// Indicates information relating to margin formats.
    /// </summary>
    margin,

    /// <summary>
    /// Indicates a file containing menu.
    /// </summary>
    menufile,

    /// <summary>
    /// Indicates numerically identified string table.
    /// </summary>
    messagefile,

    /// <summary>
    /// Indicates Maker Interchange Format (MIF) data.
    /// </summary>
    mif,

    /// <summary>
    /// Indicates that the datatype attribute value is a MIME Type value and is defined in the mime-type attribute.
    /// </summary>
    mimetype,

    /// <summary>
    /// Indicates GNU Machine Object data.
    /// </summary>
    mo,

    /// <summary>
    /// Indicates Message Librarian strings created by Novell's Message Librarian Tool.
    /// </summary>
    msglib,

    /// <summary>
    /// Indicates information to be displayed at the bottom of each page of a document.
    /// </summary>
    pagefooter,

    /// <summary>
    /// Indicates information to be displayed at the top of each page of a document.
    /// </summary>
    pageheader,

    /// <summary>
    /// Indicates a list of property values (e.g., settings within INI files or preferences dialog).
    /// </summary>
    parameters,

    /// <summary>
    /// Indicates Pascal source file data.
    /// </summary>
    pascal,

    /// <summary>
    /// Indicates Hypertext Preprocessor data.
    /// </summary>
    php,

    /// <summary>
    /// Indicates plain text file (no formatting other than, possibly, wrapping).
    /// </summary>
    plaintext,

    /// <summary>
    /// Indicates GNU Portable Object file.
    /// </summary>
    po,

    /// <summary>
    /// Indicates dynamically generated user defined document, e.g. Oracle Report, Crystal Report.
    /// </summary>
    report,

    /// <summary>
    /// Indicates Windows .NET binary resources.
    /// </summary>
    resources,

    /// <summary>
    /// Indicates Windows .NET Resources.
    /// </summary>
    resx,

    /// <summary>
    /// Indicates Rich Text Format (RTF) data.
    /// </summary>
    rtf,

    /// <summary>
    /// Indicates Standard Generalized Markup Language (SGML) data - document instance.
    /// </summary>
    sgml,

    /// <summary>
    /// Indicates Standard Generalized Markup Language (SGML) data - Document Type Definition (DTD).
    /// </summary>
    sgmldtd,

    /// <summary>
    /// Indicates Scalable Vector Graphic (SVG) data.
    /// </summary>
    svg,

    /// <summary>
    /// Indicates VisualBasic Script source file.
    /// </summary>
    vbscript,

    /// <summary>
    /// Indicates warning message.
    /// </summary>
    warning,

    /// <summary>
    /// Indicates Windows (Win32) resources extracted from an RC script, a message file, or a compiled file.
    /// </summary>
    winres,

    /// <summary>
    /// Indicates Extensible HyperText Markup Language (XHTML) data - document instance.
    /// </summary>
    xhtml,

    /// <summary>
    /// Indicates Extensible Markup Language (XML) data - document instance.
    /// </summary>
    xml,

    /// <summary>
    /// Indicates Extensible Markup Language (XML) data - Document Type Definition (DTD).
    /// </summary>
    xmldtd,

    /// <summary>
    /// Indicates Extensible Stylesheet Language (XSL) data.
    /// </summary>
    xsl,

    /// <summary>
    /// Indicates XUL elements.
    /// </summary>
    xul,
#pragma warning restore SA1300 // Element must begin with upper-case letter
}