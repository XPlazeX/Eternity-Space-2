using System.Collections;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    const float noisePertactChance = 0.0003f;

    [SerializeField] private Animator _lightAnimator;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private bool _lightWork = true;
    [SerializeField][Range(0, 0.1f)] private float _cameraAccuracy = 0.1f;

    private IEnumerator _movingIE;
    private bool _nirvus = false;

    private void Start() {
        if (!_lightWork)
            return;

        TurnOnLight();
        //StartCoroutine(_noising);
    }

    private void FixedUpdate() {
        if (!_lightWork)
            return;
        if (Random.value < (noisePertactChance * (1f + ContagionHandler.ContagionLevel)) && !_nirvus)
        _lightAnimator.SetTrigger("Noise");
    }

    public void CodeRed()
    {
        _lightAnimator.SetTrigger("CodeRed");
        _lightWork = false;
    }

    public void TurnOnLight()
    {
        _lightAnimator.SetTrigger("TurnOn");
    }

    public void ExtraNoise()
    {
        _lightAnimator.SetTrigger("Noise");
        _nirvus = false;
    }

    public void Nirvus()
    {
        _lightAnimator.SetTrigger("Nirvus");
        _nirvus = true;
    }

    public void Move(Transform target)
    {
        if (_movingIE != null)
            StopCoroutine(_movingIE);

        _movingIE = Moving(new Vector3(target.position.x, target.position.y, transform.position.z));
        StartCoroutine(_movingIE);
    }

    private IEnumerator Moving(Vector3 toPos)
    {
        while ((transform.position - toPos).magnitude > _cameraAccuracy)
        {
            transform.position = Vector3.Lerp(transform.position, toPos, _moveSpeed * Time.fixedDeltaTime);

            yield return new WaitForFixedUpdate();
        }
    }

}
