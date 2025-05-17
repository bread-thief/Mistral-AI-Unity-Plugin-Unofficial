using System.Collections;
using System.Collections.Generic;
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
        private static List<Message> history = new List<Message>();
        private static string currentResponse = "";

        public static string GetHistory() => string.Join("\n", history.ConvertAll(m => $"{m.GetRole()}: {m.GetContent()}"));

        public static string GetCurrentResponse() => currentResponse;

        public static void SendRequest(string request, MonoBehaviour monoBehaviour, bool writingUser = true)
        {
            if (string.IsNullOrEmpty(request))
                return;

            if (writingUser)
                history.Add(new Message("user", request));

            monoBehaviour.StartCoroutine(SendRequestEnumerator(request));
        }

        private static IEnumerator SendRequestEnumerator(string prompt)
        {
            currentResponse = "";
            var messages = new List<Message>(history) { new Message("user", prompt) };
            var requestData = new Request(GetModelName(), messages.ToArray());
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
                    if (response?.GetChoices() != null && response.GetChoices().Length > 0)
                    {
                        string reply = response.GetChoices()[0].GetMessage().GetContent();
                        currentResponse = reply;
                        history.Add(new Message("assistant", reply));
                    }
                    else
                    {
                        history.Add(new Message("assistant", "Empty answer."));
                    }
                }
                else
                {
                    MistralLogger.LogError($"Error: {request.error}");
                    MistralLogger.LogError($"Server response: {request.downloadHandler.text}");
                    history.Add(new Message("assistant", "Error receiving response."));
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
            private string model;
            [JsonProperty("messages")]
            private Message[] messages;

            public string GetModel() => model;

            public Message[] GetMessages() => messages;

            public Request(string model, Message[] messages)
            {
                this.model = model;
                this.messages = messages;
            }
        }

        public class Message
        {
            [JsonProperty("role")]
            private string role;
            [JsonProperty("content")]
            private string content;

            public string GetRole() => role;

            public string GetContent() => content;

            public Message(string role, string content)
            {
                this.role = role;
                this.content = content;
            }
        }

        public class Response
        {
            [JsonProperty("choices")]
            private Choice[] choices;

            public Choice[] GetChoices() => choices;
        }

        public class Choice
        {
            [JsonProperty("message")]
            private Message message;

            public Message GetMessage() => message;
        }

        public enum ModelType
        {
            MistralNemo,
            MistralSmall,
            CodestralMamba
        }
    }
}