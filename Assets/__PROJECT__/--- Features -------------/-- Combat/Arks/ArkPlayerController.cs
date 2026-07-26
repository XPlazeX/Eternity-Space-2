using UnityEngine;

public class ArkPlayerController : MonoBehaviour
{
    [SerializeField] private Transform controllingTransform;
    [SerializeField] private float controllingSpeed;
    [SerializeField] private float maxSpeed;
    [Header("Visual")]
    [SerializeField] private ParticleSystem[] psRenderers;
    [SerializeField] private float psRadialPower = 10f;

    private bool _canControl = false;

    public void StartControlling(Vector3 startPosition)
    {
        controllingTransform.position = startPosition;
        _canControl = true;
    }

    public void StopControlling()
    {
        _canControl = false;
    }

    void Update()
    {
        if (!_canControl)
        {
            return;
        }

        Vector3 dragDelta = PlayerInput.PointerDrag;

        if (dragDelta.magnitude > 0f)
        {
            Vector3 targetDelta = Player.Orientation * (dragDelta * controllingSpeed * ESTime.arkDeltaTime);
            if (targetDelta.magnitude > maxSpeed * ESTime.arkDeltaTime)
            {
                targetDelta = targetDelta.normalized * maxSpeed * ESTime.arkDeltaTime;
            }

            controllingTransform.position = controllingTransform.position + targetDelta;
        }

        UpdateRenderers();
    }

    private void UpdateRenderers()
    {
        float targetRadialMultiplier = PlayerInput.MainFirePressed ? -psRadialPower : 0f;

        for (int i = 0; i < psRenderers.Length; i++)
        {
            ParticleSystem ps = psRenderers[i];

            var velocityOverLifetime = ps.velocityOverLifetime;

            velocityOverLifetime.radialMultiplier = targetRadialMultiplier;
        }
    }
}
