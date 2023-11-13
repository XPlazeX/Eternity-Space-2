using System.Collections;
using UnityEngine;

public class AnimPSCalls : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] PS;

    public void PlayPS(int id)
    {
        PS[id].Play();
        print($"play ps {id}");
    } 
}
