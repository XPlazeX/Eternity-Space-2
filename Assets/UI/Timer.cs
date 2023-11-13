using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private Text _text;

    private void OnEnable() {
        if (_text == null)
        {
            _text = GetComponent<Text>();
        }

        float t = Time.time;
        int sec = Mathf.FloorToInt(t % 60);
        int min = Mathf.FloorToInt(t / 60);
        _text.text = string.Format("|| {0:00}:{1:00}.{2:00}", min, sec, Mathf.Floor((t - Mathf.Floor(t)) * 100));
        //_text.text = $"|| {min}:{sec}.{Mathf.Round((t - Mathf.Floor(t)) * 10000)}";
    }
}
