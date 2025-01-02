using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IzukaBusClient
{
    internal  class JWTHelper
    {
       public static async Task<string> GetAccessToken(string clientId, string url, string secreteKey)
        {
           

            string jsonContent = $"{{\"ClientId\":\"{clientId}\",\"SecretKey\":\"{secreteKey}\"}}";

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("accept", "*/*");

                var content = new StringContent(
                 jsonContent,       
                 Encoding.UTF8,    
                 "application/json"  
                );

                // Make the POST request to the CreateClient API endpoint
                HttpResponseMessage response = await client.PostAsync("api/Auth/Authenticate", content);

                // Check if the response was successful
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseBody);
                    return responseBody;
                }
                else
                {
                    Console.WriteLine("Error: " + response.StatusCode);
                    return null;
                }
            }
        }
    }
}

