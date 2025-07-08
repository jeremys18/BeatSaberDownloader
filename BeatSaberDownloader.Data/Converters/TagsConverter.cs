using BeatSaberDownloader.Data.Enums;
using Newtonsoft.Json;

namespace BSSD.DownloadService.Converters
{
    public class TagsConverter: JsonConverter<Tag[]>
    {
        public override Tag[] ReadJson(JsonReader reader, Type objectType, Tag[] existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return [];
            }
            var tagNames = ((string)reader.Value).Split('-');
            var tags = tagNames.Select(tag => (Tag)Enum.Parse(typeof(Tag), tag.Trim())).ToArray();
            return tags;
        }
        public override void WriteJson(JsonWriter writer, Tag[] value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            foreach (var tag in value)
            {
                writer.WriteValue(tag);
            }
            writer.WriteEndArray();
        }
    }
}
