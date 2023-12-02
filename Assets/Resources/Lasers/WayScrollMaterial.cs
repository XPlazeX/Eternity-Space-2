using UnityEngine;

public class WayScrollMaterial : MonoBehaviour
{
    [SerializeField] private float _scrollXSpeed;
    [SerializeField] private float _scrollYSpeed;

    private LineRenderer _lr;
    // Start is called before the first frame update
    void Start()
    {
        _lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float offsetX = Time.time * _scrollXSpeed;
        float offsetY = Time.time * _scrollYSpeed;

        _lr.material.mainTextureOffset = new Vector2(offsetX, offsetY);
    }
}
