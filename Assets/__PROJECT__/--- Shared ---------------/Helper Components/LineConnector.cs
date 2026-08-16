using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class LineConnector : MonoBehaviour
{
    [SerializeField] private Transform[] initialPoints;
    [SerializeField] private bool isLoop;
    [SerializeField] private bool isUpdating = true;

    private LineRenderer _lr;
    private List<Transform> _pointList = new List<Transform>();

    private void Start() {
        _pointList.AddRange(initialPoints);
        Rebuild();
    }

    void LateUpdate()
    {
        if (!isUpdating) return;

        Rebuild();
    }

    public void AddPoint(Transform pointTransform)
    {
        _pointList.Add(pointTransform);
        Rebuild();
    }

    private void Rebuild()
    {
        if (_lr == null)
        {
            _lr = GetComponent<LineRenderer>();
            _lr.loop = isLoop;
        }

        Vector3[] positions = new Vector3[_pointList.Count];

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = _pointList[i].position;
        }

        _lr.positionCount = positions.Length;
        _lr.SetPositions(positions);
    }

    public void SetColor(Color c)
    {
        if (_lr == null)
        {
            _lr = GetComponent<LineRenderer>();
            _lr.loop = isLoop;
        }
            
        _lr.startColor = c;
        _lr.endColor = c;
    }
}
