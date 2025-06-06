using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonitorAppLib.Clients
{
    internal class DelayClient
    {
        private readonly HttpClient _client;
        private readonly string _url = "http://localhost:5000/";

        public DelayClient(HttpClient client)
        {
            _client = client;
        }
        public async Task<HttpStatusCode> SendDelay(int delay)
        {
            var jsonSetDelay = JsonSerializer.Serialize(
                new SetMonitorDelay() { Delay = delay });
            var content = new StringContent(jsonSetDelay, Encoding.UTF8, "text/plain");
            var response = await _client.PostAsync(_url, content);
            return response.StatusCode;
        }
    }
}
