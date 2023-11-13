using UnityEngine;

public class DefaultFailure : MonoBehaviour
{
    [SerializeField] private GameObject _asquirePanel;

    public static DefaultFailure instance;

    private void Awake() {
        if (instance == null)
        {
            instance = this;
        } else 
        {
            Destroy(this);
        }
    }

    public static void Failure(Animator anim)
    {
        anim.SetTrigger("Fail");
    }

    public static void Asquire()
    {
        instance._asquirePanel.SetActive(true);
    }
    
}
