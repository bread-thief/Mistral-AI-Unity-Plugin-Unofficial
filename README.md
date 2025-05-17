# Mistral AI Unity Plugin (Unofficial)
![Project logo](https://ltdfoto.ru/images/2025/05/16/Banner.png)

### Description
This is an unofficial Unity plugin that allows you to quickly set up integration with the Mistral AI API. All you need is to specify the API Key and URL, without installing additional components or complex settings.

---

## 📝 Table of Contents
- [Overview](#overview)
- [Getting Started](#getting-started)
- [Download and Install](#download-and-install)
- [Usage](#usage)
- [Built Using](#built-using)
- [Authors](#authors)

---

## 🧐 Overview <a name="overview"></a>
This project is designed to simplify Unity's interaction with the Mistral AI API. After setting up the API Key and URL, you can access Mistral from your scripts.

---

## 🏁 Getting Started <a name="getting-started"></a>

### Prerequisites
- Unity (supported versions)
- API Key and URL from Mistral AI (get it on the website)

### Download and Install <a name="download-and-install"></a>
1. **Install NuGet**
   - Copy the link or do it in the [NuGetForUnity repository](https://github.com/GlitchEnzo/NuGetForUnity).
   *https://github.com/GlitchEnzo/NuGetForUnity.git*
2. **Extract the Files**
   - Extract the contents of the ZIP file to a location of your choice.

3. **Open Unity Project**
   - Open your Unity project where you want to integrate the Mistral AI plugin.

4. **Import the Plugin**
   - In Unity, go to `Assets > Import Package > Custom Package`.
   - Navigate to the location where you extracted the plugin files and select the `.unitypackage` file.
   - Click on "Open" to import the plugin into your Unity project.

5. **Set Up API Key and URL**
   - In Unity, open the menu: `MistralAI/Help`.
   - In the window that appears, find out how to get:
     - **API Key** — your key
     - **API URL** — the address of the server API

6. **Verify Installation**
   - Ensure that the plugin is correctly imported and there are no errors in the Unity console.

7. **Start Using the Plugin**
   - You are now ready to use the Mistral AI Unity Plugin in your project.

### How to set up
1. In Unity, open the menu:
- `MistralAI/Help`
2. In the window that appears, find out how to get:
- **API Key** — your key
- **API URL** — the address of the server API

---

## 🚀 Usage <a name="usage"></a>
After setting up the API Key and URL, you can access the API from your scripts. To do this, use the provided methods.

```csharp
using UnityEngine;
using Mistral.AI;

public class UsagePlugin : MonoBehaviour
{
   private void Start()
   {
      MistralAIChat.SendRequest("Hello! 2+2=?", this, true);
      string request = MistralAIChat.GetCurrentResponse();
   }
}
```

![Usage Example](https://ltdfoto.ru/images/2025/05/16/UsageExample.png)

---

## ⛏️ Technologies used <a name="built-using"></a>
- Unity Engine
- C#
- NuGet
- MistralAI

---

## ✍️ Authors <a name="authors"></a>
- [@bread-thief](https://github.com/bread-thief) — Idea and development

---

*Please note: this is an unofficial project and is not affiliated with Mistral AI.*
```

This version includes a script for usage and a field for text/link to an image under the usage section. Make sure to replace the GitHub repository link and the image URL with the actual links.
