using System.Collections;
using UnityEngine;

public class AbyssDamage : MonoBehaviour
{
    [SerializeField] private float _timeDelay;

    private void Start() {
        StartCoroutine(Pressure());
    }

    private IEnumerator Pressure()
    {
        yield return new WaitForSeconds(_timeDelay);

        for (int i = 0; i < 6; i++)
        {
            PlayerShipData.TakeDamage(i + 1); 
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }

        for (int i = 0; i < 3; i++)
        {
            PlayerShipData.TakeDamage(11); 
            yield return new WaitForSeconds(1.2f);
        }

        PlayerShipData.TakeDamage(1000); 
    }
}
