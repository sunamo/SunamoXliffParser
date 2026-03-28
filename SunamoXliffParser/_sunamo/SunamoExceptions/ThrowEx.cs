namespace SunamoXliffParser._sunamo.SunamoExceptions;

/// <summary>
/// Provides methods for throwing exceptions with detailed context about the calling code.
/// </summary>
internal partial class ThrowEx
{
    /// <summary>
    /// Throws an exception if the specified variable is null.
    /// </summary>
    /// <param name="variableName">The name of the variable to check.</param>
    /// <param name="variable">The variable value to check for null.</param>
    /// <returns>True if the variable was null and an exception was thrown; otherwise, false.</returns>
    internal static bool IsNull(string variableName, object? variable = null)
    {
        return ThrowIsNotNull(Exceptions.IsNull(FullNameOfExecutedCode(), variableName, variable));
    }

    /// <summary>
    /// Gets the fully qualified name of the currently executing code location.
    /// </summary>
    /// <returns>A string in the format "TypeName.MethodName".</returns>
    internal static string FullNameOfExecutedCode()
    {
        Tuple<string, string, string> placeOfException = Exceptions.PlaceOfException();
        string fullName = FullNameOfExecutedCode(placeOfException.Item1, placeOfException.Item2, true);
        return fullName;
    }

    /// <summary>
    /// Gets the fully qualified name of the executed code from the specified type and method name.
    /// </summary>
    /// <param name="type">The type or type name of the calling code.</param>
    /// <param name="methodName">The method name, or null to determine it from the call stack.</param>
    /// <param name="isFromThrowEx">Whether the call originates from a ThrowEx method, which affects stack depth calculation.</param>
    /// <returns>A string in the format "TypeName.MethodName".</returns>
    static string FullNameOfExecutedCode(object type, string methodName, bool isFromThrowEx = false)
    {
        if (methodName == null)
        {
            int depth = 2;
            if (isFromThrowEx)
            {
                depth++;
            }

            methodName = Exceptions.CallingMethod(depth);
        }
        string typeFullName;
        if (type is Type typeValue)
        {
            typeFullName = typeValue.FullName ?? "Type cannot be get via type is Type type2";
        }
        else if (type is MethodBase method)
        {
            typeFullName = method.ReflectedType?.FullName ?? "Type cannot be get via type is MethodBase method";
            methodName = method.Name;
        }
        else if (type is string)
        {
            typeFullName = type.ToString() ?? "Type cannot be get via type is string";
        }
        else
        {
            Type objectType = type.GetType();
            typeFullName = objectType.FullName ?? "Type cannot be get via type.GetType()";
        }
        return string.Concat(typeFullName, ".", methodName);
    }

    /// <summary>
    /// Throws an exception with the specified message if it is not null.
    /// </summary>
    /// <param name="exception">The exception message to throw, or null if no exception should be thrown.</param>
    /// <param name="isReallyThrowing">Whether to actually throw the exception or just return the result.</param>
    /// <returns>True if the exception message was not null; otherwise, false.</returns>
    internal static bool ThrowIsNotNull(string? exception, bool isReallyThrowing = true)
    {
        if (exception != null)
        {
            Debugger.Break();
            if (isReallyThrowing)
            {
                throw new Exception(exception);
            }
            return true;
        }
        return false;
    }
}
