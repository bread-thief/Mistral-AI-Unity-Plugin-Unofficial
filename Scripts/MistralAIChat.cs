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
#if UNITY_EDITOR
            MistralConfigurationWindow.ShowConfigurationWindow();
            var settings = MistralConfigurationWindow.GetSettings();
            return settings != null && !string.IsNullOrEmpty(settings.ApiKey) ? settings.ApiKey : "";
#else
            return "";
#endif
        }

        /// <summary>
        /// Retrieves the API URL from the configuration settings.
        /// </summary>
        /// <returns>The API URL as a string, or an empty string if not set.</returns>
        public static string GetApiUrl()
        {
#if UNITY_EDITOR
            MistralConfigurationWindow.ShowConfigurationWindow();
            var settings = MistralConfigurationWindow.GetSettings();
            return settings != null && !string.IsNullOrEmpty(settings.ApiUrl) ? settings.ApiUrl : "";
#else
            return "https://api.mistral.ai/v1/chat/completions";
#endif
        }

        /// <summary>
        /// Retrieves the selected model type from the configuration settings.
        /// </summary>
        /// <returns>The ModelType enum value.</returns>
        public static ModelType GetModelType()
        {
#if UNITY_EDITOR
            MistralConfigurationWindow.ShowConfigurationWindow();
            var settings = MistralConfigurationWindow.GetSettings();
            return settings != null ? settings.Model : ModelType.MistralNemo;
#else
            return ModelType.MistralNemo;
#endif
        }
    }

    public static class MistralAIChat
    {
        // ==============================================
        // ==================Variables===================
        // ==============================================
        #region Variables
        private static List<Message> _history = new List<Message>();
        private static string _currentResponse = "";
        private static List<float> _messageTimestamps = new List<float>();
        private static bool _hasResponded = true;
        #endregion

        // ==============================================
        // ===================Function===================
        // ==============================================
        #region Function
        /// <summary>
        /// Checks if the AI has responded to the last request.
        /// </summary>
        /// <returns>True if the AI has responded; otherwise, false.</returns>
        public static bool GetHasResponded() => _hasResponded;

        /// <summary>
        /// Sends a request to the AI using the last user message in the conversation history.
        /// Initiates the response process if the AI hasn't responded yet.
        /// </summary>
        /// <param name="monoBehaviour">The MonoBehaviour to run the coroutine.</param>
        public static void ReplyToLastMessage(MonoBehaviour monoBehaviour)
        {
            if (_history.Count == 0 || !_hasResponded)
                return;

            string lastUserMessage = _history[_history.Count - 1].GetContent();
            SendRequest(lastUserMessage, monoBehaviour);
        }

        /// <summary>
        /// Gets the entire conversation history as a formatted string.
        /// </summary>
        /// <returns>A string containing the roles and contents of all messages.</returns>
        public static string GetHistory() => string.Join("\n", _history.ConvertAll(m => $"\n{m.GetRole()}: {m.GetContent()}\n"));

        /// <summary>
        /// Clears the current conversation history.
        /// </summary>
        public static void ClearHistory() => _history.Clear();

        /// <summary>
        /// Gets the number of messages in the conversation history.
        /// </summary>
        /// <returns>The count of messages.</returns>
        public static int GetHistoryCount() => _history.Count;

        /// <summary>
        /// Retrieves the message at the specified index in the history.
        /// </summary>
        /// <param name="index">The index of the message to retrieve.</param>
        /// <returns>The Message object if index is valid; otherwise, null.</returns>
        public static Message GetMessageAt(int index)
        {
            if (index >= 0 && index < _history.Count)
                return _history[index];
            return null;
        }

        /// <summary>
        /// Gets the current AI response.
        /// </summary>
        /// <returns>The latest response string from the AI.</returns>
        public static string GetCurrentResponse() => _currentResponse;

        /// <summary>
        /// Saves the conversation history to a JSON file at the specified path.
        /// </summary>
        /// <param name="filePath">The file path where the history will be saved.</param>
        public static void SaveHistoryToFile(string filePath)
        {
            List<Dictionary<string, string>> historyData = new List<Dictionary<string, string>>();
            foreach (var msg in _history)
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
            _history.Clear();
            foreach (var item in historyData)
            {
                if (item.ContainsKey("role") && item.ContainsKey("content"))
                    _history.Add(new Message(item["role"], item["content"]));
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
            foreach (var msg in _history)
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
            foreach (var msg in _history)
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
            if (index >= 0 && index < _history.Count)
            {
                Message msg = _history[index];
                Message updatedMsg = new Message(msg.GetRole(), newContent);
                _history[index] = updatedMsg;
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
            if (_messageTimestamps.Count < 2)
                return 0f;
            return _messageTimestamps[_messageTimestamps.Count - 1] - _messageTimestamps[0];
        }

        /// <summary>
        /// Exports the conversation history in markdown format.
        /// </summary>
        /// <returns>A string containing the formatted conversation history.</returns>
        public static string ExportHistoryToMarkdown()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            foreach (var msg in _history)
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
            if (!_hasResponded)
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

        private static void SendRequestHandler(string request, MonoBehaviour monoBehaviour, string apiKey, string apiUrl, ModelType modelType)
        {
            if (string.IsNullOrEmpty(request))
                return;
            _history.Add(new Message("user", request));
            monoBehaviour.StartCoroutine(SendRequestEnumerator(request, apiKey, apiUrl, modelType));
            AddMessageTimestamp();
            _hasResponded = false;
        }

        private static IEnumerator SendRequestEnumerator(string prompt, string apiKey, string apiUrl, ModelType modelType)
        {
            _currentResponse = "";
            var messages = new List<Message>(_history) { new Message("user", prompt) };
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
                        _currentResponse = reply;
                        _history.Add(new Message("assistant", reply));
                        _hasResponded = true;
                    }
                    else
                    {
                        Debug.LogWarning("Received an empty response from AI.");
                        _history.Add(new Message("assistant", "Empty response."));
                        _hasResponded = true;
                    }
                }
                else
                {
                    Debug.LogError($"Error: {request.error}");
                    Debug.LogError($"Server response: {request.downloadHandler.text}");
                    _history.Add(new Message("assistant", "Error retrieving response."));
                    _hasResponded = true;
                }
            }
        }

        private static void AddMessageTimestamp() => _messageTimestamps.Add(Time.time);

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
            private string _model;

            [JsonProperty("messages")]
            private Message[] _messages;

            /// <summary>
            /// Gets the model name.
            /// </summary>
            /// <returns>The model name as a string.</returns>
            public string GetModel() => _model;

            /// <summary>
            /// Gets the array of messages.
            /// </summary>
            /// <returns>An array of Message objects.</returns>
            public Message[] GetMessages() => _messages;

            /// <summary>
            /// Initializes a new instance of the Request class.
            /// </summary>
            /// <param name="model">Model name.</param>
            /// <param name="messages">Array of messages.</param>
            public Request(string model, Message[] messages)
            {
                this._model = model;
                this._messages = messages;
            }
        }

        /// <summary>
        /// Represents a message in the conversation.
        /// </summary>
        public class Message
        {
            [JsonProperty("role")]
            private string _role;

            [JsonProperty("content")]
            private string _content;

            /// <summary>
            /// Gets the role of the message sender (e.g., user, assistant).
            /// </summary>
            /// <returns>The role as a string.</returns>
            public string GetRole() => _role;

            /// <summary>
            /// Gets the message content.
            /// </summary>
            /// <returns>The message content as a string.</returns>
            public string GetContent() => _content;

            /// <summary>
            /// Initializes a new instance of the Message class.
            /// </summary>
            /// <param name="role">The role of the sender.</param>
            /// <param name="content">The message content.</param>
            public Message(string role, string content)
            {
                this._role = role;
                this._content = content;
            }
        }

        /// <summary>
        /// Represents the response from the AI API.
        /// </summary>
        public class Response
        {
            [JsonProperty("choices")]
            private Choice[] _choices;

            /// <summary>
            /// Gets the array of choices returned by the API.
            /// </summary>
            /// <returns>An array of Choice objects.</returns>
            public Choice[] GetChoices() => _choices;
        }

        /// <summary>
        /// Represents a single choice in the AI response.
        /// </summary>
        public class Choice
        {
            [JsonProperty("message")]
            private Message _message;

            /// <summary>
            /// Gets the message associated with this choice.
            /// </summary>
            /// <returns>The Message object.</returns>
            public Message GetMessage() => _message;
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