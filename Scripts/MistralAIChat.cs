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
        /// <summary>
        /// Retrieves the API key from the configuration settings.
        /// </summary>
        /// <returns>The API key as a string, or an empty string if not set.</returns>
        public static string GetApiKey()
        {
            var settings = MistralConfigurationWindow.GetSettings();
            return settings != null && !string.IsNullOrEmpty(settings.ApiKey) ? settings.ApiKey : "";
        }

        /// <summary>
        /// Retrieves the API URL from the configuration settings.
        /// </summary>
        /// <returns>The API URL as a string, or an empty string if not set.</returns>
        public static string GetApiUrl()
        {
            var settings = MistralConfigurationWindow.GetSettings();
            return settings != null && !string.IsNullOrEmpty(settings.ApiUrl) ? settings.ApiUrl : "";
        }

        /// <summary>
        /// Retrieves the selected model type from the configuration settings.
        /// </summary>
        /// <returns>The ModelType enum value.</returns>
        public static ModelType GetModelType()
        {
            var settings = MistralConfigurationWindow.GetSettings();
            return settings != null ? settings.Model : ModelType.MistralNemo;
        }
    }

    public static class MistralAIChat
    {
        // ==============================================
        // ==================Variables===================
        // ==============================================
        #region Variables
        private static List<Message> history = new List<Message>();
        private static string currentResponse = "";
        private static List<float> messageTimestamps = new List<float>();
        private static bool hasResponded = true;
        #endregion

        // ==============================================
        // ===================Function===================
        // ==============================================
        #region Function
        /// <summary>
        /// Checks if the AI has responded to the last request.
        /// </summary>
        /// <returns>True if the AI has responded; otherwise, false.</returns>
        public static bool GetHasResponded() => hasResponded;

        /// <summary>
        /// Sends a request to the AI using the last user message in the conversation history.
        /// Initiates the response process if the AI hasn't responded yet.
        /// </summary>
        /// <param name="monoBehaviour">The MonoBehaviour to run the coroutine.</param>
        public static void ReplyToLastMessage(MonoBehaviour monoBehaviour)
        {
            if (history.Count == 0 || !hasResponded)
                return;

            string lastUserMessage = history[history.Count - 1].GetContent();
            SendRequest(lastUserMessage, monoBehaviour);
        }

        /// <summary>
        /// Gets the entire conversation history as a formatted string.
        /// </summary>
        /// <returns>A string containing the roles and contents of all messages.</returns>
        public static string GetHistory() => string.Join("\n", history.ConvertAll(m => $"\n{m.GetRole()}: {m.GetContent()}\n"));

        /// <summary>
        /// Clears the current conversation history.
        /// </summary>
        public static void ClearHistory() => history.Clear();

        /// <summary>
        /// Gets the number of messages in the conversation history.
        /// </summary>
        /// <returns>The count of messages.</returns>
        public static int GetHistoryCount() => history.Count;

        /// <summary>
        /// Retrieves the message at the specified index in the history.
        /// </summary>
        /// <param name="index">The index of the message to retrieve.</param>
        /// <returns>The Message object if index is valid; otherwise, null.</returns>
        public static Message GetMessageAt(int index)
        {
            if (index >= 0 && index < history.Count)
                return history[index];
            return null;
        }

        /// <summary>
        /// Gets the current AI response.
        /// </summary>
        /// <returns>The latest response string from the AI.</returns>
        public static string GetCurrentResponse() => currentResponse;

        /// <summary>
        /// Saves the conversation history to a JSON file at the specified path.
        /// </summary>
        /// <param name="filePath">The file path where the history will be saved.</param>
        public static void SaveHistoryToFile(string filePath)
        {
            List<Dictionary<string, string>> historyData = new List<Dictionary<string, string>>();
            foreach (var msg in history)
            {
                historyData.Add(new Dictionary<string, string>
                {
                    { "role", msg.GetRole() },
                    { "content", msg.GetContent() }
                });
            }

            string json = JsonConvert.SerializeObject(historyData, Formatting.Indented);
            System.IO.File.WriteAllText(filePath, json);
            Debug.Log($"History saved to: {filePath}");
        }

        /// <summary>
        /// Loads conversation history from a JSON file at the specified path.
        /// </summary>
        /// <param name="filePath">The file path to load the history from.</param>
        public static void LoadHistoryFromFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                Debug.LogWarning($"File not found: {filePath}");
                return;
            }

            string json = System.IO.File.ReadAllText(filePath);
            var historyData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);
            history.Clear();
            foreach (var item in historyData)
            {
                if (item.ContainsKey("role") && item.ContainsKey("content"))
                    history.Add(new Message(item["role"], item["content"]));
            }
            Debug.Log("History loaded!");
        }

        /// <summary>
        /// Gets a dictionary counting messages by each role.
        /// </summary>
        /// <returns>A dictionary with roles as keys and message counts as values.</returns>
        public static Dictionary<string, int> GetMessageCountByRole()
        {
            Dictionary<string, int> counts = new Dictionary<string, int>();
            foreach (var msg in history)
            {
                string role = msg.GetRole();
                if (counts.ContainsKey(role))
                    counts[role]++;
                else
                    counts[role] = 1;
            }
            return counts;
        }

        /// <summary>
        /// Searches the conversation history for messages containing the specified keyword.
        /// </summary>
        /// <param name="keyword">The keyword to search for.</param>
        /// <returns>A list of messages that contain the keyword.</returns>
        public static List<Message> SearchMessages(string keyword)
        {
            List<Message> results = new List<Message>();
            foreach (var msg in history)
            {
                if (msg.GetContent().ToLower().Contains(keyword.ToLower()))
                    results.Add(msg);
            }
            return results;
        }

        /// <summary>
        /// Edits the content of a message at the specified index.
        /// </summary>
        /// <param name="index">The index of the message to edit.</param>
        /// <param name="newContent">The new content for the message.</param>
        /// <returns>True if the message was successfully edited; otherwise, false.</returns>
        public static bool EditMessage(int index, string newContent)
        {
            if (index >= 0 && index < history.Count)
            {
                Message msg = history[index];
                Message updatedMsg = new Message(msg.GetRole(), newContent);
                history[index] = updatedMsg;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates the total time spent in the current dialog based on message timestamps.
        /// </summary>
        /// <returns>The total dialog time in seconds.</returns>
        public static float GetTotalDialogTime()
        {
            if (messageTimestamps.Count < 2)
                return 0f;
            return messageTimestamps[messageTimestamps.Count - 1] - messageTimestamps[0];
        }

        /// <summary>
        /// Exports the conversation history in markdown format.
        /// </summary>
        /// <returns>A string containing the formatted conversation history.</returns>
        public static string ExportHistoryToMarkdown()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var msg in history)
                sb.AppendLine($"**{msg.GetRole()}:** {msg.GetContent()}\n");
            return sb.ToString();
        }
        #endregion

        // ==============================================
        // ================RequestMethods================
        // ==============================================
        #region Request Methods
        /// <summary>
        /// Sends a request to the AI with the specified user input.
        /// Uses default API key, URL, and model type from configuration.
        /// </summary>
        /// <param name="request">The user input message.</param>
        /// <param name="monoBehaviour">The MonoBehaviour to run the coroutine.</param>
        public static void SendRequest(string request, MonoBehaviour monoBehaviour)
        {
            if (!hasResponded)
            {
                Debug.LogWarning("AI has not responded to the previous request, sending is unavailable.");
                return;
            }
            SendRequestHandler(request, monoBehaviour, Data.GetApiKey(), Data.GetApiUrl(), Data.GetModelType());
        }

        /// <summary>
        /// Sends a request with a specified API key, using default URL and model type.
        /// </summary>
        /// <param name="request">The user input message.</param>
        /// <param name="monoBehaviour">The MonoBehaviour to run the coroutine.</param>
        /// <param name="apiKey">The API key to use for the request.</param>
        public static void SendRequest(string request, MonoBehaviour monoBehaviour, string apiKey) => SendRequestHandler(request, monoBehaviour, apiKey, Data.GetApiUrl(), Data.GetModelType());

        /// <summary>
        /// Sends a request with specified API key and model type.
        /// </summary>
        /// <param name="request">The user input message.</param>
        /// <param name="monoBehaviour">The MonoBehaviour to run the coroutine.</param>
        /// <param name="apiKey">The API key to use.</param>
        /// <param name="modelType">The model type to use for the request.</param>
        public static void SendRequest(string request, MonoBehaviour monoBehaviour, string apiKey, ModelType modelType) => SendRequestHandler(request, monoBehaviour, apiKey, Data.GetApiUrl(), modelType);

        /// <summary>
        /// Sends a request with specified model type, using default API key and URL.
        /// </summary>
        /// <param name="request">The user input message.</param>
        /// <param name="monoBehaviour">The MonoBehaviour to run the coroutine.</param>
        /// <param name="modelType">The model type to use for the request.</param>
        public static void SendRequest(string request, MonoBehaviour monoBehaviour, ModelType modelType) => SendRequestHandler(request, monoBehaviour, Data.GetApiKey(), Data.GetApiUrl(), modelType);

        /// <summary>
        /// The core method that handles sending the request with all parameters specified.
        /// Initiates the coroutine to perform the web request.
        /// </summary>
        /// <param name="request">The user input message.</param>
        /// <param name="monoBehaviour">The MonoBehaviour to start the coroutine.</param>
        /// <param name="apiKey">API key for authorization.</param>
        /// <param name="apiUrl">API URL endpoint.</param>
        /// <param name="modelType">Model type to specify which AI model to use.</param>
        private static void SendRequestHandler(string request, MonoBehaviour monoBehaviour, string apiKey, string apiUrl, ModelType modelType)
        {
            if (string.IsNullOrEmpty(request))
                return;
            // Add the user message to history
            history.Add(new Message("user", request));
            // Start the coroutine to send the request and handle response
            monoBehaviour.StartCoroutine(SendRequestEnumerator(request, apiKey, apiUrl, modelType));
            AddMessageTimestamp();
            hasResponded = false;
        }

        /// <summary>
        /// Coroutine that manages sending the web request and processing the response asynchronously.
        /// </summary>
        /// <param name="prompt">The user input prompt.</param>
        /// <param name="apiKey">API key for authorization.</param>
        /// <param name="apiUrl">API URL endpoint.</param>
        /// <param name="modelType">Model type to specify which AI model to use.</param>
        /// <returns>An IEnumerator for coroutine execution.</returns>
        private static IEnumerator SendRequestEnumerator(string prompt, string apiKey, string apiUrl, ModelType modelType)
        {
            currentResponse = "";
            var messages = new List<Message>(history) { new Message("user", prompt) };
            var requestData = new Request(GetModelName(modelType), messages.ToArray());
            string jsonData = JsonConvert.SerializeObject(requestData);

            using (UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
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
                        hasResponded = true;
                    }
                    else
                    {
                        Debug.LogWarning("Received an empty response from AI.");
                        history.Add(new Message("assistant", "Empty response."));
                        hasResponded = true;
                    }
                }
                else
                {
                    Debug.LogError($"Error: {request.error}");
                    Debug.LogError($"Server response: {request.downloadHandler.text}");
                    history.Add(new Message("assistant", "Error retrieving response."));
                    hasResponded = true;
                }
            }
        }

        /// <summary>
        /// Records the timestamp of a message sent or received.
        /// </summary>
        private static void AddMessageTimestamp() => messageTimestamps.Add(Time.time);

        /// <summary>
        /// Converts a ModelType enum value to the corresponding model name string used by the API.
        /// </summary>
        /// <param name="modelType">The ModelType enum value.</param>
        /// <returns>The string name of the model for API requests.</returns>
        private static string GetModelName(ModelType modelType)
        {
            return modelType switch
            {
                ModelType.MistralNemo => "open-mistral-nemo",
                ModelType.MistralSmall => "mistral-small-latest",
                ModelType.CodestralMamba => "open-codestral-mamba",
                _ => "error",
            };
        }
        #endregion
    }

    namespace Components
    {
        /// <summary>
        /// Represents a request payload to send to the AI API.
        /// </summary>
        public class Request
        {
            [JsonProperty("model")]
            private string model;

            [JsonProperty("messages")]
            private Message[] messages;

            /// <summary>
            /// Gets the model name.
            /// </summary>
            /// <returns>The model name as a string.</returns>
            public string GetModel() => model;

            /// <summary>
            /// Gets the array of messages.
            /// </summary>
            /// <returns>An array of Message objects.</returns>
            public Message[] GetMessages() => messages;

            /// <summary>
            /// Initializes a new instance of the Request class.
            /// </summary>
            /// <param name="model">Model name.</param>
            /// <param name="messages">Array of messages.</param>
            public Request(string model, Message[] messages)
            {
                this.model = model;
                this.messages = messages;
            }
        }

        /// <summary>
        /// Represents a message in the conversation.
        /// </summary>
        public class Message
        {
            [JsonProperty("role")]
            private string role;

            [JsonProperty("content")]
            private string content;

            /// <summary>
            /// Gets the role of the message sender (e.g., user, assistant).
            /// </summary>
            /// <returns>The role as a string.</returns>
            public string GetRole() => role;

            /// <summary>
            /// Gets the message content.
            /// </summary>
            /// <returns>The message content as a string.</returns>
            public string GetContent() => content;

            /// <summary>
            /// Initializes a new instance of the Message class.
            /// </summary>
            /// <param name="role">The role of the sender.</param>
            /// <param name="content">The message content.</param>
            public Message(string role, string content)
            {
                this.role = role;
                this.content = content;
            }
        }

        /// <summary>
        /// Represents the response from the AI API.
        /// </summary>
        public class Response
        {
            [JsonProperty("choices")]
            private Choice[] choices;

            /// <summary>
            /// Gets the array of choices returned by the API.
            /// </summary>
            /// <returns>An array of Choice objects.</returns>
            public Choice[] GetChoices() => choices;
        }

        /// <summary>
        /// Represents a single choice in the AI response.
        /// </summary>
        public class Choice
        {
            [JsonProperty("message")]
            private Message message;

            /// <summary>
            /// Gets the message associated with this choice.
            /// </summary>
            /// <returns>The Message object.</returns>
            public Message GetMessage() => message;
        }

        /// <summary>
        /// Enumeration of available AI model types.
        /// </summary>
        public enum ModelType
        {
            MistralNemo,
            MistralSmall,
            CodestralMamba
        }
    }
}