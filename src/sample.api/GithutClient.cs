using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace sample.api
{
    public class GithubClient
    {
        private readonly HttpClient _client;
        public GithubClient(HttpClient httpclient)
        {
            _client = httpclient;
        }
        public async Task<int> GetPageStatusCode(Uri uri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Add("Xero-User-Id", Guid.NewGuid().ToString());
            return (int)(await _client.SendAsync(request)).StatusCode;
        }
    }
}
