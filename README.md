# How to integrate Microsoft Semantic Kernel (ChatGPT and Ollama) in a Blazor Web Application

For more information about **Microsoft.SemanticKernel** navigate to this URL: 

https://learn.microsoft.com/en-us/semantic-kernel/

## 1. Generate an OpenAI Key for ChatGPT

First of all for using the ChatGPT OpenAI APIs services you have to Sign Up in this web page:

https://auth.openai.com/

After Signing Up you can access the OpenAPI web page

![image](https://github.com/user-attachments/assets/20ecd685-eb1e-48c1-80e1-9f599901e2bd)

Now you have to create a API Key. For this purpose we navigate to this web page:

https://platform.openai.com/api-keys

Then we press in the **Create new secret key** button

![image](https://github.com/user-attachments/assets/e4bab1b3-0bca-4bdb-8166-5603e85d4ee7)

We copy the API Key for pasting in our C# application source code

![image](https://github.com/user-attachments/assets/f76ead90-2112-4879-9930-1fc6ca1c08d2)

![image](https://github.com/user-attachments/assets/24877c5d-6e53-4e4c-8528-c9ada1f0c817)

## 2. Download and Install Ollama in your local laptop

https://ollama.com/download

![image](https://github.com/user-attachments/assets/b89a70a5-a5ff-4f81-83e4-ea352d7d1fd9)

To verify the Ollama installation run these commands:

For downloading the phi3 model:

```
ollama run phi3
```

For listing the models:

```
ollama list
```

![image](https://github.com/user-attachments/assets/caf096b3-2d59-4534-b943-6685e3f1f3ef)

Verify Ollama is running

```
curl http://localhost:11434
```

![image](https://github.com/user-attachments/assets/f54ed356-d5b5-4e97-8652-e5abee40df6b)

Send a request:

```
curl -X POST http://localhost:11434/v1/completions ^
-H "Content-Type: application/json" ^
-d "{ \"model\": \"phi3:latest\", \"prompt\": \"hello\" }"
```

![image](https://github.com/user-attachments/assets/fd1abcd5-967a-44f8-adc4-82ed4d784783)

## 3. Create a Blazor Web Application


## 4. Create a Component for invoking ChatGPT

```razor
﻿@page "/AIChatGPT"
@inject IJSRuntime JS

<h3>Ask GPT-4 a Question</h3>

<div>
    <label>Enter a prompt:</label>
    <input type="text" @bind="userPrompt" />
</div>
<button @onclick="AskGPT4">Ask GPT-4</button>

@if (!string.IsNullOrEmpty(gptResponse))
{
    <div>
        <h4>Response:</h4>
        <p>@gptResponse</p>
    </div>
}

@code {
    private string userPrompt = string.Empty;
    private string gptResponse = string.Empty;

    private async Task AskGPT4()
    {
        if (string.IsNullOrWhiteSpace(userPrompt))
        {
            return;
        }

        gptResponse = await InvokeGPT4Async(userPrompt);
    }

    private async Task<string> InvokeGPT4Async(string prompt)
    {
        var builder = Kernel.CreateBuilder();

        builder.AddOpenAIChatCompletion(
            modelId: "gpt-4",   // Use GPT-4 model
            apiKey: "API-KEY"); // Replace with your actual OpenAI API key

        var kernel = builder.Build();

        // Define prompt execution settings
        var settings = new OpenAIPromptExecutionSettings
            {
                MaxTokens = 100,
                Temperature = 1
            };
        var kernelArguments = new KernelArguments(settings);

        // Use the user-provided prompt instead of hardcoding one
        var responseStream = kernel.InvokePromptStreamingAsync(prompt, kernelArguments);

        // Accumulate the result from the stream
        var responseBuilder = new StringBuilder();
        await foreach (var message in responseStream)
        {
            responseBuilder.Append(message.ToString());
        }

        return responseBuilder.ToString();
    }
}
```

## 5. Create a Component for invoking Ollama Phi3 

```razor
﻿@page "/AIChatPhi3"
@inject HttpClient Http  // Inject HttpClient for making the API call

<h3>Ask Phi-3 a Question</h3>

<div>
    <label>Enter a prompt:</label>
    <input type="text" @bind="userPrompt" />
</div>
<button @onclick="AskPhi3">Ask Phi-3</button>

@if (!string.IsNullOrEmpty(gptResponse))
{
    <div>
        <h4>Response:</h4>
        <p>@gptResponse</p>
    </div>
}

@code {
    private string userPrompt = string.Empty;
    private string gptResponse = string.Empty;

    private async Task AskPhi3()
    {
        if (string.IsNullOrWhiteSpace(userPrompt))
        {
            return;
        }

        gptResponse = await InvokePhi3Async(userPrompt);
    }

    private async Task<string> InvokePhi3Async(string prompt)
    {
        try
        {
            // Define the endpoint and model settings
            var endpoint = "http://localhost:11434/v1/completions";
            var requestBody = new
            {
                model = "phi3:latest",
                prompt = prompt
            };

            // Serialize the request body
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

            // Send the POST request
            var response = await Http.PostAsync(endpoint, content);

            if (response.IsSuccessStatusCode)
            {
                // Read and parse the response
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var completionResponse = System.Text.Json.JsonDocument.Parse(jsonResponse);

                // Extract the text from the "choices" array in the response
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
            return $"Error: Unable to connect to the Ollama service at http://localhost:11434. Please ensure the service is running.";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}
```

## 6. Create a Service for invoking ChatGPT

```csharp
﻿namespace BlazorAISample1.Services
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
```

## 7. Create a Service for invoking Ollama Phi3

```csharp
﻿using System.Text.Json;
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
```

## 8. Register the Services in the middleware 


```csharp
using BlazorAISample1.Components;
using BlazorAISample1.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(sp => new HttpClient());


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register the ChatGPTService
builder.Services.AddSingleton<ChatGPTService>();
builder.Services.AddScoped<OllamaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
```

## 9. Create a Component for consuming the ChatGPT service

```razor
﻿@page "/ServiceAIChatGPT"
@using BlazorAISample1.Services
@inject ChatGPTService GPTService
@inject IJSRuntime JS

<h3>Ask GPT-4 a Question</h3>

<div>
    <label>Enter a prompt:</label>
    <input type="text" @bind="userPrompt" />
</div>
<button @onclick="AskGPT4">Ask GPT-4</button>

@if (!string.IsNullOrEmpty(gptResponse))
{
    <div>
        <h4>Response:</h4>
        <p>@gptResponse</p>
    </div>
}

@code {
    private string userPrompt = string.Empty;
    private string gptResponse = string.Empty;

    private async Task AskGPT4()
    {
        if (string.IsNullOrWhiteSpace(userPrompt))
        {
            gptResponse = "Please enter a valid prompt.";
            return;
        }

        gptResponse = await GPTService.AskGPT4Async(userPrompt);
    }
}
```


## 10. Create a Component for consuming the Ollama Phi3 service

```razor
﻿@page "/ServiceAIChatPhi3"
@using BlazorAISample1.Services
@inject OllamaService OllamaService  // Inject the OllamaService for making the API call

<h3>Ask Phi-3 a Question</h3>

<div>
    <label>Enter a prompt:</label>
    <input type="text" @bind="userPrompt" />
</div>
<button @onclick="AskPhi3">Ask Phi-3</button>

@if (!string.IsNullOrEmpty(gptResponse))
{
    <div>
        <h4>Response:</h4>
        <p>@gptResponse</p>
    </div>
}

@code {
    private string userPrompt = string.Empty;
    private string gptResponse = string.Empty;

    private async Task AskPhi3()
    {
        if (string.IsNullOrWhiteSpace(userPrompt))
        {
            return;
        }

        gptResponse = await OllamaService.GetResponseAsync(userPrompt);
    }
}
```

## 11. Add the new components in the NavMenu.razor

```razor
 <div class="nav-item px-3">
            <NavLink class="nav-link" href="AIChatGPT">
                <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> AIChatGPT
            </NavLink>
        </div>
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="ServiceAIChatGPT">
                <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> ServiceAIChatGPT
            </NavLink>
        </div>
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="AIChatPhi3">
                <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> AIOllama
            </NavLink>
        </div>
        <div class="nav-item px-3">
            <NavLink class="nav-link" href="ServiceAIChatPhi3">
                <span class="bi bi-list-nested-nav-menu" aria-hidden="true"></span> ServiceAIChatPhi3
            </NavLink>
        </div>
```

## 12. Run the application and verify the results

![image](https://github.com/user-attachments/assets/5c686d06-f31e-401e-98fa-9f10bfe378ce)

We first invoke the **ChatGTP** component

![image](https://github.com/user-attachments/assets/27e6fedb-7b44-4743-8f4b-e3575fef959b)

We verify the component consuming the **ChatGPT** service

![image](https://github.com/user-attachments/assets/d8ca6469-e509-41ee-8878-fff5e0156b28)

We first invoke the **Ollama Phi3** component

![image](https://github.com/user-attachments/assets/b74d8c66-56cc-4823-aac7-f661ac5cbc79)

We verify the component consuming the **Ollama Phi3** service

![image](https://github.com/user-attachments/assets/7aa77ecd-4033-4ab2-b2a5-fe25d1c44d0b)

