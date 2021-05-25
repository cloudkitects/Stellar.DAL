namespace Stellar.DAL
{
    /// <summary>
    /// The method used for escaping keywords.
    /// </summary>
    public enum KeywordEscapeMethod
    {
        /// <summary>No escape method is used.</summary>
        None = 0,
        /// <summary>Keywords are enclosed in square brackets. Used by SQL Server, SQLite.</summary>
        SquareBracket = 1,
        /// <summary>Keywords are enclosed in double quotes. Used by Post-gre SQL, SQLite.</summary>
        DoubleQuote = 2,
        /// <summary>Keywords are enclosed in back-ticks aka grave accents (ASCII code 96). Used by MySQL, SQLite.</summary>
        Backtick = 3
    }
}
