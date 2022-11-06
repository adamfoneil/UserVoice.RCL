namespace UserVoice.RCL.Extensions
{
    internal static class Dropdown
    {
        internal static Dictionary<int, string> FromEnum<TEnum>() where TEnum : struct, Enum
        {
            var result = new Dictionary<int, string>();
            var values = Enum.GetValues<TEnum>();

            foreach (var item in values)
            {
                var name = Enum.GetName(item);
                result.Add(Convert.ToInt32(item), FormatEnumName(name));
            }

            return result;
        }

        /// <summary>
        /// put a space before capital letters
        /// </summary>
        private static string FormatEnumName(string name) =>
            string.Join(string.Empty, name.Select((c, index) => (char.IsUpper(c) && index > 0) ? $" {c}" : c.ToString()));
    }
}
