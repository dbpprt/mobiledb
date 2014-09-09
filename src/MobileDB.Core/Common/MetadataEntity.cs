using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MobileDB.Common
{
    [JsonObject(MemberSerialization.OptIn)]
    public class MetadataEntity
    {
        [JsonProperty("identity")]
        public object Identity { get; set; }

        [JsonProperty("entity")]
        public object Entity { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, string> Metadata { get; set; }

        public object EntityOfType(Type type)
        {
            var jobj = Entity as JObject;

            if (jobj != null)
            {
                return jobj.ToObject(type);
            }

            // entity should never be on disk before and should be assignable from 
            // the desired type
            return Entity;
        }
    }
}