namespace UserVoice.RCL.Extensions
{
    internal static class Dates
    {
        /// <summary>
        /// based on https://stackoverflow.com/a/79601/2023653
        /// </summary>
        internal static string ElapsedRelative(this DateTime firstDate, DateTime secondDate)
        {
            var offsets = new SortedList<double, Func<TimeSpan, string>>
            {
                { 0.75, _ => "less than a minute"},
                { 1.5, _ => "about a minute"},
                { 45, span => $"{span.TotalMinutes:F0} minutes"},
                { 90, span => "about an hour"},
                { 1440, span => $"about {span.TotalHours:F0} hours"},
                { 2880, span => "a day"},
                { 43200, span => $"{span.TotalDays:F0} days"},
                { 86400, span => "about a month"},
                { 525600, span => $"{span.TotalDays / 30:F0} months"},
                { 1051200, span => "about a year"},
                { double.MaxValue, span => $"{span.TotalDays / 365:F0} years"}
            };

            var span = firstDate - secondDate;
            var suffix = span.TotalMinutes > 0 ? " from now" : " ago";
            span = new TimeSpan(Math.Abs(span.Ticks));
            return offsets.First(n => span.TotalMinutes < n.Key).Value(span) + suffix;
        }
    }
}
