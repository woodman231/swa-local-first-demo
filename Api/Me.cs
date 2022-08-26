using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Shared.Models;
using System;
using System.Text;
using System.Text.Json;

namespace Api
{
    public static class Me
    {
        [FunctionName("Me")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var myResponse = new ClientPrincipalResponse();

            var myClientPrincipal = GetClientPrincipal(req, log);

            if (myClientPrincipal.UserId is not null)
            {
                myResponse.ClientPrincipal = myClientPrincipal;
            }

            return new OkObjectResult(myResponse);
        }

        private static ClientPrincipal GetClientPrincipal(HttpRequest req, ILogger log)
        {            
            var clientPrincipal = new ClientPrincipal();

            if (req.Headers.TryGetValue("x-ms-client-principal", out var header))
            {
                var data = header[0];
                var decoded = Convert.FromBase64String(data);
                var json = Encoding.UTF8.GetString(decoded);
                clientPrincipal = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }

            return clientPrincipal;
        }

        private class ClientPrincipalResponse
        {
            public ClientPrincipal ClientPrincipal { get; set; } = null;
        }
    }
}
