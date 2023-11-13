using System.Collections.Generic;
using UnityEngine;

public class PullForObjects
{
    private PullableObject _sample;
    public PullableObject Sample => _sample;

    private List<PullableObject> Pool = new List<PullableObject>();

    public PullForObjects(PullableObject sampleObject)
    {
        _sample = MonoBehaviour.Instantiate(sampleObject, Vector3.zero, Quaternion.identity);
        //_sample.Binded = true;
        _sample.Initialize();
        _sample.gameObject.SetActive(false);

        SceneTransition.SceneClosing += ClearPool;
        //Debug.Log($"Sample of {_sample.gameObject.name} initialized, position : {_sample.transform.position}");
    }

    public GameObject GetGameObject(bool autoActive = true)
    {
        GameObject selectedObj = null;
        for (int i = 0; i < Pool.Count; i++)
        {
            if (Pool[i].Sleeping == true)
            {
                if (Pool[i] == null)
                    return null;
                selectedObj = Pool[i].gameObject;
                selectedObj.transform.rotation = Quaternion.identity;
            }
        }

        if (selectedObj == null)
            {
                PullableObject newPoolObject = MonoBehaviour.Instantiate(_sample, Vector3.zero, Quaternion.identity);
                Pool.Add(newPoolObject);
                //newPoolObject.Binded = true;
                newPoolObject.Initialize();
                selectedObj = newPoolObject.gameObject;
            }
        
        if (autoActive)
            selectedObj.SetActive(true);
        return selectedObj;
    }

    public void SampleChanged()
    {
        for (int i = 0; i < Pool.Count; i++)
        {
            Pool[i].Binded = false;
        }
        Pool.Clear();
    }

    public void ClearPool()
    {
        for (int i = 0; i < Pool.Count; i++)
        {
            MonoBehaviour.Destroy(Pool[i].gameObject);
        }
        Pool.Clear();
    }
}

public class PullableObject : MonoBehaviour
{
    protected bool _initialized = false;
    private bool _binded = false;
    public bool Binded
    {
        get 
        {
            return _binded;
        }
        set
        {
            _binded = value;
            if (!value && Sleeping)
                Destroy(gameObject);
        }
    }
    public bool Sleeping {get; private set;}

    protected virtual void SetDefaultStats()
    {
        //print("Set default stats for empty pullable object");
    }

    public virtual void Initialize()
    {
        _initialized = true;
        Binded = true;
    }

    private void OnEnable()
    {
        SetDefaultStats();
        Sleeping = false;
    }

    protected virtual void OnDisable()
    {
        Sleeping = true;
        if (!Binded) Destroy(gameObject);
    }
}