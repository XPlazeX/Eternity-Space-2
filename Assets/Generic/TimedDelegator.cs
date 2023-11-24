using System.Collections;
using UnityEngine;

public class TimedDelegator : MonoBehaviour
{
    public delegate void commonAction();

    private void OnEnable() {
        SceneTransition.SceneTransit += StopAll;
    }

    private void OnDisable() {
        SceneTransition.SceneTransit -= StopAll;
    }

    public void FuseAction(commonAction action, float fuseTime)
    {
        StartCoroutine(TimedAction(action, fuseTime));
    }

    private IEnumerator TimedAction(commonAction action, float fuseTime)
    {
        yield return new WaitForSeconds(fuseTime);

        action.Invoke();
    }

    private void StopAll()
    {
        StopAllCoroutines();
    }
}
