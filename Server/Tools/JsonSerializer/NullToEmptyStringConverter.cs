﻿using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server.Tools.JsonSerializer;

public class NullToEmptyStringConverter : JsonConverter<string>
{
    public override bool HandleNull => true;
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert == typeof(string);
    }
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value ?? "");
    }
}