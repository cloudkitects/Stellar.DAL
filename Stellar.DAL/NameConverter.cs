using System.Globalization;
using System.Text.RegularExpressions;

namespace Stellar.DAL;

/// <summary>
/// A converter between naming conventions.
/// </summary>
public static partial class NameConverter
{
    /// <summary>
    /// The default text info used to change case to title case (aka proper case)
    /// TODO: inject a culture info or use as default (including the invariant calls)
    /// TODO: inject an acronym list to preserve?
    /// </summary>
    private static TextInfo TextInfo => new CultureInfo("en-US", false).TextInfo;

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
            throw new ArgumentException(
                "Tne source naming convention must be different than the target naming convention.");
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
        #region Pascal to...
        AddConversion(
            NamingConvention.Pascal, 
            NamingConvention.Camel,
            (source) => char.ToLowerInvariant(source[0]) + source[1..]);

        AddConversion(
            NamingConvention.Pascal,
            NamingConvention.LowerSnake,
            (source) =>
            {
                var target = PascalTransitionRegex().Replace(source, "$1_$2")
                    .ToLowerInvariant();

                return target;
            });

        AddConversion(
            NamingConvention.Pascal,
            NamingConvention.UpperSnake,
            (source) =>
            {
                var target = TextInfo.ToTitleCase(UnderscoresRegex().Replace(source, " ")
                    .ToUpperInvariant()
                    .Replace(" ", string.Empty));

                return target;
            });
        #endregion

        AddConversion(
            NamingConvention.LowerSnake,
            NamingConvention.Pascal,
            (source) =>
            {
                var target = SnakeTransitionRegex().Replace(source.ToLowerInvariant(), match => match.Value.ToUpperInvariant())
                    .Replace("_", string.Empty);

                return target;
            });

        AddConversion(
            NamingConvention.UpperSnake,
            NamingConvention.Pascal,
            Conversions[string.Format(KeyTemplate, NamingConvention.LowerSnake, NamingConvention.Pascal)]);
    }

    public static string Convert(NamingConvention sourceConvention, NamingConvention targetConvention,
        string source)
    {
        var key = string.Format(KeyTemplate, sourceConvention, targetConvention);

        return Conversions[key](source);
    }

    [GeneratedRegex(@"([a-z0-9])([A-Z])")]
    private static partial Regex PascalTransitionRegex();
    [GeneratedRegex(@"_+")]
    private static partial Regex UnderscoresRegex();
    [GeneratedRegex(@"(?:\b|_)(\w)")]
    private static partial Regex SnakeTransitionRegex();
}
