using UnityEngine;
using System.Collections;

public class Chaotix : MonoBehaviour
{
    [SerializeField] private float _chaosPower;
    [SerializeField] private float _reloadRetargeting;
    [SerializeField] private float _rotationSpeed;

    private void OnEnable() {
        StartCoroutine(ChaosDirection());
    }

    private IEnumerator ChaosDirection()
    {
        float timer;
        while (true)
        {
            timer = _reloadRetargeting;

            yield return StartCoroutine(RotatingToRandomDirerction());

            yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_reloadRetargeting));
        }
    }

    private IEnumerator RotatingToRandomDirerction()
    {
        yield return new WaitForSeconds(SceneStatics.MultiplyByChaos(_reloadRetargeting));

        float selectedAngle = SceneStatics.MultiplyByChaos(Random.Range(-_chaosPower, _chaosPower));
        Vector3 selectedDirection = Quaternion.Euler(0, 0,  transform.eulerAngles.z + selectedAngle) * Vector3.up;

        while (Vector3.Angle(transform.up, selectedDirection) > 1f)
        {
            transform.up = Vector3.RotateTowards(transform.up, selectedDirection, _rotationSpeed * Time.deltaTime, 0f);
            yield return new WaitForFixedUpdate();
        }
    }
}
