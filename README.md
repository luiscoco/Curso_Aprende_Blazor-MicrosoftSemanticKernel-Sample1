# How to integrate Microsoft Semantic Kernel (ChatGPT and Ollama) in a Blazor Web Application

## 1. Generate an OpenAI Key for ChatGPT

https://platform.openai.com/api-keys

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

