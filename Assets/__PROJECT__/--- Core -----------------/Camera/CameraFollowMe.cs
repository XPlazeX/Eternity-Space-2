using UnityEngine;

public class CameraFollowMe : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float predication;

    void Start()
    {
        CameraController.instance.StartFollowing(transform, speed, offset, predication);
    }
}
