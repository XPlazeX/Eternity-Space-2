using UnityEngine;

[System.Serializable]
public struct Blueprint
{
    public bool inDevelop;
    public int handingID;
    public UnlockRequire[] requirementsID;
    public int cosPrice;
    public int posPrice;
    public Sprite sprite;
    public int localizationID;
}
