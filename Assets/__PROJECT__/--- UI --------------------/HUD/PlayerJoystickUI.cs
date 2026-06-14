using UnityEngine;

public class PlayerJoystickUI : MonoBehaviour
{
    [SerializeField] private Transform baseTransform;
    [SerializeField] private Transform thumbTransform;
    [SerializeField] private LineRenderer directionLineRenderer;
    [SerializeField] private float maxThumbAmplitude;

    private void Start() {
        
    }

    void Update()
    {
        Vector2 relativePosition = PlayerController.RelativeJoystickPosition;

        thumbTransform.position = baseTransform.position + ((Vector3)relativePosition * maxThumbAmplitude);
        directionLineRenderer.SetPosition(1, thumbTransform.localPosition);
    }
}
