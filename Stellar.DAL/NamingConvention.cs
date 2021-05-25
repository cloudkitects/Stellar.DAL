namespace Stellar.DAL
{
    /// <summary>
    /// Database object naming conventions.
    /// </summary>
    public enum NamingConvention
    {
        /// <summary>
        /// Literally, PascalCase aka upper camel case--and no conversion from C#.
        /// </summary>
        Pascal = 0,
        /// <summary>
        /// camelCase, e.g., objectPropertyInfo.
        /// </summary>
        Camel = 1,
        /// <summary>
        /// Lowercase snake aka lower-underscore case, e.g., customer_name.
        /// </summary>
        LowerSnake = 2,
        /// <summary>
        /// Uppercase snake aka upper-underscore case, e.g., FIRST_NAME.
        /// </summary>
        UpperSnake = 3
    }
}
