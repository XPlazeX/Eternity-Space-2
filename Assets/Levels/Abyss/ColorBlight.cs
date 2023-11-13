using UnityEngine;

public class ColorBlight : MonoBehaviour
{
    [SerializeField] private Gradient _gradient;
    [SerializeField] private float _delay;
    [SerializeField] private float _timeParse;
    [SerializeField] private bool _cycle = false;

    private SpriteRenderer sr;
    private float timer;

    private void Start() {
        sr = GetComponent<SpriteRenderer>();
        timer = _timeParse;
    }

    private void Update() {
        if (Time.frameCount < 2)
            return;
            
        sr.color = _gradient.Evaluate(1f - timer / _timeParse);
            
        timer -= Time.unscaledDeltaTime;
        //print(timer);

        if (_cycle && timer <= 0f)
        {
            timer = _timeParse;
        }
    }
}
