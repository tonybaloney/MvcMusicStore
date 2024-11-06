using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

public class AIFunctionApiClient
{
    private readonly HttpClient client = new HttpClient();

    public AIFunctionApiClient(string host)
    {
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        client.DefaultRequestHeaders.Add("Accept", "application/json");
        client.BaseAddress = new Uri(host);
    }

    public async Task<string> GetRecordSearchAsync(string query)
    {
        try
        {
            var uriBuilder = new UriBuilder(client.BaseAddress)
            {
                Path = "/api/RecordSearch",
                Query = $"query={Uri.EscapeDataString(query)}"
            };
            HttpResponseMessage response = await client.GetAsync(uriBuilder.Uri);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(responseBody);
        }
        catch (HttpRequestException e)
        {
            // Handle exception
            Console.WriteLine($"Request error: {e.Message}");
            return null;
        }
    }
}
