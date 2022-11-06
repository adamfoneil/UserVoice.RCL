namespace UserVoice.RCL.Extensions
{
    internal static class Css
    {
        public static string? GetClass(Dictionary<string, object>? parameters) => ((parameters?.TryGetValue("class", out object? value) ?? false) && value is not null) ? value.ToString() : null;
    }
}
