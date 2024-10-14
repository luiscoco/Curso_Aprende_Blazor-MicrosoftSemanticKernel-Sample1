using System.Text.Json;
using System.Text;

namespace BlazorAISample1.Services
{
    public class OllamaService
    {
        private readonly HttpClient _httpClient;

        public OllamaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetResponseAsync(string prompt)
        {
            var endpoint = "http://localhost:11434/v1/completions";

            var requestBody = new
            {
                model = "phi3:latest",
                prompt = prompt
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var completionResponse = JsonDocument.Parse(jsonResponse);

                    var completionText = completionResponse.RootElement
                        .GetProperty("choices")[0]
                        .GetProperty("text")
                        .GetString();

                    return completionText ?? "No response";
                }
                else
                {
                    return $"Error: Service request failed. Status: {response.StatusCode}";
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
                return $"Error: Unable to connect to the Ollama service at {endpoint}. Please ensure the service is running.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
                return $"Error: {ex.Message}";
            }
        }
    }
}
