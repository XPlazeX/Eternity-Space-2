using UnityEngine;

public class ObjectSkin : MonoBehaviour
{
    [SerializeField] private GameObject[] _skins;

    private void Start() {
        for (int i = 0; i < _skins.Length; i++)
        {
            _skins[i].SetActive(i == Skins.SOCurrentSkin());
        }
    }
}
