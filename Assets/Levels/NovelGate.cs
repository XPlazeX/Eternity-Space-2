using UnityEngine;
using UnityEngine.UI;

public class NovelGate : MonoBehaviour
{
    [SerializeField] private Text _label;

    private int _callbackID;
    private NovelMission _coreMission;

    public void SetCallbackID(int id, NovelMission core)
    {
        _callbackID = id;
        _coreMission = core;
    }

    public void SetText(string t) => _label.text = t;

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Player"))
            return;

        _coreMission.GateChoiced(_callbackID);
    }
}
