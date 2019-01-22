using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using Xpress.Core.DependencyInjection;
using Xpress.Core.Timing;

namespace Xpress.Core.Serialization
{
     public class JsonIsoDateTimeConverter : IsoDateTimeConverter, ITransientDependency
    {
        private readonly IClock _clock;

        public JsonIsoDateTimeConverter(IClock clock)
        {
            _clock = clock;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
            {
                return true;
            }

            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var date = base.ReadJson(reader, objectType, existingValue, serializer) as DateTime?;

            if (date.HasValue)
            {
                return _clock.Normalize(date.Value);
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var date = value as DateTime?;
            base.WriteJson(writer, date.HasValue ? _clock.Normalize(date.Value) : value, serializer);
        }
    }
}
