using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public const float sound_overborders = 4f;

    public delegate void scaleOperation(float val);
    public event scaleOperation ChangingScale;
    public event scaleOperation BordersChange;

    [SerializeField] private float dumping;
    [SerializeField] private float _speed;
    //[SerializeField] private Quaternion _borders_xXyY;
    [Header("Информация только для просмотра, изменения не будут учтены.")]
    [SerializeField] private Vector2 _bordersX;
    [SerializeField] private Vector2 _bordersY;

    public static Quaternion Borders_xXyY {get; private set;}
    //public static Vector2 BorderSize {get; private set;}
    public static float Size {get; private set;}
    public static float ShakePower {get; set;} = 1f;
    private static CameraController instance;

    public bool CanMoving {get; set;} = true;

    private Transform _player;
    private Camera _camera;


    public void Initialize(Vector2 x_borders, Vector2 y_borders) // from BackgroundLoader
    {
        instance = this;

        _bordersX = x_borders;
        _bordersY = y_borders;

        _camera = Camera.main;

        SetBorders();
        Player.PlayerChanged += FindPlayer;
    }

    void Update()
    {
        if (!CanMoving)
            return;

        if (_player != null)
            transform.position = Vector3.Lerp(transform.position, new Vector3 (_player.position.x, _player.position.y + 2f, transform.position.z), dumping * Time.deltaTime);
        transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * Time.deltaTime * _speed;

        transform.position = new Vector3 
        (
            Mathf.Clamp(transform.position.x, _bordersX.x, _bordersX.y),
            Mathf.Clamp(transform.position.y, _bordersY.x, _bordersY.y),
            transform.position.z
        );
    }

    public static void Shake(float power, float tactMult = 1f) => instance.StartCoroutine(instance.Shaking(power, tactMult));

    private IEnumerator Shaking(float power, float timeMultult)
    {
        int tacts = Mathf.RoundToInt(16 * timeMultult);
        float totalPower = 0.1f * power * ShakePower;
        while (tacts > 0)
        {
            transform.position += new Vector3(Random.Range(-totalPower, totalPower), Random.Range(-totalPower, totalPower), 0f);

            tacts --;
            yield return null;
        }
    }

    public void FindPlayer() => _player = Player.PlayerTransform;

    public void SetScale(float newScale, float speed = 1f)
    {
        StartCoroutine(ScaleChanging(newScale, speed));
    }
    
    private IEnumerator ScaleChanging(float newScale, float speed = 1f)
    {
        float settingScale = newScale * _camera.orthographicSize;
        float oldScale = 0;

        while (Mathf.Abs(_camera.orthographicSize - settingScale) > 0.05f)
        {
            oldScale = _camera.orthographicSize;
            _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, settingScale, speed * Time.deltaTime);


            ChangingScale?.Invoke(_camera.orthographicSize / oldScale);
            SetBorders();

            yield return null;
        }
    }

    private void SetBorders()
    {
        Size = _camera.orthographicSize;

        Borders_xXyY = new Quaternion(
            _bordersX.x - (Size / 1.78f) + 0.25f, _bordersX.y + (Size / 1.78f) - 0.25f,
            _bordersY.x - Size, _bordersY.y + Size);

        BordersChange?.Invoke(0);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector2 (Borders_xXyY.x, Borders_xXyY.w), new Vector2 (Borders_xXyY.y, Borders_xXyY.w));
        Gizmos.DrawLine(new Vector2 (Borders_xXyY.x, Borders_xXyY.z), new Vector2 (Borders_xXyY.y, Borders_xXyY.z));
        Gizmos.DrawLine(new Vector2 (Borders_xXyY.x, Borders_xXyY.w), new Vector2 (Borders_xXyY.x, Borders_xXyY.z));
        Gizmos.DrawLine(new Vector2 (Borders_xXyY.y, Borders_xXyY.w), new Vector2 (Borders_xXyY.y, Borders_xXyY.z));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2 (_bordersX.x, _bordersY.y), new Vector2 (_bordersX.y, _bordersY.y));
        Gizmos.DrawLine(new Vector2 (_bordersX.x, _bordersY.x), new Vector2 (_bordersX.y, _bordersY.x));
        Gizmos.DrawLine(new Vector2 (_bordersX.x, _bordersY.y), new Vector2 (_bordersX.x, _bordersY.x));
        Gizmos.DrawLine(new Vector2 (_bordersX.y, _bordersY.y), new Vector2 (_bordersX.y, _bordersY.x));
    }

    public static bool InsideSoundArea(Vector3 position)
    {
        return (position.x > Borders_xXyY.x - sound_overborders) && (position.x < Borders_xXyY.y + sound_overborders) 
            && (position.y > Borders_xXyY.z - sound_overborders) && (position.y < Borders_xXyY.w + sound_overborders);
    }

    public static bool InsideGameField(Vector3 position)
    {
        return (position.x > Borders_xXyY.x) && (position.x < Borders_xXyY.y) 
            && (position.y > Borders_xXyY.z) && (position.y < Borders_xXyY.w);
    }

    public static Vector3 GetRandomFieldPosition()
    {
        return new Vector3(Random.Range(Borders_xXyY.x, Borders_xXyY.y), Random.Range(Borders_xXyY.z, Borders_xXyY.w), 0f);
    }

    public static Vector3 GetRandomFieldPosition(float minDistanceToPlayer)
    {
        Vector3 output = new Vector3(Random.Range(Borders_xXyY.x, Borders_xXyY.y), Random.Range(Borders_xXyY.z, Borders_xXyY.w), 0f);

        while ((output - Player.PlayerTransform.position).magnitude < minDistanceToPlayer)
        {
            output = new Vector3(Random.Range(Borders_xXyY.x, Borders_xXyY.y), Random.Range(Borders_xXyY.z, Borders_xXyY.w), 0f);
        }

        return output;
    }

    public static Vector3 GetRandomFieldPosition(float minDistanceToOrigin, Vector3 origin)
    {
        Vector3 output = new Vector3(Random.Range(Borders_xXyY.x, Borders_xXyY.y), Random.Range(Borders_xXyY.z, Borders_xXyY.w), 0f);

        while ((output - origin).magnitude < minDistanceToOrigin)
        {
            output = new Vector3(Random.Range(Borders_xXyY.x, Borders_xXyY.y), Random.Range(Borders_xXyY.z, Borders_xXyY.w), 0f);
        }

        return output;
    }
}
