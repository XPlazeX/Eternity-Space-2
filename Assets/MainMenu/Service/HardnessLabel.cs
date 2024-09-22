using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class HardnessLabel : MonoBehaviour
{
    [SerializeField] private Gradient _gradient;
    [SerializeField] private float _denominator;

    private void Start() {
        print($"<color=cyan>HARDNESS: {GameSessionInfoHandler.HardnessMultiplier}</color>");
        GetComponent<Text>().text = $"+{Mathf.Round((GameSessionInfoHandler.HardnessMultiplier - 1f) * 100f)}%";
        GetComponent<Text>().color = _gradient.Evaluate(Mathf.Clamp01((GameSessionInfoHandler.HardnessMultiplier - 1f) / _denominator));
    }
}
