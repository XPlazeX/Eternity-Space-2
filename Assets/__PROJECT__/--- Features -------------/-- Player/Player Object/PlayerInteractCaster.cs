using UnityEngine;

public class PlayerInteractCaster : MonoBehaviour
{
    [SerializeField] private ExplosionObject explosionObject;
    [SerializeField] private float interactScale = 1f;

    void Update()
    {
        if (SledgeDirector.IsPlayerControlling && PlayerInput.SelectionDown)
        {
            SpawnExplosion();
        }
    }

    public void SpawnExplosion(Vector3 position)
    {
        if (explosionObject == null)
        {
            Debug.LogError($"Null explosion object on {gameObject.name}");
            return;
        }

        ExplosionObject explosion = Pool.Spawn(explosionObject, position, Quaternion.identity);//_explosionHandler.InstantiateExplosion(position, _explosionCode);
        explosion.SetScale(interactScale);// * (1f / ShipStats.GetValue("Pressure"));
    }

    public void SpawnExplosion()
    {
        SpawnExplosion(transform.position);
    }
}
