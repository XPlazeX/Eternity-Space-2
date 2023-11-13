using UnityEngine;
using System.Collections;

public class AIStartDelay : MonoBehaviour
{
    [SerializeField] private EnemyAIRoot _targetAI;
    [SerializeField] private float _delay;

    private void Start() {
        StartCoroutine(Delay());
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(_delay);

        _targetAI.StartMoving();
        _targetAI.transform.parent = null;

        Destroy(this);
    }
}
