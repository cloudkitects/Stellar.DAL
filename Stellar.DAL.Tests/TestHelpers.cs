namespace Stellar.DAL.Tests;

public class TestHelpers
{
    #region properties
    public static readonly string Root = System.Reflection.Assembly
            .GetExecutingAssembly()?.GetName()?.Name?
            .Replace('.', '-') ?? "unit-tests";
    #endregion

    #region helpers
    private static readonly Random Random = new();
    public static string RandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        
        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)])]);
    }

    public static string RandomDigits(int length)
    {
        const string chars = "0123456789";

        return new string([.. Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)])]);
    }

    public static string TestDbName()
    {
        return $"{Root}-{RandomString(6)}";
    }

    public static string ParseSqlFile(string name, string database)
    {
        using var reader = new StreamReader(new FileStream(name, FileMode.Open));
        
        var template = reader.ReadToEnd();

        return string.Format(template, database);
    }
    #endregion
}
