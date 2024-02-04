using UnityEngine;

public class DocModuleChoice : MonoBehaviour
{
    [SerializeField] private ModuleGetHandler[] _moduleGetHandlers;
    [SerializeField] private GameObject _destroyingObject;

    public Doc _bindedDoc;
    private bool _choiced;
    
    private void Start() {
        for (int i = 0; i < _moduleGetHandlers.Length; i++)
        {
            _moduleGetHandlers[i].ModuleGetted += OnModuleGetted;
        }
    }

    public void BindDoc(Doc doc)
    {
        if (doc != null)
            _bindedDoc = doc;
    }

    public void OnModuleGetted()
    {
        if (_choiced)
            return;

        _choiced = true;
        print("choice");
        _bindedDoc.ChoiceMade();
        Destroy(_destroyingObject);
    }
}
