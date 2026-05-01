using System.Collections;
using UnityEngine;

public class DrillController : BindingObject
{
    [SerializeField] private float _duration;
    [SerializeField] private Vector2 _followOffset;
    [SerializeField] private float _followSpeed;

    private void OnEnable() {
        StartCoroutine(Work());
    }

    public override void LoseBind()
    {
        StopAllCoroutines();
        StopWork();
        //base.LoseBind();
    }

    private void StopWork()
    {
        GetComponent<Animator>().SetTrigger("Stop");
    }

    private IEnumerator Work()
    {
        float timer = _duration;

        while (timer > 0)
        {
            if (_parentTransform != null)
                transform.position = Vector3.Lerp(transform.position, _parentTransform.position + (_parentTransform.rotation * _followOffset), _followSpeed * Time.deltaTime);

            timer -= Time.deltaTime;

            yield return new WaitForFixedUpdate();
        }

        StopWork();
    }

    public void Disable() // for animator
    {
        gameObject.SetActive(false);
    }
}
