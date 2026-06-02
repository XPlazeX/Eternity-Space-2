using UnityEngine;

public class OrientateByPlayer : MonoBehaviour
{
    [SerializeField] private bool orienatateLocalPosition = true;

    private Vector3 _originalLocalPosition;

    void Start()
    {
        _originalLocalPosition = transform.localPosition;
    }

    void Update()
    {
        if (orienatateLocalPosition)
        {
            transform.localPosition = Player.Orientation * _originalLocalPosition;
        }

        transform.rotation = Player.Orientation;
        
    }
}
