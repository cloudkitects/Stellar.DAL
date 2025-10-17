using System.Globalization;
using System.Text.RegularExpressions;

namespace Stellar.DAL;

/// <summary>
/// A converter between naming conventions.
/// </summary>
/// <remarks>
/// TODO: inject an acronym list to preserve?
/// </remarks>
public static partial class NameConverter
{
    private const string KeyTemplate = "{0}To{1}";


    /// <summary>
    /// A delegate signature to add conversions.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public delegate string NameConversion(string source);
    
    /// <summary>
    /// A cache of conversions.
    /// </summary>
    private static readonly Dictionary<string, NameConversion> Conversions = [];

    public static void AddConversion(NamingConvention sourceConvention, NamingConvention targetConvention, NameConversion conversion)
    {
        if (sourceConvention == targetConvention)
        {
            throw new ArgumentException("The source and target naming conventions to convert cannot be the same.");
        }

        if (conversion is null)
        {
            throw new ArgumentNullException(nameof(conversion), "The conversion parameter cannot be null.");
        }

        var key = string.Format(KeyTemplate, sourceConvention, targetConvention);

        if (Conversions.ContainsKey(key))
        {
            Conversions[key] = conversion;

            return;
        }

        Conversions.Add(key, conversion);
    }

    static NameConverter()
    {
        #region Pascal
        AddConversion(
            NamingConvention.Pascal, 
            NamingConvention.Camel,
            (source) => CamelPascalRegex().Replace(source, match => match.Groups[1].Value.ToLowerInvariant() + match.Groups[2].Value));

        AddConversion(
            NamingConvention.Pascal,
            NamingConvention.LowerSnake,
            (source) => PascalTransitionRegex().Replace(source, match => match.Groups[1].Value + '_' + match.Groups[2].Value).ToLowerInvariant());

        AddConversion(
            NamingConvention.Pascal,
            NamingConvention.UpperSnake,
            (source) => PascalTransitionRegex().Replace(source, match => match.Groups[1].Value + '_' + match.Groups[2].Value).ToUpperInvariant());
        #endregion

        #region Camel
        AddConversion(
            NamingConvention.Camel, 
            NamingConvention.Pascal,
            (source) => CamelPascalRegex().Replace(source, match => match.Groups[1].Value.ToUpperInvariant() + match.Groups[2].Value));

        AddConversion(
            NamingConvention.Camel,
            NamingConvention.LowerSnake,
            (source) => PascalTransitionRegex().Replace(source, match => match.Groups[1].Value + '_' + match.Groups[2].Value).ToLowerInvariant());

        AddConversion(
            NamingConvention.Camel,
            NamingConvention.UpperSnake,
            (source) => PascalTransitionRegex().Replace(source, match => match.Groups[1].Value + '_' + match.Groups[2].Value).ToUpperInvariant());
        #endregion


        #region lower snake
        AddConversion(
            NamingConvention.LowerSnake,
            NamingConvention.Pascal,
            (source) => SnakePascalRegex().Replace(source, match => match.Value.ToUpperInvariant()).Replace("_", string.Empty));
    
        AddConversion(
        NamingConvention.LowerSnake,
        NamingConvention.Camel,
        (source) => SnakeCamelRegex().Replace(source.ToLowerInvariant(), match => match.Groups[1].Value.ToUpperInvariant()));

        AddConversion(
        NamingConvention.LowerSnake,
        NamingConvention.UpperSnake,
        (source) => source.ToUpperInvariant());
        #endregion

        #region upper snake
        AddConversion(
            NamingConvention.UpperSnake,
            NamingConvention.Pascal,
            (source) => SnakePascalRegex().Replace(source.ToLowerInvariant(), match => match.Value.ToUpperInvariant()).Replace("_", string.Empty));

        AddConversion(
        NamingConvention.UpperSnake,
        NamingConvention.Camel,
        (source) => SnakeCamelRegex().Replace(source.ToLowerInvariant(), match => match.Groups[1].Value.ToUpperInvariant()));

        AddConversion(
        NamingConvention.UpperSnake,
        NamingConvention.LowerSnake,
        (source) => source.ToLowerInvariant());
        #endregion
    }

    public static string Convert(NamingConvention sourceConvention, NamingConvention targetConvention,
        string source)
    {
        var key = string.Format(KeyTemplate, sourceConvention, targetConvention);

        return Conversions[key](source);
    }

    [GeneratedRegex(@"([a-z0-9])([A-Z])")]
    private static partial Regex PascalTransitionRegex();
    
    [GeneratedRegex(@"(?:_)(\w)")]
    private static partial Regex SnakeCamelRegex();
    
    [GeneratedRegex(@"(?:\b)(\w)(\w+)")]
    private static partial Regex CamelPascalRegex();

    [GeneratedRegex(@"(:?[\W_])(\w)")]
    private static partial Regex SnakePascalRegex();
}
