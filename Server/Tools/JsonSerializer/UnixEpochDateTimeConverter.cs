using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Server.Tools.JsonSerializer;

public class UnixEpochDateTimeConverter : JsonConverter<DateTime>
{
    static readonly DateTime s_epoch = new(1970, 1, 1, 0, 0, 0);
    static readonly Regex s_regex = new Regex("^/Date\\(([+-]*\\d+)\\)/$", RegexOptions.CultureInvariant);

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var formatted = reader.GetString()!;
        var match = s_regex.Match(formatted);

        if (!match.Success || !long.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out long unixTime))
        {
            throw new JsonException();
        }

        return s_epoch.AddMilliseconds(unixTime);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var unixTime = Convert.ToInt64((value - s_epoch).TotalMilliseconds);

        var formatted = FormattableString.Invariant($"/Date({unixTime})/");
        writer.WriteStringValue(formatted);
    }
}