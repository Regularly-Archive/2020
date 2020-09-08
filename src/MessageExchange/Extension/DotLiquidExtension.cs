using DotLiquid;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace MessageExchange.Extension
{
    public static class DotLiquidExtension
    {
        public static Hash FromJson(JObject jObject)
        {
            var resultHash = new Hash();

            if (jObject == null)
                return null;

            foreach (JProperty item in jObject.Children())
            {
                if (item.HasValues && item.Type == JTokenType.Property)
                {
                    switch (item.Value.Type)
                    {
                        case JTokenType.Object:
                            var jObj = item.Value as JObject;
                            if (jObj != null)
                                resultHash.Add(item.Name, FromJson(jObj));
                            break;
                        case JTokenType.Array:
                            var jArray = item.Value as JArray;
                            if (jArray != null)
                                resultHash.Add(item.Name, FromJson(jArray));
                            break;
                        case JTokenType.String:
                        case JTokenType.Integer:
                        case JTokenType.Date:
                        case JTokenType.Boolean:
                        case JTokenType.Float:
                        case JTokenType.Guid:
                        case JTokenType.TimeSpan:
                        case JTokenType.Uri:
                            JValue value = item.Value as JValue;
                            if (value != null)
                                resultHash.Add(item.Name, value.Value);
                            break;
                    }
                }
            }

            return resultHash;
        }

        private static List<Hash> FromJson(JArray jsonArray)
        {
            if (jsonArray == null)
                return null;

            var listHash = new List<Hash>();
            for (int i = 0; i < jsonArray.Count; i++)
            {
                var item = (JObject)jsonArray[i];
                var value = FromJson(item);
                if (value != null) {
                    listHash.Add(FromJson(item));
                }
            }

            return listHash;
        }
    }
}
