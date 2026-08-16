using UnityEngine;

public class EventedRoatator : MonoBehaviour
{
    [SerializeField] private Transform rotatingTransform;

    public void RotateTimeScaled(float r)
    {
        rotatingTransform.Rotate(new Vector3(0, 0, r * ESTime.worldDeltaTime));
    }
}
