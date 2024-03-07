using System.Collections;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private bool _initEmpty = false;
    [SerializeField] private UIAnimationGroupObject[] _uiGroups;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private bool _cameraWork = true;
    [SerializeField][Range(0, 2f)] private float _rectAccuracy = 1f;

    private MainMenuCamera _mainMenuCamera;
    private int _lastID = 0;
    private bool _canMove = true;
    private Coroutine[] _movingUICoroutines;

    private void Start() {
        if (_initEmpty)
        {
            _lastID = -1;
        }

        _movingUICoroutines = new Coroutine[_uiGroups.Length];

        if (_cameraWork)
            _mainMenuCamera = Camera.main.GetComponent<MainMenuCamera>();
    }

    public void OpenGroup(int id)
    {
        if (!_canMove || (_lastID == id))
            return;

        if (_lastID == -1)
        {
            print($"Moving rect : {id}");
            _lastID = id;

            _uiGroups[id].Rect.gameObject.SetActive(true);
            _movingUICoroutines[id] = StartCoroutine(MovingUI(id, true));

            if (_cameraWork)
                _mainMenuCamera.Move(_uiGroups[id].CameraTarget);
            return;
        }

        if (_movingUICoroutines[_lastID] != null)
        {
            StopCoroutine(_movingUICoroutines[_lastID]);
            _movingUICoroutines[_lastID] = null;
        }

        _movingUICoroutines[_lastID] = StartCoroutine(MovingUI(_lastID, false));

        _lastID = id;
        //print($"Moving rect : {id}");

        _uiGroups[id].Rect.gameObject.SetActive(true);

        if (_movingUICoroutines[id] != null)
        {
            StopCoroutine(_movingUICoroutines[id]);
            _movingUICoroutines[id] = null;
        }

        _movingUICoroutines[id] = StartCoroutine(MovingUI(id, true));
        if (_cameraWork)
            _mainMenuCamera.Move(_uiGroups[id].CameraTarget);
    }

    public void StartGame()
    {
        if (_canMove == false)
            return;

        _canMove = false;

        GameObject.FindWithTag("Angar").GetComponent<AngarAnimationController>().StartGame();
        HideCurrent();
        if (_cameraWork)
            _mainMenuCamera.CodeRed();
    }

    public void HideCurrent()
    {
        StopAllCoroutines();
        _movingUICoroutines[_lastID] = StartCoroutine(MovingUI(_lastID, false));
        _lastID = -1;
    }

    public void PowerDown()
    {
        if (_cameraWork)
            _mainMenuCamera.CodeRed();
        HideCurrent();
        _canMove = false;
        Invoke("Quit", 1f);
    }

    private void Quit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    } 

    private IEnumerator MovingUI(int idGroup, bool opening = true)
    {
        RectTransform rect = _uiGroups[idGroup].Rect;
        Vector2 toPosition = _uiGroups[idGroup].TargetPosition;

        if (!opening)
        {
            toPosition = _uiGroups[idGroup].StartPosition;
            rect.anchoredPosition = _uiGroups[idGroup].TargetPosition;
        } else
        {
            rect.anchoredPosition = _uiGroups[idGroup].StartPosition;
        }

        while ((rect.anchoredPosition - toPosition).magnitude > _rectAccuracy)
        {
            rect.anchoredPosition += (toPosition - rect.anchoredPosition) * _moveSpeed * Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

        if (!opening)
            rect.gameObject.SetActive(false);

    }

    [System.Serializable]
    private class UIAnimationGroupObject
    {
        [SerializeField] private Transform _cameraTarget;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private Vector2 _startPosition;
        [SerializeField] private Vector2 _targetPosition;

        public Transform CameraTarget => _cameraTarget;
        public RectTransform Rect => _rect;
        public Vector2 StartPosition => _startPosition;
        public Vector2 TargetPosition => _targetPosition;
    }
}
