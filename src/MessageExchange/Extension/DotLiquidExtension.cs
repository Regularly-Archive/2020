using DotLiquid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace MessageExchange.Extension
{
    public static class DotLiquidExtension
    {
        /// <summary>
        /// 解析JObject
        /// </summary>
        /// <param name="jObject"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 解析Xml
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static Hash FromXml(XmlDocument document)
        {
            var json = JsonConvert.SerializeXmlNode(document);
            var obj = JObject.Parse(json);
            return FromJson(obj);
        }

        /// <summary>
        /// 解析JArray
        /// </summary>
        /// <param name="jArray"></param>
        /// <returns></returns>
        private static List<Hash> FromJson(JArray jArray)
        {
            if (jArray == null)
                return null;

            var listHash = new List<Hash>();
            for (int i = 0; i < jArray.Count; i++)
            {
                var item = (JObject)jArray[i];
                var value = FromJson(item);
                if (value != null) {
                    listHash.Add(FromJson(item));
                }
            }

            return listHash;
        }
    }
}
