using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Backend.Helper
{
    using System.Text.Json;
    using System.Text.Json.Serialization;

    /// <summary>
    /// A custom JSON converter for <see cref="TimeOnly"/> values.
    /// </summary>
    /// <remarks>
    /// This converter serializes <see cref="TimeOnly"/> values to strings in the format "HH:mm:ss"
    /// and deserializes strings back into <see cref="TimeOnly"/> instances.
    /// </remarks>
    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        /// <summary>
        /// The time format used for serialization and deserialization ("HH:mm:ss").
        /// </summary>
        private const string Format = "HH:mm:ss";

        /// <summary>
        /// Reads and converts the JSON string to a <see cref="TimeOnly"/> value.
        /// </summary>
        /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
        /// <param name="typeToConvert">The type being converted (always <see cref="TimeOnly"/>).</param>
        /// <param name="options">The serializer options.</param>
        /// <returns>A <see cref="TimeOnly"/> parsed from the JSON string.</returns>
        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return TimeOnly.ParseExact(reader.GetString()!, Format);
        }

        /// <summary>
        /// Writes a <see cref="TimeOnly"/> value as a JSON string using the "HH:mm:ss" format.
        /// </summary>
        /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
        /// <param name="value">The <see cref="TimeOnly"/> value to serialize.</param>
        /// <param name="options">The serializer options.</param>
        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }

}
