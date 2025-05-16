# Mistral AI Unity Plugin (Unofficial)
![Project logo](Mistral-AI-Unity-Plugin-Unofficial/Textures/Banner.png)

### Description
This is an unofficial Unity plugin that allows you to quickly set up integration with the Mistral AI API. All you need is to specify the API Key and URL, without installing additional components or complex settings.

---

## 📝 Table of Contents
- [Overview](#overview)
- [Getting Started](#getting-started)
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

---

## ⛏️ Technologies used <a name="built-using"></a>
- Unity Engine
- C#
- MistralAI
---

## ✍️ Authors <a name="authors"></a>
- [@bread-thief](https://github.com/bread-thief) — Idea and development

---

*Please note: this is an unofficial project and is not affiliated with Mistral AI.*
