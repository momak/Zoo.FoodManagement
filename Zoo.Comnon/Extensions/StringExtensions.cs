namespace Zoo.Common.Extensions
{
    public static class StringExtensions
    {
        public static decimal FromPercentageString(this string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;

            return decimal.Parse(value.Replace("%", "")) / 100;
        }
    }
}
