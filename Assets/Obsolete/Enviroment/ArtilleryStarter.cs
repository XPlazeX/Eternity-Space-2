using UnityEngine;

public class ArtilleryStarter : MonoBehaviour
{
    void Start()
    {
        GameObject.FindWithTag("BG-Station").GetComponent<Artillery>().HandWorkStart();
    }
}
