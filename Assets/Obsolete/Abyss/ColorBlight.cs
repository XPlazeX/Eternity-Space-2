using UnityEngine;
using UnityEngine.UI;

public class ColorBlight : MonoBehaviour
{
    [SerializeField] private bool _uiImage = false;
    [SerializeField] private bool _uiText = false;
    [SerializeField] private Gradient _gradient;
    [SerializeField] private float _delay;
    [SerializeField] private float _timeParse;
    [SerializeField] private bool _cycle = false;
    [SerializeField] private bool _unscaledTime = true;

    private SpriteRenderer sr;
    private Image _img;
    private Text _text;
    private float timer;

    private void Start() {
        if (_uiImage)
            _img = GetComponent<Image>();
        else if (_uiText)
        {
            _text = GetComponent<Text>();
        }
        else
            sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable() {
        timer = _timeParse;
    }

    private void Update() {
        if (Time.frameCount < 2)
            return;
        
        if (_uiImage)
            _img.color = _gradient.Evaluate(1f - timer / _timeParse);
        else if (_uiText)
            _text.color = _gradient.Evaluate(1f - timer / _timeParse);
        else
            sr.color = _gradient.Evaluate(1f - timer / _timeParse);
            
        timer -= _unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        //print(timer);

        if (_cycle && timer <= 0f)
        {
            timer = _timeParse;
        }
    }
}
