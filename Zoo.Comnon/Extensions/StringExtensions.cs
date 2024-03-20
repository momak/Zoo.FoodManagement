using System.Globalization;

namespace Zoo.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        ///  Extension method to return a decimal representation of string percentage.
        /// </summary>
        /// <param name="value">string percentage input ex. 22%</param>
        /// <returns>decimal value</returns>
        public static decimal FromPercentageString(this string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            return decimal.Parse(value.Replace("%", "")) / 100;
        }

        /// <summary>
        ///  Extension method to return a decimal representation of string.
        /// </summary>
        /// <param name="value">decimal as a string</param>
        /// <param name="culture">optional culture specific</param>
        /// <returns>decimal value</returns>
        public static decimal ToDecimal(this string? value, CultureInfo? culture = null)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            culture ??= CultureInfo.InvariantCulture;

            return decimal.TryParse(value, NumberStyles.Any, culture, out var result) 
                ? result 
                : 0;
        }
    }
}
