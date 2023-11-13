using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetResolution : MonoBehaviour
{
    [SerializeField] private Vector2Int _resolution;

    public void Set()
    {
        Screen.SetResolution(_resolution.x, _resolution.y, FullScreenMode.FullScreenWindow);
        print("try set resolution");
    }
}
