using UnityEngine;
using UnityEngine.UI;

public class TogglerButton : MonoBehaviour
{
    public delegate void togAction();
    public event togAction Toggled;

    [SerializeField] private Sprite _spriteON;
    [SerializeField] private Sprite _spriteOFF;
    [SerializeField] private bool _defaultState = false;

    public bool ON {get; private set;}

    private void Start() {
        GetComponent<Image>().sprite = _spriteOFF;
        
        if (_defaultState)
            Toggle();
    }

    public void Toggle()
    {
        ON = !ON;

        if (ON)
        {
            GetComponent<Image>().sprite = _spriteON;
        } else 
        {
            GetComponent<Image>().sprite = _spriteOFF;
        }
        Toggled?.Invoke();
    } 
}
