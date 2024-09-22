using UnityEngine;

public class MinibossAI : MonoBehaviour
{
    const float startDelay = 0.2f; // необходим, чтобы AI успели обработать свои методы Start

    [SerializeField] private bool _autoStart = true;
    [SerializeField] private EnemyAIRoot[] _AIArray;
    [SerializeField] private AIPreset[] _presets;

    private float _timer = 0f;
    private int _order = 0;
    private EnemyAIRoot _activeAI = null;
    private bool _active = false;

    private void Start() {
        if (_autoStart)
            _active = true;
        _timer = startDelay;
    }

    public void StartMoving() => _active = true;

    private void Update()
    {
        if (!_active)
            return;

        _timer -= Time.deltaTime;

        if (_timer > 0f)
            return;

        _activeAI?.StopMoving();
        _activeAI = _AIArray[_presets[_order].Code];
        _activeAI.StartMoving();

        _timer = _presets[_order].Duration;

        _order ++;

        if (_order >= _presets.Length)
            _order = 0;

    }

    [System.Serializable]
    private class AIPreset
    {
        [SerializeField] private int _code;
        [SerializeField] private float _duration;

        public int Code => _code;
        public float Duration => _duration;
    }
}
