using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace GridExplorerBot
{
    public static class WebUtils
    {
        public class WebRequest
        {
            public string httpMethod = "";
            public Dictionary<string, string> queryStringParameters = new Dictionary<string, string>();
            public string body = "";

            public bool IsGet() { return httpMethod == "GET"; }
            public bool IsPost() { return httpMethod == "POST"; }
        }

        public static WebRequest GetJsonObject(string jsonText)
        {
            WebRequest request = new WebRequest();

            using (JsonTextReader reader = new JsonTextReader(new System.IO.StringReader(jsonText)))
            {
                while (reader.Read())
                {
                    string currentToken = reader.Value as string;
                    if (currentToken == "httpMethod")
                    {
                        request.httpMethod = reader.ReadAsString();
                    }
                    else if (request.queryStringParameters.Count == 0 && currentToken == "crc_token")
                    {
                        request.queryStringParameters["crc_token"] = reader.ReadAsString();
                    }
                    else if (currentToken == "body")
                    {
                        request.body = reader.ReadAsString();
                    }
                }
            }

            return request;
        }
    }
}