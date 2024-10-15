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


## 5. Create a Component for invoking Ollama Phi3 


## 6. Create a Service for invoking ChatGPT


## 7. Create a Service for invoking Ollama Phi3



## 8. Register the Services in the middleware 


## 9. Create a Component for consuming the ChatGPT service


## 10. Create a Component for consuming the Ollama Phi3 service


## 11. Add the new components in the NavMenu.razor


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

