using UnityEngine;

public class ToggleByParsing : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToToggle;
    [SerializeField] private bool stateOnParsing;

    private SledgeTransitor _sledgeTransitor;

    void Start()
    {
        _sledgeTransitor = FindAnyObjectByType<SledgeTransitor>();
    }

    void Update()
    {
        if (_sledgeTransitor == null)
        {
            _sledgeTransitor = FindAnyObjectByType<SledgeTransitor>();
            if (_sledgeTransitor == null)
                return;
        }
            

        for (int i = 0; i < objectsToToggle.Length; i++)
        {
            objectsToToggle[i].SetActive(_sledgeTransitor.IsParsing == stateOnParsing);
        }
    }
}
