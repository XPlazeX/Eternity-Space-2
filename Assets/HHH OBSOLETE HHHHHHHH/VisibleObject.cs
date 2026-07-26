using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisibleObject : MonoBehaviour
{
    private void OnBecameVisible() {
        GetComponent<Animator>().enabled = true;
        print("vision");
    }

    private void OnBecameInvisible() {
        GetComponent<Animator>().enabled = false;
        print("Invision");
    }
}
