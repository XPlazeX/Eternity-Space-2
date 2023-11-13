using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dev : MonoBehaviour
{
    public static bool IsLogging {get; private set;} = true;

    // private void Awake() {
    //     #if !UNITY_EDITOR
    //         isLogging = false;
    //     #endif
    // }
}
