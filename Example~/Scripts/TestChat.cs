using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Mistral.AI;

public class TestChat : MonoBehaviour
{
    [SerializeField] private TMP_InputField userInputField;
    [SerializeField] private TMP_InputField chatHistoryInputField;
    [SerializeField] private Button sendButton;

    private void Start()
    {
        sendButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (!string.IsNullOrEmpty(userInputField.text))
        {
            MistralAIChat.SendRequest(userInputField.text, this, true);
            userInputField.text = "";
        }
    }

    private void Update() => chatHistoryInputField.text = MistralAIChat.GetHistory();
}