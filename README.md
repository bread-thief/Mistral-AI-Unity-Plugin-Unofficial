# Mistral AI Unity Plugin (Unofficial) 🌟

![Project logo](https://i.ibb.co/jPdH5KDD/Banner.png)

### 📰 Description
This is an unofficial plugin for Unity that allows you to quickly set up integration with the Mistral AI API. All you need is to specify the API Key and URL, without installing additional components or complex settings.

---

## 📝 Table of Contents
- [Overview](#overview)
- [Getting Started](#getting-started)
- [Download-and-install](#download-and-install)
- [Usage](#usage)
- [Built-using](#built-using)
- [Authors](#authors)

---

## 🧐 Overview <a name="overview"></a>
This project is designed to simplify Unity's interaction with the Mistral AI API. Once you have set up the API Key and URL, you can access Mistral from your scripts.

---

## 🏁 Getting Started <a name="getting-started"></a>

### Prerequisites
- Unity (supported versions)
- Mistral AI API Key and URL (get it from the website)

### Download and Install <a name="download-and-install"></a>
1. **Install NuGet**
- Copy the link or do it in the [**NuGetForUnity**](https://github.com/GlitchEnzo/NuGetForUnity) repository.
~~~
https://github.com/GlitchEnzo/NuGetForUnity.git
~~~
- In Unity, go to `Window > Package Manager`:

![Alt ​​text](https://i.ibb.co/F4Mdtz2j/1.png)

- Paste the copied link and click the "Install" button:

![Alt ​​text](https://i.ibb.co/SwyR7L0z/2.png)

2. **Installing Newtonsoft.Json**
- After installing NuGet, the NuGet tab should appear:

![Alt ​​text](https://i.ibb.co/TDh4Zppc/3.png)

- In Unity, go to `NuGet > Manage NuGet Packages`:

![Alt text](https://i.ibb.co/Xf0RRJHn/4.png)

- In the window that appears, find [**Newtonsoft.Json**](https://github.com/JamesNK/Newtonsoft.Json) in the list and click the "Install" button:

![Alt ​​text](https://i.ibb.co/yndPtkYG/5.png)

3. **Installing MistralAIUnityPlugin**
- Copy the link.
~~~
https://github.com/bread-thief/Mistral-AI-Unity-Plugin-Unofficial.git
~~~
- In Unity, go to `Window > Package Manager`:

![Alt ​​text](https://i.ibb.co/F4Mdtz2j/1.png)

- Paste the copied link and click the "Install" button:

![Alt ​​text](https://i.ibb.co/LdRFDzd0/6.png)

- After installing the plugin, a tab with the same name should appear:

![Alt ​​text](https://i.ibb.co/M0Mcrty/7.png)

---

## 🚀 Using <a name="usage"></a>
Once you have set up the API Key and URL, you can access the AI ​​from your scripts. To do this, use the provided methods.

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
---

## ⛏️ Technologies used <a name="built-using"></a>
- Unity Engine
- C#
- NuGet (Newtonsoft.Json)
- MistralAI

---

## ✍️ Authors <a name="authors"></a>
- [@bread-thief](https://github.com/bread-thief) — Idea and development

---

*Please note: this is an unofficial project and is not affiliated with Mistral AI.*