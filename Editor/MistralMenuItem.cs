#if UNITY_EDITOR
using Mistral.AI.Components;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

public abstract class MistralHelpWindow : EditorWindow
{
    protected static Texture2D _icon;
    protected new string _title;

    protected virtual void OnEnable()
    {
        if (_icon == null)
            _icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/MistralAI/Textures/MistralIcon.png");
    }

    protected void Initialize(string windowTitle)
    {
        _title = windowTitle;
        titleContent = new GUIContent(_title, _icon);
        Show();
    }

    protected abstract string HeaderText { get; }
    protected abstract string[] TextAreas { get; }
    protected abstract string UrlLink { get; }
    protected abstract string ButtonText { get; }
    protected abstract string CloseButtonText { get; }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        GUIStyle centeredStyle = new GUIStyle(EditorStyles.whiteLargeLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };
        
        GUILayout.Label(HeaderText, centeredStyle);
        GUILayout.Space(10);
        
        foreach (var text in TextAreas)
        {
            EditorGUILayout.TextArea(text, EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);
        }
        
        GUILayout.Label("ENJOY!", centeredStyle);
        GUILayout.Space(10);
        
        if (GUILayout.Button(ButtonText))
            Application.OpenURL(UrlLink);
            
        if (GUILayout.Button(CloseButtonText))
            Close();
            
        GUILayout.EndVertical();
    }

    public static T ShowWindow<T>(string windowTitle) where T : MistralHelpWindow
    {
        T window = GetWindow<T>();
        window.Initialize(windowTitle);
        return window;
    }
}

public class MistralApiKeyHelp : MistralHelpWindow
{
    protected override string HeaderText => "How to get ApiKey from Mistral AI?";
    protected override string[] TextAreas => new string[]
    {
        "Sign up or sign in: First, you need to have an account with Mistral AI. If you don't have one, you'll need to sign up for one on their platform. If you already have an account, sign in.",
        "Go to the API section: Once you've signed in, go to the section of the platform where API keys are managed. This is often in your account settings or a dedicated API section.",
        "Generate an API key: Look for the option to generate a new API key. It may be labeled 'Generate a new API key.' Then, enter a Key Name (any name for your key). And be sure to select a date (one day later than today).",
        "Copy the API key: Once the key is generated, be sure to copy it."
    };
    protected override string UrlLink => "https://console.mistral.ai/";
    protected override string ButtonText => "URL";
    protected override string CloseButtonText => "Close";

    [MenuItem("MistralAI/Help/ApiKey")]
    public static void ShowHelpWindow()
    {
        ShowWindow<MistralApiKeyHelp>("MistralAI Help");
    }
}

public class MistralApiUrlHelp : MistralHelpWindow
{
    protected override string HeaderText => "How to get ApiURL from Mistral AI?";
    protected override string[] TextAreas => new string[]
    {
        "ApiURL is the base address of Mistral API, which is usually specified in the documentation or in the API keys section. Usually it is something like the one currently specified in the MistralAI object, but it is recommended to check the exact URL in the platform documentation or in the API section."
    };
    protected override string UrlLink => "https://docs.mistral.ai/api/";
    protected override string ButtonText => "URL";
    protected override string CloseButtonText => "Close";

    [MenuItem("MistralAI/Help/ApiURL")]
    public static void ShowHelpWindow()
    {
        ShowWindow<MistralApiUrlHelp>("MistralAI Help");
    }
}

public class MistralModelsHelp : MistralHelpWindow
{
    protected override string HeaderText => "Which model to choose?";
    protected override string[] TextAreas => new string[]
    {
        "More information about the models can be found in the official documentation of MistralAI in the Free Models section."
    };
    protected override string UrlLink => "https://docs.mistral.ai/getting-started/models/models_overview/";
    protected override string ButtonText => "URL";
    protected override string CloseButtonText => "Close";

    [MenuItem("MistralAI/Help/Models")]
    public static void ShowHelpWindow()
    {
        ShowWindow<MistralModelsHelp>("MistralAI Help");
    }
}

public class MistralConfigurationWindow : EditorWindow
{
    private MistralApiSettings _settings;
    private Texture2D _bannerTexture;

    [MenuItem("MistralAI/Configuration")]
    public static void ShowConfigurationWindow()
    {
        GetWindow<MistralConfigurationWindow>("MistralAI Configuration").LoadSettings();
    }

    private void OnEnable()
    {
        _bannerTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.unity.mistral-ai/Textures/Banner.png");
        if (_bannerTexture != null)
        {
            minSize = new Vector2(_bannerTexture.width / 2, _bannerTexture.height + 10);
            maxSize = minSize;
        }
    }

    private void LoadSettings()
    {
        string[] guids = AssetDatabase.FindAssets("t:MistralApiSettings");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _settings = AssetDatabase.LoadAssetAtPath<MistralApiSettings>(path);
        }
        else
            CreateNewSettings();
    }

    private void CreateNewSettings()
    {
        _settings = ScriptableObject.CreateInstance<MistralApiSettings>();
        string path = "Assets/MistralAI/Resources";
        if (!AssetDatabase.IsValidFolder(path))
            AssetDatabase.CreateFolder("Assets/MistralAI", "Resources");
        AssetDatabase.CreateAsset(_settings, $"{path}/MistralApiSettings.asset");
        AssetDatabase.SaveAssets();
    }

    private void OnGUI()
    {
        if (_bannerTexture == null || _settings == null)
        {
            EditorGUILayout.HelpBox("Required assets not found!", MessageType.Error);
            return;
        }

        Rect bannerRect = new Rect(10, 10, _bannerTexture.width / 2, _bannerTexture.height / 2);
        GUI.DrawTexture(bannerRect, _bannerTexture, ScaleMode.ScaleToFit);
        GUILayout.Space(bannerRect.height + 20);

        GUIStyle centeredStyle = new GUIStyle(EditorStyles.whiteLargeLabel)
        {
            alignment = TextAnchor.MiddleCenter
        };
        
        GUILayout.Label("Mistral API Settings", centeredStyle);

        EditorGUI.BeginChangeCheck();
        
        string newApiKey = EditorGUILayout.TextField(
            new GUIContent("API Key", "API key (More details: MistralAI/Help/ApiKey)"), 
            _settings.ApiKey);
            
        string newApiUrl = EditorGUILayout.TextField(
            new GUIContent("API URL", "API URL (More details: MistralAI/Help/ApiURL)"), 
            _settings.ApiUrl);
            
        ModelType newModel = (ModelType)EditorGUILayout.EnumPopup(
            new GUIContent("Model", "Model AI (More details: MistralAI/Help/Models)"), 
            _settings.Model);

        if (EditorGUI.EndChangeCheck())
        {
            _settings.SetSettings(newApiKey, newApiUrl, newModel);
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();
        }
    }

    public static MistralApiSettings GetSettings()
    {
        string[] guids = AssetDatabase.FindAssets("t:MistralApiSettings");
        return guids.Length > 0 ? 
            AssetDatabase.LoadAssetAtPath<MistralApiSettings>(AssetDatabase.GUIDToAssetPath(guids[0])) : 
            null;
    }
}

public class MistralAIDocumentation : EditorWindow
{
    [MenuItem("MistralAI/Documentation")]
    public static void OpenDocumentation()
    {
        Application.OpenURL("https://docs.mistral.ai/");
    }
}

public class Autor : EditorWindow
{
    [MenuItem("MistralAI/Author")]
    public static void OpenAuthorPage()
    {
        Application.OpenURL("https://github.com/bread-thief/");
    }
}
#endif