using UnityEngine;

public class DeathCaller : MonoBehaviour
{
    [SerializeField] private int _deathExplosionType;

    private void Start() {
        SceneStatics.SceneCore.GetComponent<ExplosionHandler>().PreloadExplosion(_deathExplosionType);
    }

    public void DeathExplosion() {
        ExplosionHandler handler = SceneStatics.SceneCore.GetComponent<ExplosionHandler>();
        handler.SpawnExplosion(transform.position, _deathExplosionType);
    }

}
