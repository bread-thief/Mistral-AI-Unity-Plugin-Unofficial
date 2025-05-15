using Mistral.AI.Components;
using UnityEditor;
using UnityEngine;

public abstract class MistralHelpWindow : EditorWindow
{
    protected static Texture2D icon;
    protected new string title;

    protected virtual void OnEnable()
    {
        if (icon == null)
            icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/MistralAI/Textures/MistralIcon.png");
    }

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
            this.Close();
        GUILayout.EndVertical();
    }

    public static T ShowWindow<T>(string menuPath, string windowTitle) where T : MistralHelpWindow
    {
        T window = GetWindow<T>();
        window.Initialize(windowTitle);
        return window;
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
        "Sign up or sign in: First, you need to have an account with Mistral AI. If you don’t have one, you’ll need to sign up for one on their platform. If you already have an account, sign in.",
        "Go to the API section: Once you’ve signed in, go to the section of the platform where API keys are managed. This is often in your account settings or a dedicated API section.",
        "Generate an API key: Look for the option to generate a new API key. It may be labeled “Generate a new API key.” Then, enter a Key Name (any name for your key). And be sure to select a date (one day later than today).",
        "Copy the API key: Once the key is generated, be sure to copy it."
    };
    protected override string UrlLink => "https://console.mistral.ai/";
    protected override string ButtonText => "URL";
    protected override string CloseButtonText => "Close";

    [MenuItem("MistralAI/Help/ApiKey")]
    public static void ShowWindow() => ShowWindow<MistralApiKeyHelp>("MistralAI/Help/ApiKey", "MistralAI Help");
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
    public static void ShowWindow()=>ShowWindow<MistralApiUrlHelp>("MistralAI/Help/ApiURL", "MistralAI Help");
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
    public static void ShowWindow()=>ShowWindow<MistralModelsHelp>("MistralAI/Help/Models", "MistralAI Help");
}

/*public class MistralMistralAIChatMenuItem : EditorWindow
{
    [MenuItem("MistralAI/MistralAIChat")]
    public static void ShowWindow()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Plugins/MistralAI/Prefabs/MistralAIChat.prefab");

        if (prefab == null)
        {
            Debug.LogWarning("Please, restore prefab");
            return;
        }

        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (instance != null)
        {
            Undo.RegisterCreatedObjectUndo(instance, "Spawn Prefab");
            instance.transform.position = Vector3.zero;
            Selection.activeObject = instance;
        }
        else
            Debug.LogError("Failed to create prefab instance");
    }
}*/

public class MistralConfigurationWindow : EditorWindow
{
    private MistralApiSettings settings;
    private Texture2D bannerTexture;

    [MenuItem("MistralAI/Configuration")]
    public static void ShowWindow() => GetWindow<MistralConfigurationWindow>("MistralAI Configuration").LoadSettings();

    private void OnEnable()
    {
        bannerTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Plugins/MistralAI/Textures/Banner.png");
        Vector2 size = new Vector2(bannerTexture.width / 2, bannerTexture.height + 10);
        this.minSize = size;
        this.maxSize = size;
    }

    private void LoadSettings()
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
    }

    private void OnGUI()
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
                EditorGUILayout.LabelField("Settings asset not found or created.");
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
            Debug.LogError("Banner not found!");
    }

    public static MistralApiSettings GetSettings()
    {
        string[] guids = AssetDatabase.FindAssets("t:MistralApiSettings");
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<MistralApiSettings>(path);
        }
        return null;
    }
}

public class MistralAIDocumentation : EditorWindow
{
    [MenuItem("MistralAI/Documentation")]
    public static void ShowWindow() => Application.OpenURL("https://docs.mistral.ai/");   
}