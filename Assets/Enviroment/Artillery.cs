using System.Collections;
using UnityEngine;

public class Artillery : MonoBehaviour
{
    [SerializeField] private bool _antiPlayer = true;
    [SerializeField] private string _targetTag;
    [Space()]
    [SerializeField] private bool _autoStart = true;
    [SerializeField] private float _delayWork;
    [SerializeField] private Vector2 _minMaxReload;
    [SerializeField] private Vector2Int _minMaxSeries;
    [Header("Attacks")]
    [SerializeField] private int _explosionCode;
    [SerializeField] private float _attackReload;
    [SerializeField] private Vector2 _x_y_playerRandomOffset;
    [Header("Animations")]
    [Space()]
    [SerializeField] private ParticleSystem _psFirer;

    private ExplosionHandler _explosionHandler;

    private void Start() 
    {
        _explosionHandler = SceneStatics.SceneCore.GetComponent<ExplosionHandler>();
        if (_autoStart)
            StartCoroutine(Work());
    }

    public void Fire()
    {
        if (Player.PlayerTransform == null)
            return;
            
        if (_psFirer != null)
            _psFirer.Emit(1);

        Vector3 position = (_antiPlayer ? Player.PlayerTransform.position : GetNearestTransformWithTag(_targetTag).position) + new Vector3(
            Random.Range(-_x_y_playerRandomOffset.x, _x_y_playerRandomOffset.x),
            Random.Range(-_x_y_playerRandomOffset.y, _x_y_playerRandomOffset.y), 0f);

        _explosionHandler.SpawnExplosion(position, _explosionCode);
    }

    public void HandWorkStart()
    {
        StartCoroutine(Work());
    }

    private IEnumerator Work()
    {
        yield return new WaitForSeconds(_delayWork);

        while (true)
        {
            int repeats = Random.Range(_minMaxSeries.x, _minMaxSeries.y + 1);

            for (int i = 0; i < repeats; i++)
            {
                Fire();
                yield return new WaitForSeconds(_attackReload);
            }

            yield return new WaitForSeconds(Random.Range(_minMaxReload.x, _minMaxReload.y));
        }
    }

    private Transform GetNearestTransformWithTag(string targetTag)
    {
        GameObject[] allTargets = GameObject.FindGameObjectsWithTag(targetTag);
        Transform nearestTarget = null;
        float minDistance = 10000f;

        for (int i = 0; i < allTargets.Length; i++)
        {
            float distance = (allTargets[i].transform.position - transform.position).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = allTargets[i].transform;
            }
        }

        return nearestTarget;
    }
}
