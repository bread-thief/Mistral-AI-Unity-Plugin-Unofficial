using Mistral.AI.Components;
using UnityEditor;
using UnityEngine;

public static class MistralIcons
{
    public static Texture2D Icon;
    public static void LoadIcon()
    {
        if (Icon == null)
            Icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/MistralAI/Textures/MistralIcon.png");
    }
}

public abstract class MistralHelpWindow : EditorWindow
{
    protected static Texture2D icon;

    public static void InitializeIcon()
    {
        if (icon == null)
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/MistralAI/Textures/MistralIcon.png");
    }

    public static T ShowWindow<T>(string windowTitle) where T : MistralHelpWindow
    {
        T window = GetWindow<T>();
        window.Initialize(windowTitle);
        return window;
    }

    protected string title;

    protected void Initialize(string windowTitle)
    {
        title = windowTitle;
        titleContent = new GUIContent(title, icon);
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
        GUIStyle centeredStyle = new GUIStyle(EditorStyles.whiteLargeLabel);
        centeredStyle.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label(HeaderText, centeredStyle);
        GUILayout.Space(10);
        foreach (var text in TextAreas)
        {
            GUILayout.TextArea(text, EditorStyles.wordWrappedLabel);
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
}

public static class MistralApiHelp
{
    [MenuItem("MistralAI/Help/ApiKey")]
    public static void ShowApiKeyHelp()
    {
        MistralIcons.LoadIcon();
        MistralHelpWindow.ShowWindow<MistralApiKeyHelp>("MistralAI/Help/ApiKey");
    }

    [MenuItem("MistralAI/Help/ApiURL")]
    public static void ShowApiUrlHelp()
    {
        MistralIcons.LoadIcon();
        MistralHelpWindow.ShowWindow<MistralApiUrlHelp>("MistralAI/Help/ApiURL");
    }

    [MenuItem("MistralAI/Help/Models")]
    public static void ShowModelsHelp()
    {
        MistralIcons.LoadIcon();
        MistralHelpWindow.ShowWindow<MistralModelsHelp>("MistralAI/Help/Models");
    }
}

public class MistralApiKeyHelp : MistralHelpWindow
{
    protected override string HeaderText => "How to get ApiKey from Mistral AI?";
    protected override string[] TextAreas => new string[]
    {
        "Sign up or sign in: First, you need to have an account with Mistral AI. If you don’t have one, you’ll need to sign up for one on their platform. If you already have an account, sign in.",
        "Go to the API section: Once you’ve signed in, go to the section of the platform where API keys are managed. This is often in your account settings or a dedicated API section.",
        "Generate an API key: Look for the option to generate a new API key. It may be labeled “Generate a new API key.” Then, enter a Key Name (any name for your key). And be sure to select a date (one day later than today).",
        "Copy the API key: Once the key is generated, be sure to copy it."
    };
    protected override string UrlLink => "https://console.mistral.ai/";
    protected override string ButtonText => "URL";
    protected override string CloseButtonText => "Close";
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
}

public static class MistralConfiguration
{
    private static MistralApiSettings settings;
    private static Texture2D bannerTexture;

    [MenuItem("MistralAI/Configuration")]
    public static void ShowWindow()
    {
        LoadSettings();
        var window = GetWindow<MistralConfigWindow>("MistralAI Configuration");
        window.OnGUIInternal();
    }

    public static void LoadSettings()
    {
        string[] guids = AssetDatabase.FindAssets("t:MistralApiSettings");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            settings = AssetDatabase.LoadAssetAtPath<MistralApiSettings>(path);
        }
        else
        {
            settings = ScriptableObject.CreateInstance<MistralApiSettings>();
            string path = "Assets/Plugins/MistralAI/Resources";
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/Plugins/MistralAI", "Resources");
            }
            AssetDatabase.CreateAsset(settings, $"{path}/MistralApiSettings.asset");
            AssetDatabase.SaveAssets();
        }
        bannerTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/MistralAI/Textures/Banner.png");
    }

    public static void OnGUIInternal()
    {
        if (bannerTexture != null)
        {
            float bannerWidth = bannerTexture.width / 2;
            float bannerHeight = bannerTexture.height / 2;
            Rect bannerRect = new Rect(10, 10, bannerWidth, bannerHeight);
            GUI.DrawTexture(bannerRect, bannerTexture, ScaleMode.ScaleToFit);
            GUILayout.Space(bannerHeight + 10);
            if (settings == null)
            {
                Debug.LogWarning("Settings asset not found");
                return;
            }
            GUIStyle centeredStyle = new GUIStyle(EditorStyles.whiteLargeLabel);
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("Mistral API Settings", centeredStyle);
            EditorGUI.BeginChangeCheck();
            settings.SetSettings(
                EditorGUILayout.TextField(new GUIContent("API Key", "API key (More details: MistralAI/Help/ApiKey)"), settings.ApiKey),
                EditorGUILayout.TextField(new GUIContent("API URL", "API URL (More details: MistralAI/Help/ApiURL)"), settings.ApiUrl),
                (ModelType)EditorGUILayout.EnumPopup(new GUIContent("Model", "Model AI (More details: MistralAI/Help/Models)"), settings.Model)
            );
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
            }
        }
        else
        {
            Debug.LogError("Banner not found!");
        }
    }
}

public static class MistralDocumentation
{
    [MenuItem("MistralAI/Documentation")]
    public static void Open()
    {
        Application.OpenURL("https://docs.mistral.ai/");
    }
}