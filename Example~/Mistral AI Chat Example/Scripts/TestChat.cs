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
        userInputField.onEndEdit.AddListener(OnButtonClick);
        sendButton.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (!string.IsNullOrEmpty(userInputField.text))
        {
            MistralAIChat.SendRequest(userInputField.text, this);
            userInputField.text = null;
        }
    }

    private void Update() => chatHistoryInputField.text = MistralAIChat.GetHistory();
}