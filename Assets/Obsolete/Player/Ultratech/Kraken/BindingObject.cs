using UnityEngine;

public class BindingObject : MonoBehaviour
{
    [SerializeField] private bool _bindParentTransform;

    protected Transform _parentTransform;

    public void Bind(Transform targetTransform) {
        _parentTransform = targetTransform;
    }

    private void FixedUpdate() {
        if (_bindParentTransform && _parentTransform == null)
        {
            LoseBind();
        }
    }

    public virtual void LoseBind()
    {
        gameObject.SetActive(false);
    }
}
