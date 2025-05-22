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
        userInputField.onSubmit.AddListener(OnSubmitInputField);
        sendButton.onClick.AddListener(OnButtonClick);
    }

    private void OnSubmitInputField(string text) => OnButtonClick();

    private void OnButtonClick()
    {
        string message = userInputField.text;
        if (!string.IsNullOrEmpty(message) && !MistralAIChat.GetHasResponded())
        {
            MistralAIChat.SendRequest(message, this);
            userInputField.text = null;
            userInputField.ActivateInputField();
        }
    }

    private void Update() => chatHistoryInputField.text = MistralAIChat.GetHistory();
}