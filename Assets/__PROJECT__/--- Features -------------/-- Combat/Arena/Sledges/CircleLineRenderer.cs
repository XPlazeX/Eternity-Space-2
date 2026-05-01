using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleLineRenderer : MonoBehaviour
{
    [SerializeField] private int _dotCount = 20;
    [SerializeField] private float _startRadius = 5f;
    [SerializeField] private bool _autoStart = true;

    private LineRenderer _lr;
    
    public float radius {get; private set;}
    public Vector3 center => transform.position;

    private void Start() {
        if (_autoStart)
            SetRadius(_startRadius);
    }

    public void SetRadius(float r)
    {
        radius = r;
        UpdateRender();
    }

    private void UpdateRender()
    {
        if (_lr == null)
            _lr = GetComponent<LineRenderer>();

        Vector3[] positions = new Vector3[_dotCount];
        float theta = 0f;

        for (int i = 0; i < _dotCount; i++)
        {
            Vector3 position = new Vector3(Mathf.Sin(theta) * radius, Mathf.Cos(theta) * radius, 0f);
            positions[i] = position;

            theta += (2f * Mathf.PI) / _dotCount;
        }

        _lr.positionCount = _dotCount;
        _lr.SetPositions(positions);
    }

    public void SetColor(Color c)
    {
        if (_lr == null)
            _lr = GetComponent<LineRenderer>();
            
        _lr.startColor = c;
        _lr.endColor = c;
    }
}
