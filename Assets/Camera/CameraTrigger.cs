using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private float _newCameraScale;
    [SerializeField] private float _changingSpeed;


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            Camera.main.GetComponent<CameraController>().SetScale(_newCameraScale, _changingSpeed);

            Destroy(this);
        }
    }
}
