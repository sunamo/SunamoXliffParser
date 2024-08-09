namespace SunamoXliffParser.fmdev.ResX;

public static class ResXFile
{
    private static readonly Type type = typeof(ResXFile);

    public static List<ResXEntry> Read(string filename, ResXOption options = ResXOption.None)
    {
        var result = new List<ResXEntry>();
        using (var resx = new ResXResourceReader(filename))
        {
            resx.UseResXDataNodes = true;
            var dict = resx.GetEnumerator();
            while (dict.MoveNext())
            {
                var node = dict.Value as ResXDataNode;
                var comment = options.HasFlag(ResXOption.SkipComments)
                    ? string.Empty
                    : node.Comment.Replace("\r", string.Empty);
                result.Add(new ResXEntry
                {
                    Id = dict.Key as string,
                    Value = (node.GetValue((ITypeResolutionService)null) as string).Replace("\r", string.Empty),
                    Comment = comment
                });
            }

            resx.Close();
        }

        return result;
    }

    public static void Write(string filename, IEnumerable<ResXEntry> entries, ResXOption options = ResXOption.None)
    {
        using (var resx = new ResXResourceWriter(filename))
        {
            foreach (var entry in entries)
            {
                var node = new ResXDataNode(entry.Id,
                    entry.Value.Replace("\r", string.Empty).Replace("\n", Environment.NewLine));
                if (!options.HasFlag(ResXOption.SkipComments) && !string.IsNullOrWhiteSpace(entry.Comment))
                    node.Comment = entry.Comment.Replace("\r", string.Empty).Replace("\n", Environment.NewLine);

                resx.AddResource(node);
            }

            resx.Close();
        }
    }

    /// <summary>
    ///     Generates a public C# designer class.
    /// </summary>
    /// <param name="resXFile">The source resx file.</param>
    /// <param name="className">The base class name.</param>
    /// <param name="namespaceName">The namespace for the generated code.</param>
    /// <returns>false if generation of at least one property couldn't be generated.</returns>
    public static bool GenerateDesignerFile(string resXFile, string className, string namespaceName)
    {
        return GenerateDesignerFile(resXFile, className, namespaceName, false);
    }

    /// <summary>
    ///     Generates a C# designer class.
    /// </summary>
    /// <param name="resXFile">The source resx file.</param>
    /// <param name="className">The base class name.</param>
    /// <param name="namespaceName">The namespace for the generated code.</param>
    /// <param name="publicClass">Specifies if the class has public or public access level.</param>
    /// <returns>false if generation of at least one property failed.</returns>
    public static bool GenerateDesignerFile(string resXFile, string className, string namespaceName,
        bool publicClass)
    {
        // It is absolutely nonsense coz GenerateDesignerFile is just calling by another GenerateDesignerFile and both have no more references

        return false;

        //if (!File.Exists(resXFile))
        //{
        //    throw new Exception($"The file '{resXFile}' could not be found");
        //}
        //if (string.IsNullOrEmpty(className))
        //{
        //    throw new Exception($"The class name must not be empty or null");
        //}
        //if (string.IsNullOrEmpty(namespaceName))
        //{
        //    throw new Exception($"The namespace name must not be empty or null");
        //}
        //string[] unmatchedElements;
        ////System.Resources.Tools.
        //var codeProvider = new Microsoft.CSharp.CSharpCodeProvider();
        //System.CodeDom.CodeCompileUnit code =
        //    StronglyTypedResourceBuilder.Create(
        //        resXFile,
        //        className,
        //        namespaceName,
        //        codeProvider,
        //        publicClass,
        //        out unmatchedElements);
        //var designerFileName = Path.Combine(Path.GetDirectoryName(resXFile), $"{className}.Designer.cs");
        //using (StreamWriter writer = new StreamWriter(designerFileName, false, System.Text.Encoding.UTF8))
        //{
        //    codeProvider.GenerateCodeFromCompileUnit(code, writer, new System.CodeDom.Compiler.CodeGeneratorOptions());
        //}
        //return unmatchedElements.Length == 0;
    }
}