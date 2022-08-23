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

                    var baseAddress = new Uri($"{req.Scheme}://{req.Host}");

                    var cookieContainer = new CookieContainer();

                    var handler = new HttpClientHandler() { CookieContainer = cookieContainer };

                    var client = new HttpClient(handler) { BaseAddress = baseAddress };

                    cookieContainer.Add(baseAddress, new Cookie("StaticWebAppsAuthCookie", swaCookie));

                    var response = await client.GetFromJsonAsync<ClientPrincipalPayLoad>("/.auth/me");

                    if (response is not null)
                    {
                        if (response.ClientPrincipal is not null)
                        {
                            return response.ClientPrincipal;
                        }
                    }
                }
                catch(Exception ex)
                {
                    log.LogError(ex.Message);
                }



                /*
                log.LogInformation("SWA Cookie Found");
                var decoded = Convert.FromBase64String(swaCookie);
                log.LogInformation("SWA Cookie Decoded to a Byte Array");
                var json = Encoding.UTF8.GetString(decoded);
                log.LogInformation($"SWA Cookie JSON: {json}");
                principal = JsonConvert.DeserializeObject<ClientPrincipal>(json);
                log.LogInformation($"SWA Cookie Deserialized");
                */
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
