using UnityEngine;
using UnityEngine.UI;

public class VersionBooster : MonoBehaviour
{
    [SerializeField][Range(0, 1)] private int _controlVersion = 0;
    [Space()]
    [SerializeField] private Image _targetImage;
    [SerializeField] private Sprite[] _panelSprites;
    [SerializeField] private GameObject[] _togglingGameObjects;
    [SerializeField] private RuStoreSkinToggler _ruStoreSkinToggler;

    private void Awake() 
    {
        if (_controlVersion == 0)
        {
            Dev.RuStoreVersion = true;
            print("RUSTORE VERSION");
            _ruStoreSkinToggler.Initialize();
        } else
        {
            Dev.RuStoreVersion = false;
            print("GOOGLEPLAY VERSION");
        }
    }

    private void Start()
    {
        SetView(_controlVersion);
    }

    private void SetView(int version)
    {
        _targetImage.sprite = _panelSprites[version];

        for (int i = 0; i < _togglingGameObjects.Length; i++)
        {
            if (i != version)
                _togglingGameObjects[i].SetActive(false);
            
            else
                _togglingGameObjects[i].SetActive(true);
        }
    }
}
