using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class LevelLabel : MonoBehaviour
{
    private void Start() {
        GetComponent<Text>().text = $"{GameSessionInfoHandler.GetSessionSave().LocalizedLocationName}: {GameSessionInfoHandler.GetSessionSave().CurrentLevel} / {GameSessionInfoHandler.GetSessionSave().MaxLevel}";
    }
}
