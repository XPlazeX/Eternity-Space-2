using UnityEngine;

public abstract class PooledObject : MonoBehaviour
{
    private IObjectPool _ownerPool;
    public bool IsInUse { get; private set; }

    public void SetOwnerPool(IObjectPool pool)
    {
        _ownerPool = pool;
    }

    public void OnTakenFromPool()
    {
        IsInUse = true;
        gameObject.SetActive(true);
        ResetState();
    }

    public void Release()
    {
        if (_ownerPool == null)
        {
            Destroy(gameObject);
            return;
        }

        IsInUse = false;
        _ownerPool.Release(this);
    }

    /// <summary>
    /// Сброс временного состояния перед повторным использованием.
    /// </summary>
    protected virtual void ResetState() { }
}

public interface IObjectPool
{
    void Release(PooledObject obj);
    void Clear();
}