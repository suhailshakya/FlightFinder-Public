using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Vacation.Auth
{
    public class Authorization
    {
        private readonly HttpClient httpClient = new HttpClient();
        public string apiKey = "";
        public string apiSecret = "";
        public string accessCode = "";

        // Constructor that takes client ID and client secret as parameters
        public Authorization()
        {
            //this.httpClient = httpClient;
        }

        //setAuth method to call the Amadeus POST command
        public async Task SetAuth()
        {
            try
            {
                await AmadeusAuth();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during authorization: {ex.Message}");
            }
        }

        public async Task AmadeusAuth()
        {
            //read authKey and authSecret from Vault
            ReadFile rf = new ReadFile();
            apiKey = rf.ReadFileCreds("authKey");
            apiSecret = rf.ReadFileCreds("authSecret");

            // Form data for the POST request
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", apiKey),
                new KeyValuePair<string, string>("client_secret", apiSecret),
            });

            //formData.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            HttpResponseMessage response = await httpClient.PostAsync("https://test.api.amadeus.com/v1/security/oauth2/token", formData);
                // HttpResponseMessage response = await postAuth.PostAsync(postAuth.BaseAddress, formData);

                if (response.IsSuccessStatusCode)
                {
                    string parsedResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(" -------------------- 1");
                    Console.WriteLine(parsedResponse);
                    Console.WriteLine(" -------------------- 1");
                    dynamic ak = JsonConvert.DeserializeObject(parsedResponse);
                    this.accessCode = ak.access_token;
                }
                else
                {
                    Console.WriteLine(response.StatusCode.ToString());
                    Console.WriteLine("Request failed, no Authorization");
                }
            //}
        }
        public string GetAccessToken()
        {
            return this.accessCode;
        }
    }
}
