using UnityEngine;

public class EternityClockTrigger : MonoBehaviour
{
    public enum EternityClockTriggerType
    {
        Start = 0,
        End = 1
    }

    [SerializeField] private EternityClockTriggerType _triggerType;
    [SerializeField] private EternityClock _eternityClock;
    [SerializeField] private GameObject _readyObject;
    [SerializeField] private GameObject _disabledObject;

    public bool Interactable {get; private set;}

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!other.CompareTag("Player") || !Interactable)
            return;

        if (_triggerType == EternityClockTriggerType.Start)
        {
            _eternityClock.StartSpan();
        } else
        {
            _eternityClock.EndSpan();
        }
    }

    public void ToggleInteractable(bool tog)
    {
        Interactable = tog;

        _readyObject.SetActive(Interactable);
        _disabledObject.SetActive(!Interactable);
        //GetComponent<SpriteRenderer>().sprite = Interactable ? _readySprite : _disabledSprite;
    }
}
