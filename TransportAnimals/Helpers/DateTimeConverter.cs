﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace TransportAnimals.Helpers {
    public class DateTimeConverter : JsonConverter<DateTimeOffset> {
        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            return DateTimeOffset.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) {
            writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffzzz"));
        }
    }
}
