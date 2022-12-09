using Microsoft.AspNetCore.Components;

namespace UserVoice.RCL.Extensions
{
    internal static class StringExtensions
    {
        internal static MarkupString ToMarkup(this string? input) => new MarkupString(input ?? string.Empty);
    }
}
