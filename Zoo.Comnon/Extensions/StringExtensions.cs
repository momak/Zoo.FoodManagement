namespace Zoo.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        ///  Extension method to return a decimal representation of string percentage.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal FromPercentageString(this string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            return decimal.Parse(value.Replace("%", "")) / 100;
        }
    }
}
