using UnityEngine;

public class UIPDA : MonoBehaviour
{
    private enum TransitionType
    {
        Fading = -1,
        MidWait = 0,
        Showing = 1,
        Showed = 2
    }
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private GameObject[] _uiGroups;
    [SerializeField] private float _fadeInTime;
    [SerializeField] private float _midTime;
    [SerializeField] private float _fadeOutTime;

    private TransitionType _currentTransition = TransitionType.Showed;
    private bool _fading = false;
    private bool _faded = false;
    private bool _showed = true;
    private float _timer = 0f;
    private float _startAlpha = 1f;
    private int _targetGroup;

    private void Update()
    {
        if (_currentTransition == TransitionType.Showed)
            return;

        if (_currentTransition == TransitionType.Fading)
        {
            _canvasGroup.alpha = Mathf.Lerp(_startAlpha, 0f, 1f - (_timer / _fadeInTime));

            if (_timer <= 0)
            {
                _canvasGroup.alpha = 0f;
                _currentTransition = TransitionType.MidWait;
                _timer = _midTime;

                for (int i = 0; i < _uiGroups.Length; i++)
                {
                    _uiGroups[i].SetActive(false);
                }
            }
        }
        else if (_currentTransition == TransitionType.MidWait && _timer <= 0f)
        {
            _currentTransition = TransitionType.Showing;
            _uiGroups[_targetGroup].SetActive(true);
            _timer = _fadeOutTime;
        }
        else if (_currentTransition == TransitionType.Showing)
        {
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, 1f - (_timer / _fadeOutTime));

            if (_timer <= 0)
            {
                _canvasGroup.alpha = 1f;
                _currentTransition = TransitionType.Showed;
                _canvasGroup.blocksRaycasts = true;
            }
        }
        
        _timer -= ESTime.unscaledDeltaTime;
    }

    public void SelectGroup(int id)
    {
        if (_targetGroup == id)
            return;

        _canvasGroup.blocksRaycasts = false;
        _targetGroup = id;
        _startAlpha = _canvasGroup.alpha;

        if (_currentTransition == TransitionType.Showing || _currentTransition == TransitionType.Showed)
        {
            _currentTransition = TransitionType.Fading;
            _timer = _fadeInTime;
        }
        //_fading = true;
    }
}
