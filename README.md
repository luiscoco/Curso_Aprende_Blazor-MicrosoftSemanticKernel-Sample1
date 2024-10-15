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

We run Visual Studio 2022 Community Edition and create a new project

![image](https://github.com/user-attachments/assets/e94a061c-64b6-40dd-a24f-9dcea73c64a1)

We select the Blazor Web App

![image](https://github.com/user-attachments/assets/e706d00f-d21c-4322-ad8c-862076792782)

We input the project name and location

![image](https://github.com/user-attachments/assets/facd8b35-3591-4b8f-8123-e696cffd15cf)

We select the .NET9 Framework and press the Create button

![image](https://github.com/user-attachments/assets/88b7ba61-b893-485a-b198-42781d8be1d1)

We verify the project folder and files structure, and we create the **Services** folder

![image](https://github.com/user-attachments/assets/797771e6-27f1-4c17-bc83-0286a57cf5d9)

## 4. Load the Nuget packages

![image](https://github.com/user-attachments/assets/ba37c193-3b78-48d7-ad74-9c3e14749f3e)

## 5. Create a Component for invoking ChatGPT

We create a new razor component **AIChatGTPComponent.razor**

![image](https://github.com/user-attachments/assets/d0b1f8a9-270d-408d-b3e4-f4b70a67bfe1)

This Blazor component allows users to input a prompt, sends that prompt to **GPT-4** via **OpenAI's API**, and then displays the response on the web page

The **GPT-4** interaction is handled asynchronously, with settings like token limit and temperature controlled programmatically

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

## 6. Create a Component for invoking Ollama Phi3 

We create a new razor component **AIOllamaComponent_.razor**

![image](https://github.com/user-attachments/assets/b5007803-a098-4034-ad6f-a5c7131470c3)

This Blazor component allows a user to send a prompt to the **Ollama Phi-3** model via an API and displays the response

It makes an asynchronous **HTTP POST** request to a local API endpoint (**http://localhost:11434/v1/completions**), sends the user’s prompt in the request body, and retrieves and displays the result

The code handles errors gracefully, returning appropriate messages when the request fails or encounters issues

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

## 7. Create a Service for invoking ChatGPT

We create the **ChatGPTService.cs** file for defining the **ChatGPT-4** service

![image](https://github.com/user-attachments/assets/d2fbfd12-50ac-4efc-a867-118ae0f70373)

This service allows you to send prompts to the **GPT-4** model from a Blazor app

It uses **Microsoft's Semantic Kernel** framework to manage communication with **OpenAI's API** and streams responses back to the caller asynchronously

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

## 8. Create a Service for invoking Ollama Phi3

We create the **OllamaService.cs** file for defining the **Ollama Phi-3** service

![image](https://github.com/user-attachments/assets/d6641b78-4713-4ec2-a3ed-4335a323010f)

The **OllamaService** class is designed to send prompts to an AI service and retrieve generated responses

It uses an asynchronous **HTTP POST** request, serializes the request body to JSON, handles the response by parsing the returned JSON, and includes error handling for both HTTP and general exceptions

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

## 9. Register the Services in the middleware (Program.cs)

We add this code in the **Program.cs** for registering the **ChatGPT-4** and **Ollama Phi-3** services in the Blazor applicatin

```csharp
builder.Services.AddSingleton<ChatGPTService>();
builder.Services.AddScoped<OllamaService>();
```

See the **Program.cs** location in the project structure

![image](https://github.com/user-attachments/assets/bfce138a-13bf-46bb-ae41-c9e507c0b421)

This is the whole middleware (**Program.cs**) source code:

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

## 10. Create a Component for consuming the ChatGPT service

We create a new razor component **AIChatGPTinjectedComponent.razor** forn invoking the **ChatGPT-4** Service

![image](https://github.com/user-attachments/assets/ed8126c4-b954-46cd-ac47-37f45941016d)

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

## 11. Create a Component for consuming the Ollama Phi3 service

We create a new razor component **AIOllamaComponentInjectedService.razor** forn invoking the **ChatGPT-4** Service

![image](https://github.com/user-attachments/assets/3154fddd-7d50-46c4-8f96-500459cc2791)

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

## 12. Add the new components in the NavMenu.razor

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

## 13. Run the application and verify the results

![image](https://github.com/user-attachments/assets/5c686d06-f31e-401e-98fa-9f10bfe378ce)

We first invoke the **ChatGTP** component

![image](https://github.com/user-attachments/assets/27e6fedb-7b44-4743-8f4b-e3575fef959b)

We verify the component consuming the **ChatGPT** service

![image](https://github.com/user-attachments/assets/d8ca6469-e509-41ee-8878-fff5e0156b28)

We first invoke the **Ollama Phi3** component

![image](https://github.com/user-attachments/assets/b74d8c66-56cc-4823-aac7-f661ac5cbc79)

We verify the component consuming the **Ollama Phi3** service

![image](https://github.com/user-attachments/assets/7aa77ecd-4033-4ab2-b2a5-fe25d1c44d0b)

