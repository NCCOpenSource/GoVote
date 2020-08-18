using ConsoleGraphTest;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Collections.Generic;
using System.Net.Http;

namespace VotingApp.Services
{
    public class GetMembersFromAzure
    {
        private static IConfiguration _configuration;
        private static GraphServiceClient _graphServiceClient;
        private static HttpClient _httpClient;

        public GetMembersFromAzure(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IGraphServiceUsersCollectionPage GetCurrentMembers()
        {
            GraphServiceClient graphClient = GetAuthenticatedGraphClient();
            List<QueryOption> options = new List<QueryOption>
            {
                new QueryOption("$filter", "jobTitle eq 'Member'"),
            };

            var graphResult = graphClient.Users.Request(options).GetAsync().Result;

            return graphResult;


        }

        private static GraphServiceClient GetAuthenticatedGraphClient()
        {
            var authenticationProvider = CreateAuthorizationProvider();
            _graphServiceClient = new GraphServiceClient(authenticationProvider);
            return _graphServiceClient;
        }

        private static HttpClient GetAuthenticatedHTTPClient()
        {
            var authenticationProvider = CreateAuthorizationProvider();
            _httpClient = new HttpClient(new AuthHandler(authenticationProvider, new HttpClientHandler()));
            return _httpClient;
        }

        private static IAuthenticationProvider CreateAuthorizationProvider()
        {
            string clientId = _configuration.GetSection("GraphAPI").GetValue<string>("applicationId");
            string clientSecret = _configuration.GetSection("GraphAPI").GetValue<string>("applicationSecret");
            string redirectUri = _configuration.GetSection("GraphAPI").GetValue<string>("redirectUri");
            string tenantId = _configuration.GetSection("GraphAPI").GetValue<string>("tenantId");
            string authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";

            //this specific scope means that application will default to what is defined in the application registration rather than using dynamic scopes
            List<string> scopes = new List<string>();
            scopes.Add("https://graph.microsoft.com/.default");

            var cca = new ConfidentialClientApplication(clientId, authority, redirectUri, new ClientCredential(clientSecret), null, null);
            return new MsalAuthenticationProvider(cca, scopes.ToArray());
        }
    }
}
