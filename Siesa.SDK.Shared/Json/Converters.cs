using System;
using System.Globalization;
using Newtonsoft.Json;
namespace Siesa.SDK.Shared.Json
{
    public class DateOnlyJsonConverter : JsonConverter<DateOnly?>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateOnly? ReadJson(JsonReader reader, Type objectType, DateOnly? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if(reader.Value == null)
            {
                return null;
            }
            return DateOnly.ParseExact((string)reader.Value, DateFormat, CultureInfo.InvariantCulture);
        }

        public override void WriteJson(JsonWriter writer, DateOnly? value, JsonSerializer serializer)
        {
            if(value.HasValue)
            {
                writer.WriteValue(value.Value.ToString(DateFormat, CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteNull();
            }
            
        }
    }

    public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
    {
        private const string TimeFormat = "HH:mm:ss.FFFFFFF";

        public override TimeOnly ReadJson(JsonReader reader, Type objectType, TimeOnly existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return TimeOnly.ParseExact((string)reader.Value, TimeFormat, CultureInfo.InvariantCulture);
        }

        public override void WriteJson(JsonWriter writer, TimeOnly value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString(TimeFormat, CultureInfo.InvariantCulture));
        }
    }
    
}