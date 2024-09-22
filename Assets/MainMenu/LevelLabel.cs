using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LevelLabel : MonoBehaviour
{
    private void Start() {
        SetLevelData();
    }

    public void SetLevelData()
    {
        GetComponent<Text>().text = $"{GameSessionInfoHandler.GetSessionSave().LocalizedLocationName}: {GameSessionInfoHandler.GetSessionSave().CurrentLevel} / {GameSessionInfoHandler.GetSessionSave().MaxLevel}";
    }
}
