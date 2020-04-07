using Newtonsoft.Json;
using System.Collections.Generic;

namespace MJR047.FunctionApp1
{
    public class SayHelloRequest
    {
        [JsonProperty("names")]
        public IEnumerable<string> Names { get; set; }
    }
}
