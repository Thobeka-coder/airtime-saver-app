import os
import requests
import json

endpoint = os.getenv("ENDPOINT_URL", "https://airtime-saver.openai.azure.com/")
deployment = os.getenv("DEPLOYMENT_NAME", "gpt-4.1-mini")
search_endpoint = os.getenv("SEARCH_ENDPOINT", "https://airtime-saver.search.windows.net")
search_key = os.getenv("SEARCH_KEY", "put your Azure AI Search admin key here")
search_index = os.getenv("SEARCH_INDEX_NAME", "airtimesaveridx")
subscription_key = os.getenv("AZURE_OPENAI_API_KEY", "API Key")

api_version = "2025-01-01-preview"

url = f"{endpoint}openai/deployments/{deployment}/chat/completions?api-version={api_version}"

headers = {
    "Content-Type": "application/json",
    "api-key": subscription_key
}

#Prepare the chat prompt
chat_prompt = [
    {
        "role": "system",
        "content": "You are an AI assistant that helps people find information."
    },
    {
        "role": "user",
        "content": "Hello, can you help me learn about Old Mutual products?"
    }
]

# Include speech result if speech is enabled
messages = chat_prompt

# Prepare the request body
body = {
    "messages": messages,
    "max_tokens": 800,
    "temperature": 1,
    "top_p": 1,
    "frequency_penalty": 0,
    "presence_penalty": 0,
    "stop": None,
    "stream": False,
    "data_sources": [
        {
            "type": "azure_search",
            "parameters": {
                "endpoint": search_endpoint,
                "index_name": search_index,
                "authentication": {
                    "type": "api_key",
                    "key": search_key
                }
            }
        }
    ]
}

# Remove None values from body (API does not accept null/None for stop)
body = {k: v for k, v in body.items() if v is not None}

# Send the request
response = requests.post(url, headers=headers, data=json.dumps(body))

# Print the response
print(response.status_code)
print(response.text)
