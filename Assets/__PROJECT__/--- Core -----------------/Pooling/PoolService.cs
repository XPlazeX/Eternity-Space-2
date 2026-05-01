using System.Collections.Generic;
using UnityEngine;

public class PoolService : MonoBehaviour
{
    public static PoolService Instance { get; private set; }

    private Dictionary<PooledObject, IObjectPool> _poolsByPrefab = new();

    [SerializeField] private Transform _poolRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public T Get<T>(T prefab, Vector3 position, Quaternion rotation, int prewarm = 0) where T : PooledObject
    {
        ObjectPool<T> pool = GetOrCreatePool(prefab, prewarm);
        return pool.Get(position, rotation);
    }

    public ObjectPool<T> GetOrCreatePool<T>(T prefab, int prewarm = 0) where T : PooledObject
    {
        if (_poolsByPrefab.TryGetValue(prefab, out IObjectPool existing))
            return (ObjectPool<T>)existing;

        Transform container = _poolRoot != null
            ? new GameObject($"{prefab.name}_Pool").transform
            : null;

        if (container != null)
            container.SetParent(_poolRoot);

        ObjectPool<T> pool = new ObjectPool<T>(prefab, container, prewarm);
        _poolsByPrefab.Add(prefab, pool);
        return pool;
    }

    public void ClearAll()
    {
        foreach (var pair in _poolsByPrefab)
        {
            if (pair.Value is ObjectPool<PooledObject>)
            {
                // generic cast так не сработает универсально
            }
        }

        _poolsByPrefab.Clear();
    }
}

public static class Pool
{
    public static T Spawn<T>(T prefab, Vector3 position, Quaternion rotation) where T : PooledObject
    {
        return PoolService.Instance.Get(prefab, position, rotation);
    }

    public static T Spawn<T>(T prefab) where T : PooledObject
    {
        return PoolService.Instance.Get(prefab, Vector3.zero, Quaternion.identity);
    }
}