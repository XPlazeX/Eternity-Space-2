using System.Collections.Generic;
using UnityEngine;

public class SceneLocalizator : MonoBehaviour
{
    static Dictionary<string, TextLoader> TextLoaders = new Dictionary<string, TextLoader>();

    public static string GetLocalizedString(string fileName, int row, int col)
    {
        if (!TextLoaders.ContainsKey(fileName))
            TextLoaders.Add(fileName, new TextLoader("Localization", fileName, -1));
        
        return TextLoaders[fileName].GetCell(row, col);
    }

    public static void Reload()
    {
        TextLoaders.Clear();
        print("--- Clear Scene loacalizator ---");
    }

}
