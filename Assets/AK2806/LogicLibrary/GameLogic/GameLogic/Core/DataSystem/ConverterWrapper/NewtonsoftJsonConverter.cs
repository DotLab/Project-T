using System;
using System.Collections.Generic;
using System.Text;

namespace GameLogic.Core.DataSystem.ConverterWrapper
{
    public sealed class NewtonsoftJsonConverter : IJsonConverterRaw
    {
        public T Deserialize<T>(string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public string Serialize(object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }
    }
}
