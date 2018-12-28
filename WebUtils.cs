using Newtonsoft.Json;
using System.Collections.Generic;

namespace GridExplorerBot
{
    public static class WebUtils
    {
        public class WebRequest
        {
            public string httpMethod = "";
            public Dictionary<string,string> queryStringParameters = new Dictionary<string, string>();
        }

        public static WebRequest GetJsonObject(string jsonText)
        {
            return JsonConvert.DeserializeObject<WebRequest>(jsonText);
        }
    }
}