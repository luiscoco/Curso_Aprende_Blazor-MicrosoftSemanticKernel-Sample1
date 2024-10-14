# How to integrate Microsoft Semantic Kernel (ChatGPT and Ollama) in a Blazor Web Application

## 1. Generate an OpenAI Key for ChatGPT

https://platform.openai.com/api-keys

## 2. Download and Install Ollama in your local laptop

https://ollama.com/download

![image](https://github.com/user-attachments/assets/b89a70a5-a5ff-4f81-83e4-ea352d7d1fd9)

To verify the Ollama installation run these commands:

For running the phi3 model:

```
ollama run phi3
```

For listing the models:

```
ollama list
```
![image](https://github.com/user-attachments/assets/caf096b3-2d59-4534-b943-6685e3f1f3ef)

Check Ollama is running

```
curl http://localhost:11434
```

Send a request:

```
curl -X POST http://localhost:11434/v1/completions ^
-H "Content-Type: application/json" ^
-d "{ \"model\": \"phi3:latest\", \"prompt\": \"hello\" }"
```

![image](https://github.com/user-attachments/assets/fd1abcd5-967a-44f8-adc4-82ed4d784783)

## 3. Create a Blazor Web Application


## 4. 

