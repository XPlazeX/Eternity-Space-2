using UnityEngine;

public class CanvasGroupPulse : MonoBehaviour
{
    [SerializeField][Range(0, 1f)] private float _downValue = 0f;
    [SerializeField][Range(0, 1f)] private float _upValue = 1f;
    [SerializeField] private float _timeParse;
    [SerializeField] private bool _unscaledTime;
    [SerializeField] private bool _startTransparent = false;

    private CanvasGroup _cg;
    private float timer;
    private bool _growth = false;

    private void Start() {
        _cg = GetComponent<CanvasGroup>();

        if (_startTransparent)
        {
            _cg.alpha = 0f;
            _growth = true;
        }
    }

    private void Update() 
    {
        timer += _unscaledTime? Time.unscaledDeltaTime : Time.deltaTime;

        if (_growth)
        {
            _cg.alpha = Mathf.Lerp(_cg.alpha, _upValue, timer / _timeParse);
        } else
        {
            _cg.alpha = Mathf.Lerp(_cg.alpha, _downValue, timer / _timeParse);
        }

        if (timer >= _timeParse)
        {
            timer = 0f;
            _growth = !_growth;
        }
    }
}
