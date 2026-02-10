using System.Text.RegularExpressions;

public static class TimeParser
{
    public static int ParseToMinutes(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return 0;

        input = input.ToLowerInvariant().Trim();

        // Handle known text cases
        if (input.Contains("or less"))
            return 60; // or choose 30 depending on business rule

        int totalMinutes = 0;

        // Match hours (h, hr, hrs, hour, hours)
        var hoursMatch = Regex.Match(input, @"(\d+)\s*(h|hr|hrs|hour|hours)");
        if (hoursMatch.Success)
        {
            totalMinutes += int.Parse(hoursMatch.Groups[1].Value) * 60;
        }

        // Match minutes (m, min, mins, minute, minutes)
        var minutesMatch = Regex.Match(input, @"(\d+)\s*(m|min|mins|minute|minutes)");
        if (minutesMatch.Success)
        {
            totalMinutes += int.Parse(minutesMatch.Groups[1].Value);
        }

        return totalMinutes > 0 ? totalMinutes : 0;
    }
}
