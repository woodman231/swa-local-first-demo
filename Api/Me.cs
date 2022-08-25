using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shared.Models;
using Shared.Extensions;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net;

namespace Api
{
    public static class Me
    {
        [FunctionName("Me")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var myResponse = new ClientPrincipalResponse();

            var myClientPrincipal = await GetClientPrincipal(req, log);

            if(myClientPrincipal.UserId is not null)
            {
                myResponse.ClientPrincipal = myClientPrincipal;
            }

            return new OkObjectResult(myResponse);
        }

        private static async Task<ClientPrincipal> GetClientPrincipal(HttpRequest req, ILogger log)
        {            
            var principal = new ClientPrincipal();            

            var swaCookie = req.Cookies["StaticWebAppsAuthCookie"];

            if(swaCookie != null)
            {
                try
                {
                    string endPoint = $"{req.Scheme}://{req.Host}/.auth/me";

                    HttpClient client = new HttpClient();
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endPoint);

                    request.Headers.Add("Cookie", $"StaticWebAppsAuthCookie={swaCookie}");

                    log.LogInformation($"Sending Http Request to {endPoint} with StaticWebAppsAuthCookie={swaCookie}");

                    var response = await client.SendAsync(request);
                    var information = await response.Content.ReadAsStringAsync();

                    log.LogInformation($"Recieved the following response from {endPoint}: {information}");

                    var jsonResult = System.Text.Json.JsonSerializer.Deserialize<ClientPrincipalPayLoad>(information, new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return jsonResult.ClientPrincipal;

                }
                catch(Exception ex)
                {
                    log.LogError(ex.Message);
                }
            }

            return principal;
        }

        private class ClientPrincipalResponse
        {
            public ClientPrincipal ClientPrincipal { get; set; } = null;
        }

        private class ClientPrincipalPayLoad
        {
            public ClientPrincipal? ClientPrincipal { get; set; }
        }
    }
}
