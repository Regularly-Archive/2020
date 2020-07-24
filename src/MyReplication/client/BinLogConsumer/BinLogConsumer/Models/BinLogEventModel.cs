using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinLogConsumer.Models
{
    public class BinLogEventModel<TModel>
    {
        public string schema { get; set; }
        public string table { get; set; }
        public string action { get; set; }
        public TModel origin { get; set; }
        public TModel current { get; set; }

        public TDest MapTo<TDest>()
        {
            var json = JsonConvert.SerializeObject(current);
            return JsonConvert.DeserializeObject<TDest>(json);
        }
    }
}
