using UnityEngine;

public class OrientateByCamera : MonoBehaviour
{
    void OnEnable()
    {
        CameraController.Moved += OnCameraMoved;
        OnCameraMoved();
    }

    void OnDisable()
    {
        CameraController.Moved -= OnCameraMoved;
    }

    void OnCameraMoved()
    {
        transform.up = SceneStatics.FlatVector(CameraController.Up);
    }
}
