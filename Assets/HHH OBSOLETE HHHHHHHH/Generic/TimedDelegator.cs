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

    public void UnscaledFuseAction(commonAction action, float fuseTime)
    {
        StartCoroutine(UnscaledTimedAction(action, fuseTime));
    }

    private IEnumerator TimedAction(commonAction action, float fuseTime)
    {
        yield return new WaitForSeconds(fuseTime);

        action.Invoke();
    }

    private IEnumerator UnscaledTimedAction(commonAction action, float fuseTime)
    {
        yield return new WaitForSecondsRealtime(fuseTime);

        action.Invoke();
    }

    private void StopAll()
    {
        StopAllCoroutines();
    }
}
