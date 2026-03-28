namespace SunamoXliffParser._sunamo.SunamoExceptions;

/// <summary>
/// Provides helper methods for building exception messages and inspecting the call stack.
/// </summary>
internal sealed partial class Exceptions
{
    /// <summary>
    /// Prepends the specified prefix to an exception message if it is not empty.
    /// </summary>
    /// <param name="before">The prefix text to prepend.</param>
    /// <returns>The prefix followed by a colon and space, or an empty string if the prefix is empty.</returns>
    internal static string CheckBefore(string before)
    {
        return string.IsNullOrWhiteSpace(before) ? string.Empty : before + ": ";
    }

    /// <summary>
    /// Determines the type, method name, and stack trace of the calling code.
    /// </summary>
    /// <param name="shouldFillTypeAndMethod">Whether to fill the type and method name from the first non-ThrowEx frame.</param>
    /// <returns>A tuple containing the type name, method name, and full stack trace text.</returns>
    internal static Tuple<string, string, string> PlaceOfException(bool shouldFillTypeAndMethod = true)
    {
        StackTrace stackTrace = new();
        var stackTraceText = stackTrace.ToString();
        var lines = stackTraceText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
        lines.RemoveAt(0);
        var i = 0;
        string typeName = string.Empty;
        string methodName = string.Empty;
        for (; i < lines.Count; i++)
        {
            var line = lines[i];
            if (shouldFillTypeAndMethod)
                if (!line.StartsWith("   at ThrowEx"))
                {
                    TypeAndMethodName(line, out typeName, out methodName);
                    shouldFillTypeAndMethod = false;
                }
            if (line.StartsWith("at System."))
            {
                lines.Add(string.Empty);
                lines.Add(string.Empty);
                break;
            }
        }
        return new Tuple<string, string, string>(typeName, methodName, string.Join(Environment.NewLine, lines));
    }

    /// <summary>
    /// Extracts the type name and method name from a stack trace line.
    /// </summary>
    /// <param name="stackTraceLine">A single line from a stack trace.</param>
    /// <param name="typeName">The extracted type name.</param>
    /// <param name="methodName">The extracted method name.</param>
    internal static void TypeAndMethodName(string stackTraceLine, out string typeName, out string methodName)
    {
        var afterAtKeyword = stackTraceLine.Split("at ")[1].Trim();
        var fullMethodName = afterAtKeyword.Split("(")[0];
        var segments = fullMethodName.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        methodName = segments[^1];
        segments.RemoveAt(segments.Count - 1);
        typeName = string.Join(".", segments);
    }

    /// <summary>
    /// Gets the name of the calling method at the specified stack frame depth.
    /// </summary>
    /// <param name="frameDepth">The stack frame depth to inspect.</param>
    /// <returns>The name of the calling method, or a fallback message if it cannot be determined.</returns>
    internal static string CallingMethod(int frameDepth = 1)
    {
        StackTrace stackTrace = new();
        var methodBase = stackTrace.GetFrame(frameDepth)?.GetMethod();
        if (methodBase == null)
        {
            return "Method name cannot be get";
        }
        var methodName = methodBase.Name;
        return methodName;
    }

    /// <summary>
    /// Builds an error message indicating that a variable is null.
    /// </summary>
    /// <param name="before">A prefix for the error message, typically the calling context.</param>
    /// <param name="variableName">The name of the variable that is null.</param>
    /// <param name="variable">The variable to check for null.</param>
    /// <returns>An error message string if the variable is null; otherwise, null.</returns>
    internal static string? IsNull(string before, string variableName, object? variable)
    {
        return variable == null ? CheckBefore(before) + variableName + " " + "is null" + "." : null;
    }
}
