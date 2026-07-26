using System.Collections.Generic;
using UnityEngine;

public class SledgeLocker : MonoBehaviour
{
    [SerializeField] private int lockCount = 3;

    private HashSet<int> _unlocks = new HashSet<int>();
    private bool _unlocked = false;

    void Start()
    {
        if (lockCount > 0)
        {
            SledgeDirector.Lock();
            _unlocked = false;
        }

        else
        {
            _unlocked = true;
        }
    }

    public void SetUnlock(int code)
    {
        if (_unlocked) return;

        if (!_unlocks.Contains(code))
        {
            _unlocks.Add(code);
        }

        if (_unlocks.Count >= lockCount)
        {
            Unlock();
        }
    }

    private void Unlock()
    {
        SledgeDirector.Unlock();
        _unlocked = true;
    }
}
