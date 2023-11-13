using UnityEngine;
using UnityEngine.EventSystems;

public class LinkText : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private string _link;

    public void OnPointerClick(PointerEventData eventData)
    {
        Application.OpenURL(_link);
    }
}
