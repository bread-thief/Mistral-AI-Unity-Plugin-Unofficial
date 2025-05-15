using Mistral.AI.Components;
using UnityEngine;

[CreateAssetMenu(fileName = "MistralApiSettings", menuName = "MistralAI/Settings")]
public class MistralApiSettings : ScriptableObject
{
	[field: SerializeField, Tooltip("API key (More details: MistralAI/Help/ApiKey)")] public string ApiKey { get; private set; } = "YOUR_MISTRAL_API_KEY";
	[field: SerializeField, Tooltip("API URL (More details: MistralAI/Help/ApiURL)")] public string ApiUrl { get; private set; } = "https://api.mistral.ai/v1/chat/completions";
	[field: SerializeField, Tooltip("Models (More details: MistralAI/Help/Models)")] public ModelType Model { get; private set; } = ModelType.MistralNemo;

	public void SetSettings(string apiKey, string apiUrl, ModelType model)
	{
		ApiKey = apiKey;
		ApiUrl = apiUrl;
		Model = model;
	}
}