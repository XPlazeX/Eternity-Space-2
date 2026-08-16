using UnityEngine;

public class ParticleSystemParallaxTiler : MonoBehaviour
{
    [SerializeField] private Transform observingTransform;
    [SerializeField] private Transform bottomLeft;
    [SerializeField] private Transform bottomRight;
    [SerializeField] private Transform topLeft;
    [SerializeField] private Transform topRight;

    [Header("Manual tile size")]
    [SerializeField] private Vector2 tileSize = new Vector2(80f, 40f);

    private int _xStep = 0;
    private int _yStep = 0;

    private bool _verticalReverse = false;
    private bool _horizontalReverse = false;

    private Transform _cameraTransform; // center

    private void Start() {
        if (observingTransform == null)
        {
            observingTransform = Camera.main.transform;
        }
    }

    private void LateUpdate() 
    {
        Vector3 delta = observingTransform.position - transform.position;

        if (delta.x > (tileSize.x * _xStep + tileSize.x / 2f))
        {
            StepRight();
        }
        else if (delta.x < (tileSize.x * _xStep - tileSize.x / 2f))
        {
            StepLeft();
        }

        if (delta.y > (tileSize.y * _yStep + tileSize.y / 2f))
        {
            StepUp();
        }
        else if (delta.y < (tileSize.y * _yStep - tileSize.y / 2f))
        {
            StepDown();
        }
    }

    private void StepUp()
    {
        _yStep ++;

        if (_verticalReverse)
        {
            topLeft.transform.position += Vector3.up * tileSize.y * 2f;
            topRight.transform.position += Vector3.up * tileSize.y * 2f;
        }
        else
        {
            bottomLeft.transform.position += Vector3.up * tileSize.y * 2f;
            bottomRight.transform.position += Vector3.up * tileSize.y * 2f;
        }

        _verticalReverse = !_verticalReverse;
    }

    private void StepDown()
    {
        _yStep --;

        if (!_verticalReverse)
        {
            topLeft.transform.position += Vector3.down * tileSize.y * 2f;
            topRight.transform.position += Vector3.down * tileSize.y * 2f;
        }
        else
        {
            bottomLeft.transform.position += Vector3.down * tileSize.y * 2f;
            bottomRight.transform.position += Vector3.down * tileSize.y * 2f;
        }

        _verticalReverse = !_verticalReverse;
    }

    private void StepLeft()
    {
        _xStep --;

        if (!_horizontalReverse)
        {
            bottomRight.transform.position += Vector3.left * tileSize.x * 2f;
            topRight.transform.position += Vector3.left * tileSize.x * 2f;
        }
        else
        {
            bottomLeft.transform.position += Vector3.left * tileSize.x * 2f;
            topLeft.transform.position += Vector3.left * tileSize.x * 2f;
        }

        _horizontalReverse = !_horizontalReverse;
    }

    private void StepRight()
    {
        _xStep ++;

        if (_horizontalReverse)
        {
            bottomRight.transform.position += Vector3.right * tileSize.x * 2f;
            topRight.transform.position += Vector3.right * tileSize.x * 2f;
        }
        else
        {
            bottomLeft.transform.position += Vector3.right * tileSize.x * 2f;
            topLeft.transform.position += Vector3.right * tileSize.x * 2f;
        }

        _horizontalReverse = !_horizontalReverse;
    }
}