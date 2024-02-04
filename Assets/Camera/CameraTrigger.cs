using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    [SerializeField] private float _newCameraScale;
    [SerializeField] private float _changingSpeed;
    [SerializeField] private GameObject _destroyObject;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            Camera.main.GetComponent<CameraController>().SetScale(_newCameraScale, _changingSpeed);

            if (_destroyObject != null)
            {
                Destroy(_destroyObject);
            }

            Destroy(this);
        }
    }
}
