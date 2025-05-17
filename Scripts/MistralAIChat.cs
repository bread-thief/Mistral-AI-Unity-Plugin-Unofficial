using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Mistral.AI.Logger;
using Mistral.AI.Components;

namespace Mistral.AI
{
    public static class Data
    {
        public static string GetApiKey()
        {
            var settings = MistralConfigurationWindow.GetSettings();
            return settings != null && !string.IsNullOrEmpty(settings.ApiKey) ? settings.ApiKey : "";
        }

        public static string GetApiUrl()
        {
            var settings = MistralConfigurationWindow.GetSettings();
            return settings != null && !string.IsNullOrEmpty(settings.ApiUrl) ? settings.ApiUrl : "";
        }

        public static ModelType GetModelType()
        {
            var settings = MistralConfigurationWindow.GetSettings();
            return settings != null ? settings.Model : ModelType.MistralNemo;
        }
    }

    public static class MistralAIChat
    {
        private static string history = "";
        private static string currentResponse = "";

        public static string GetHistory() => history;
        public static string GetCurrentResponse() => currentResponse;

        public static void SendRequest(string request, MonoBehaviour monoBehaviour, bool writingUser = true)
        {
            if (string.IsNullOrEmpty(request))
                return;

            if (writingUser)
                AppendChat($"\nYou: {request}\n");

            monoBehaviour.StartCoroutine(SendRequestEnumerator(request));
        }

        private static void AppendChat(string message) => history += message;

        private static IEnumerator SendRequestEnumerator(string prompt)
        {
            currentResponse = "";
            var messages = new Message[] { new Message("user", prompt) };
            var requestData = new Request(GetModelName(), messages);
            string jsonData = JsonConvert.SerializeObject(requestData);

            using (UnityWebRequest request = UnityWebRequest.PostWwwForm(Data.GetApiUrl(), "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {Data.GetApiKey()}");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseJson = request.downloadHandler.text;
                    var response = JsonConvert.DeserializeObject<Response>(responseJson);
                    if (response?.Choices != null && response.Choices.Length > 0)
                    {
                        string reply = response.Choices[0].Message.Content;
                        currentResponse = reply;
                        AppendChat($"\nAssistant: {reply}\n");
                    }
                    else
                    {
                        AppendChat("Empty answer.\n");
                    }
                }
                else
                {
                    MistralLogger.LogError($"Error: {request.error}");
                    MistralLogger.LogError($"Server response: {request.downloadHandler.text}");
                    AppendChat("Error receiving response.\n");
                }
            }
        }

        private static string GetModelName()
        {
            return Data.GetModelType() switch
            {
                ModelType.MistralNemo => "open-mistral-nemo",
                ModelType.MistralSmall => "mistral-small-latest",
                ModelType.CodestralMamba => "open-codestral-mamba",
                _ => "error",
            };
        }
    }

    namespace Components
    {
        public class Request
        {
            [JsonProperty("model")]
            public string Model { get; set; }
            [JsonProperty("messages")]
            public Message[] Messages { get; set; }
            public Request(string model, Message[] messages)
            {
                Model = model;
                Messages = messages;
            }
        }

        public class Message
        {
            [JsonProperty("role")]
            public string Role { get; set; }
            [JsonProperty("content")]
            public string Content { get; set; }
            public Message(string role, string content)
            {
                Role = role;
                Content = content;
            }
        }

        public class Response
        {
            [JsonProperty("choices")]
            public Choice[] Choices { get; set; }
        }

        public class Choice
        {
            [JsonProperty("message")]
            public Message Message { get; set; }
        }

        public enum ModelType
        {
            MistralNemo,
            MistralSmall,
            CodestralMamba
        }
    }
}