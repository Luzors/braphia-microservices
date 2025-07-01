using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Braphia.Accounting.Converters
{
    public class DecimalJsonConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                if (decimal.TryParse(reader.GetString(), out var value))
                {
                    return value;
                }

                throw new JsonException("Invalid decimal value in JSON.");
            }

            return reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }
}
