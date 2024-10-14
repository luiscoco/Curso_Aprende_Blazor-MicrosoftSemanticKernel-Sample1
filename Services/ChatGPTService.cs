namespace BlazorAISample1.Services
{
    using System.Text;
    using Microsoft.SemanticKernel;
    using Microsoft.SemanticKernel.ChatCompletion;
    using Microsoft.SemanticKernel.Connectors.OpenAI;
    using System.Threading.Tasks;
  
    public class ChatGPTService
    {
        public readonly Kernel _kernel;

        public ChatGPTService()
        {
            var builder = Kernel.CreateBuilder();

            // Configure the OpenAI GPT-4 model with your API key
            builder.AddOpenAIChatCompletion(
                modelId: "gpt-4",
                apiKey: "API-KEY"); // Replace with your actual API key

            _kernel = builder.Build();
        }

        public async Task<string> AskGPT4Async(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                return "Prompt cannot be empty!";
            }

            var settings = new OpenAIPromptExecutionSettings
            {
                MaxTokens = 100,
                Temperature = 1
            };

            var kernelArguments = new KernelArguments(settings);

            var responseStream = _kernel.InvokePromptStreamingAsync(prompt, kernelArguments);
            var responseBuilder = new StringBuilder();

            await foreach (var message in responseStream)
            {
                responseBuilder.Append(message.ToString());
            }

            return responseBuilder.ToString();
        }
    }
}
