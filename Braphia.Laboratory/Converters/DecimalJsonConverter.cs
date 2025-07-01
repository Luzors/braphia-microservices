using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Braphia.Laboratory.Converters
{
    public class DecimalJsonConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();
                
                // Use InvariantCulture to ensure consistent decimal parsing
                if (stringValue != null && decimal.TryParse(stringValue, System.Globalization.NumberStyles.Any, 
                    System.Globalization.CultureInfo.InvariantCulture, out var value))
                {
                    return value;
                }

                throw new JsonException($"Invalid decimal value in JSON: '{stringValue}'");
            }

            return reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
