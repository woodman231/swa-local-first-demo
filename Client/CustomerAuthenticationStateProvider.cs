using Microsoft.AspNetCore.Components.Authorization;
using Shared.Extensions;
using Shared.Models;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Client
{
    public class CustomerAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;

        public CustomerAuthenticationStateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var claimsIdentity = new ClaimsIdentity();

            var response = await _httpClient.GetFromJsonAsync<ClientPrincipalPayLoad>("/.auth/me");

            if (response is not null)
            {
                if (response.ClientPrincipal is not null)
                {
                    claimsIdentity = response.ClientPrincipal.ToClaimsIdentity();
                }
            }

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authenticationState = new AuthenticationState(claimsPrincipal);

            return authenticationState;
        }

        private class ClientPrincipalPayLoad
        {
            public ClientPrincipal? ClientPrincipal { get; set; }
        }
    }
}
