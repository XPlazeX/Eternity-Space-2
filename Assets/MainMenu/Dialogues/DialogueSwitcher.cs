using UnityEngine;
using UnityEngine.UI;

public class DialogueSwitcher : MonoBehaviour
{
    [SerializeField] private RectTransform _movingRect;
    [SerializeField] private float _offset;
    [SerializeField] private float _moveSpeed;
    [Space()]
    [SerializeField] private Image _leftIcon;
    [SerializeField] private Image _rightIcon;
    [SerializeField] private Text _text;

    private Vector2 _targetPosition;
    private float _deltaMagnitude = 0.1f;
    //private bool _leftVoice = true;

    private void Start() {
        SetLeft();
    }

    private void Update() {
        if ((_movingRect.anchoredPosition - _targetPosition).magnitude < _deltaMagnitude)
            return;

        _movingRect.anchoredPosition = Vector2.Lerp(_movingRect.anchoredPosition, _targetPosition, _moveSpeed * Time.unscaledDeltaTime);
    }

    public void SetLeft()
    {
        _targetPosition = new Vector2(_offset, 0f);
    }

    public void SetRight()
    {
        _targetPosition = new Vector2(-_offset, 0f);
    }

    public void SetLeftIcon(Sprite icon)
    {
        if (icon == null)
            return;
        _leftIcon.sprite = icon;
    }

    public void SetRightIcon(Sprite icon)
    {
        if (icon == null)
            return;
        _rightIcon.sprite = icon;
    }

    public void SetText(string text)
    {
        _text.text = text;
    }

    public void CloseDialogue()
    {
        GetComponent<Animator>().SetTrigger("Close");
    }

    public void Death()
    {
        Destroy(gameObject);
    }
}
