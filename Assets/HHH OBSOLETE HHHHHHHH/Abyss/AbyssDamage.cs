using System.Collections;
using UnityEngine;

public class AbyssDamage : MonoBehaviour
{
    [SerializeField] private float _timeDelay;

    private bool _work = true;

    private void Start() {
        StartCoroutine(Pressure());
    }

    public void StopPressure() => _work = false;

    private IEnumerator Pressure()
    {
        yield return new WaitForSeconds(_timeDelay);

        while (true)
        {
            if (!_work)
            {
                yield break;
            }
            PlayerShipData.ConsumeHP(Random.Range(0, 5)); 
            yield return new WaitForSeconds(Random.Range(0.1f, 2f));
        }
    }
}
