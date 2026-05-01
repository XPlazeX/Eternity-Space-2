using UnityEngine;

public class MoveStation : MonoBehaviour
{
    private const float min_y_station_border = -23f;

    [SerializeField] private float _defaultSpeed;
    [SerializeField] private bool _unscaledTime;

    private bool _moving = true;

    private void FixedUpdate() 
    {
        if (transform.position.y < min_y_station_border)
        {
            _moving = false;
            gameObject.SetActive(false);
        }

        if (!_moving)
            return;

        if (_unscaledTime)
        {
            transform.position += Vector3.down * _defaultSpeed * Time.unscaledDeltaTime;
        }
        else
        {
            transform.position += Vector3.down * _defaultSpeed * Time.deltaTime;
        }
    }
}
