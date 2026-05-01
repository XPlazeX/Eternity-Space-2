using UnityEngine;

public class ModuleGetHandler : MonoBehaviour
{
    public delegate void moduleOperation();
    public event moduleOperation ModuleGetted;

    [SerializeField] private LevelEvent _handingEvent;
    [SerializeField] private bool _onlyOneGet = true;

    private bool _getted = false;

    public void Get()
    {
        if (_getted && _onlyOneGet)
            return;

        ModulasSave moduleSave = ModulasSaveHandler.GetSave();

        moduleSave.AddEvent(_handingEvent);

        ModulasSaveHandler.RewriteSave(moduleSave);

        _handingEvent.handingModule.Asquiring();

        _getted = true;
        ModuleGetted?.Invoke();
    }
}
