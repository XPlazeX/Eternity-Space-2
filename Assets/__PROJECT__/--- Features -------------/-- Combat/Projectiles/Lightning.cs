using System.Collections;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    [SerializeField] private float _redirectTime;
    [SerializeField] private float _maxAngle;
    [SerializeField] private bool _alwaysNormalized;

    // Start is called before the first frame update

    private void OnEnable() {
        StartCoroutine(Redirecting());
    }
    private IEnumerator Redirecting()
    {
        float timer = _redirectTime;

        while (true)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = _redirectTime;
                transform.rotation = Quaternion.Euler(0, 0, GetAngleRotation());
            }

            yield return new WaitForFixedUpdate();    
        }
    }

    private float GetAngleRotation()
    {
        if (_alwaysNormalized)
            return Random.Range(-_maxAngle, _maxAngle);
        else
            return transform.eulerAngles.z + Random.Range(-_maxAngle, _maxAngle);
    }
}
