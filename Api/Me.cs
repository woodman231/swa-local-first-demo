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

            var myClientPrincipal = GetClientPrincipal(req, log);

            if(myClientPrincipal.UserId is not null)
            {
                myResponse.ClientPrincipal = myClientPrincipal;
            }

            return new OkObjectResult(myResponse);
        }

        private static ClientPrincipal GetClientPrincipal(HttpRequest req, ILogger log)
        {
            var principal = new ClientPrincipal();

            /*
            var staticWebAppsAuthCookie = req.Cookies.Where(w => w.Key == "StaticWebAppsAuthCookie").FirstOrDefault();

            if(!string.IsNullOrEmpty(staticWebAppsAuthCookie.Value))
            {
                var data = staticWebAppsAuthCookie.Value;
                var decoded = Convert.FromBase64String(data);
                var json = Encoding.UTF8.GetString(decoded);
                principal = JsonConvert.DeserializeObject<ClientPrincipal>(json);                
            }
            */

            var swaCookie = req.Cookies["StaticWebAppsAuthCookie"];

            if(swaCookie != null)
            {
                log.LogInformation("SWA Cookie Found");
                var decoded = Convert.FromBase64String(swaCookie);
                log.LogInformation("SWA Cookie Decoded to a Byte Array");
                var json = Encoding.UTF8.GetString(decoded);
                log.LogInformation($"SWA Cookie JSON: {json}");
                principal = JsonConvert.DeserializeObject<ClientPrincipal>(json);
                log.LogInformation($"SWA Cookie Deserialized");
            }

            return principal;
        }

        private class ClientPrincipalResponse
        {
            public ClientPrincipal ClientPrincipal { get; set; } = null;
        }
    }
}
