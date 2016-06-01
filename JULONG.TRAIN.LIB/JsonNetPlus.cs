using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JULONG.TRAIN.LIB
{
    public class JsonCustomDateConverter : DateTimeConverterBase
    {
        //private TimeZoneInfo _timeZoneInfo;
        private string _dateFormat;

        public JsonCustomDateConverter()
        {
            _dateFormat = "yyyy-MM-dd HH:mm:ss";
            //_timeZoneInfo = timeZoneInfo;
        }
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(Convert.ToDateTime(value).ToString(_dateFormat));
            writer.Flush();
        }
    }
}