using UnityEngine;
using UnityEngine.UI;

public class AdditiveUI : MonoBehaviour
{
    [SerializeField] private RectTransform _placingObject;
    [SerializeField] private Vector2 _anchoredPosition;

    private void Start() {
        SceneStatics.UICore.GetComponent<UIPlacer>().PlacePassiveUI(_placingObject, _anchoredPosition);
    }

}
