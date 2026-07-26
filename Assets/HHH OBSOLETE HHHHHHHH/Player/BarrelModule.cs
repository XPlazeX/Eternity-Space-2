using System.Collections;
using UnityEngine;

public class BarrelModule : MonoBehaviour
{
    [SerializeField] private Transform[] _barrels;

    public Transform[] Barrels => _barrels;
}
