using UnityEngine;
using UnityEngine.EventSystems;

public class InputSettings : MonoBehaviour
{
    private void Start() {
        StandaloneInputModule sim = GetComponent<StandaloneInputModule>();

        sim.inputActionsPerSecond = PlayerPrefs.GetFloat("InputUpdateStep", 10f);
    }
}
