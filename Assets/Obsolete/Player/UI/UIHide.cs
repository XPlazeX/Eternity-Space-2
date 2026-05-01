using UnityEngine;

public class UIHide : MonoBehaviour
{
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void SelfDestroy()
    {
        Destroy(gameObject);
    }
}
