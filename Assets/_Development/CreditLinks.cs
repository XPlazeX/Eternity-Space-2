using UnityEngine;
using System.Collections.Generic;
using System;

public class CreditLinks : MonoBehaviour
{
    const string vk_link = "https://vk.com/plazmatixseternity";
    const string github_link = "https://github.com/XPlazeX/Eternity-Space-2";
    const string dev_email = "XPlazeX@yandex.ru";

    public void OpenVK()
    {
        Application.OpenURL(vk_link);
    }

    public void OpenGitHub()
    {
        Application.OpenURL(github_link);
    }

    public void CopyEmailToBuffer()
    {
        GUIUtility.systemCopyBuffer = dev_email;
    }
}
