using UnityEngine;

public class CoreList : MonoBehaviour
{
    [SerializeField] private Core[] _cores;

    public Core GetCore(int id) => _cores[id];
}
