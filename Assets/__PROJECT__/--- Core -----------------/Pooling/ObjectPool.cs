using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> : IObjectPool where T : PooledObject
{
    private readonly T _prefab;
    private readonly Transform _container;
    private readonly Stack<T> _available = new Stack<T>();
    private readonly List<T> _all = new List<T>();

    public T Prefab => _prefab;

    public ObjectPool(T prefab, Transform container = null, int prewarmCount = 0)
    {
        _prefab = prefab;
        _container = container;

        for (int i = 0; i < prewarmCount; i++)
        {
            T obj = CreateNew();
            ReturnInternal(obj);
        }
    }

    public T Get(Vector3 position, Quaternion rotation)
    {
        T obj = _available.Count > 0 ? _available.Pop() : CreateNew();

        Transform tr = obj.transform;
        tr.SetPositionAndRotation(position, rotation);

        obj.OnTakenFromPool();
        return obj;
    }

    public void Release(PooledObject obj)
    {
        if (obj is T typedObj)
        {
            ReturnInternal(typedObj);
        }
        else
        {
            Debug.LogError($"Trying to release wrong object type into pool of {_prefab.name}");
        }
    }

    private T CreateNew()
    {
        T obj = Object.Instantiate(_prefab, _container);
        obj.SetOwnerPool(this);
        obj.gameObject.SetActive(false);
        _all.Add(obj);
        return obj;
    }

    private void ReturnInternal(T obj)
    {
        if (obj == null) return;

        obj.gameObject.SetActive(false);
        _available.Push(obj);
    }

    public void Clear()
    {
        for (int i = 0; i < _all.Count; i++)
        {
            if (_all[i] != null)
                Object.Destroy(_all[i].gameObject);
        }

        _all.Clear();
        _available.Clear();
    }
}