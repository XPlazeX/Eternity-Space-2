using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public const float SCREEN_OUTSIDE_OFFSET = 0.25f;
    public const float sound_overborders = 4f;
    public const float defaultSize = 8.2f;

    public delegate void scaleOperation(float val);
    public event scaleOperation ChangingScale;
    public event scaleOperation BordersChange;
    public static System.Action CameraMoved;

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
    public bool CustomTarget {get; private set;} = false;
    public Vector2 CameraOffset {get; private set;} = new Vector2(0, 2f);

    private static readonly Vector2 _defaultOffset = new Vector2(0, 2f);
    private Transform _customTarget;
    private Transform _player;
    private Camera _camera;
    private float _startDumping;


    public void Initialize(Vector2 x_borders, Vector2 y_borders) // from BackgroundLoader
    {
        instance = this;
        _startDumping = dumping;

        _bordersX = x_borders;
        _bordersY = y_borders;

        _camera = Camera.main;

        SetBorders();
        Player.PlayerChanged += FindPlayer;
    }

    void LateUpdate()
    {
        if (!CanMoving)
        {
            CameraMoved?.Invoke();
            return;
        }
            

        if (_player != null && !CustomTarget)
            transform.position = Vector3.Lerp(transform.position, new Vector3 (_player.position.x + CameraOffset.x, _player.position.y + 2f + CameraOffset.y, transform.position.z), dumping * Time.deltaTime);
        else if (CustomTarget && _customTarget != null)
            transform.position = Vector3.Lerp(transform.position, new Vector3 (_customTarget.position.x + CameraOffset.x, _customTarget.position.y + 2f + CameraOffset.y, transform.position.z), dumping * Time.deltaTime);
        //transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * Time.deltaTime * _speed;

        if (!CustomTarget)
            transform.position = new Vector3 
            (
                Mathf.Clamp(transform.position.x, _bordersX.x, _bordersX.y),
                Mathf.Clamp(transform.position.y, _bordersY.x, _bordersY.y),
                transform.position.z
            );

        CameraMoved?.Invoke();
    }

    public static void ToggleCustomDumping(bool tog, float newDumping = 0)
    {
        if (tog)
        {
            instance.dumping = newDumping;
        } else
        {
            instance.dumping = instance._startDumping;
        }
    }

    public static void ToggleCustomTarget(bool tog, Transform target = null)
    {
        instance.CustomTarget = tog;
        if (tog)
        {
            instance._customTarget = target;
            if (target == null)
                Debug.Log("Устанавливается цель null для customTarget камеры. Камера перестанет двигаться.");
        } else
        {
            instance.FindPlayer();
        }
    }

    public static void SetCustomOffset(Vector2 offset)
    {
        instance.CameraOffset = offset;
    }

    public static void DisableCustomOffset()
    {
        instance.CameraOffset = _defaultOffset;
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
        float widthRatio = (float)Screen.width / Screen.height;

        Borders_xXyY = new Quaternion(
            _bordersX.x - (Size * widthRatio) - SCREEN_OUTSIDE_OFFSET, _bordersX.y + (Size * widthRatio) + SCREEN_OUTSIDE_OFFSET,
            _bordersY.x - Size - SCREEN_OUTSIDE_OFFSET, _bordersY.y + Size + SCREEN_OUTSIDE_OFFSET);

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

    private const int random_field_max_iterations = 64;

    public static Vector3 GetRandomFieldPosition(float minDistanceToPlayer)
    {
        Vector3 output = new Vector3(Random.Range(Borders_xXyY.x, Borders_xXyY.y), Random.Range(Borders_xXyY.z, Borders_xXyY.w), 0f);

        int iterations = 0;

        while (((output - Player.PlayerTransform.position).magnitude < minDistanceToPlayer) && (iterations < random_field_max_iterations))
        {
            output = new Vector3(Random.Range(Borders_xXyY.x, Borders_xXyY.y), Random.Range(Borders_xXyY.z, Borders_xXyY.w), 0f);
            iterations ++;
        }

        return output;
    }

    public static Vector3 GetRandomFieldPosition(float minDistanceToOrigin, Vector3 origin)
    {
        Vector3 output = new Vector3(Random.Range(Borders_xXyY.x, Borders_xXyY.y), Random.Range(Borders_xXyY.z, Borders_xXyY.w), 0f);

        int iterations = 0;

        while (((output - origin).magnitude < minDistanceToOrigin) && (iterations < random_field_max_iterations))
        {
            output = new Vector3(Random.Range(Borders_xXyY.x, Borders_xXyY.y), Random.Range(Borders_xXyY.z, Borders_xXyY.w), 0f);
            iterations ++;
        }

        return output;
    }

    public static Vector3 GetRandomFieldPosition(float minDistanceToOrigin, Vector3 origin, float borderOffset)
    {
        Vector3 output = new Vector3(Random.Range(Borders_xXyY.x + borderOffset, Borders_xXyY.y - borderOffset), Random.Range(Borders_xXyY.z + borderOffset, Borders_xXyY.w - borderOffset), 0f);

        int iterations = 0;

        while (((output - origin).magnitude < minDistanceToOrigin) && (iterations < random_field_max_iterations))
        {
            output = new Vector3(Random.Range(Borders_xXyY.x + borderOffset, Borders_xXyY.y - borderOffset), Random.Range(Borders_xXyY.z + borderOffset, Borders_xXyY.w - borderOffset), 0f);
            iterations ++;
        }

        return output;
    }
}
