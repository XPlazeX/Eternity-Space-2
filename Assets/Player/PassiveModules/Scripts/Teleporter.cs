using UnityEngine;
using DamageSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Teleporter : MonoBehaviour
{
    [SerializeField] private DamageKey _alarmDamageKey;
    [SerializeField] private float _teleportDistance;
    [SerializeField] private bool _useFullGameField;
    [Space()]
    [SerializeField] private Transform _customTeleportTransform;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        AttackObject attackObject = other.GetComponent<AttackObject>();

        if (attackObject == null)
            return;

        if (attackObject.KeyDamage != _alarmDamageKey)
            return;

        Vector3 newPosition = GetNewTeleportPosition();
        
        while (!CameraController.InsideGameField(newPosition))
        {
            newPosition = GetNewTeleportPosition();
        }

        if (_customTeleportTransform != null)
            _customTeleportTransform.position = newPosition;

        else 
            transform.parent.position = newPosition;
    }

    private Vector3 GetNewTeleportPosition()
    {
        if (_useFullGameField)
            return CameraController.GetRandomFieldPosition();
        
        return _customTeleportTransform == null ? GetTeleportPosition(transform.parent.position, _teleportDistance) : GetTeleportPosition(_customTeleportTransform.position, _teleportDistance);
    }

    public static Vector3 GetTeleportPosition(Vector3 origin, float teleportDistance)
    {
        return new Vector3(origin.x + Random.Range(-teleportDistance, teleportDistance), origin.y + Random.Range(-teleportDistance, teleportDistance), 0f);
    }
}
