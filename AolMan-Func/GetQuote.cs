using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace AolManFunc
{
    public static class GetQuote
    {
        [FunctionName("GetQuote")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequestMessage req, TraceWriter log)
        {
            string url = @"https://aolmanstorage.blob.core.windows.net/assets/aolman.txt";

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Method = HttpMethod.Get;
            httpRequestMessage.RequestUri = new Uri(url);

            HttpClient httpClient = new HttpClient();

            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            string[] lines;
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                await httpResponseMessage.Content.LoadIntoBufferAsync();
                string stringResult = await httpResponseMessage.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(stringResult))
                {
                    lines = await ReadAllLines(stringResult);

                    Random rand = new Random();
                    string value = lines[rand.Next(lines.Length)];
                    //return req.CreateResponse(HttpStatusCode.OK, value);
                    return new HttpResponseMessage
                    {
                        Content = new StringContent(value, Encoding.UTF8, "text/plain")
                    };
                }
            }

            return req.CreateResponse(HttpStatusCode.InternalServerError);

        }

        private static async Task<string[]> ReadAllLines(string item)
        {
            string line;
            List<String> lines = new List<String>();

            using (StringReader sr = new StringReader(item))
            {
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines.ToArray();
        }
    }
}
