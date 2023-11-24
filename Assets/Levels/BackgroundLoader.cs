using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BackgroundLoader
{
    private const float normalCameraBorderX = 6.5f;
    private const float normalCameraBorderY = 4f;
    private const float normalCameraSizeX = 11.1f; // 4.6 + 6.5
    private const float normalCameraSizeY = 12.2f; // 8.2 + 4

    [Header("Нормальный размер 8.75 на 5")]
    [SerializeField] private bool _beaconBG = false;
    [SerializeField][Range(0f, 2.5f)] private float _cameraBordersSizeXMultiplier = 1f;
    [SerializeField][Range(0f, 2.5f)] private float _cameraBordersSizeYMultiplier = 1f;
    [Space()]
    [SerializeField] private BackgroundObject[] _backgroundObjects;
    [Space()]
    [SerializeField] private string _backgroundSpriteFilename;
    [SerializeField] private Color _backgroundColor;
    [SerializeField] private bool _flipX;
    [SerializeField] private bool _flipY;
    [Header("Указывайте false, если используется особая станция(ии)")]
    [SerializeField] private bool _hasStation = false;
    [SerializeField] private string _stationSpriteFilename;
    [Space()]
    //[SerializeField] private Color _downColorMult;
    [SerializeField] private Color _upperColorMult;
    [Space()]
    [SerializeField] private Gradient _dustGradient;
    [SerializeField] private float _dustEmissionMultiplier = 1f;
    [SerializeField] private Gradient _fogGradient;
    [SerializeField] private float _fogEmissionMultiplier = 1f;
    [SerializeField][Range(-3f, 3f)] private float _windSpeed = 0f;
    [Space()]
    [SerializeField] private int _objectsToBeaconLevel = 3;

    private Transform _bgTransform;

    public bool Beacon => _beaconBG;

    public void Load()
    {
        if (Beacon)
            BeaconInitialize();
        else 
            Initialize();
    }

    private void GenericInitialize()
    {
        _bgTransform = GameObject.FindWithTag("Level core").transform;

        Camera.main.GetComponent<CameraController>().Initialize(
            new Vector2(-normalCameraBorderX * _cameraBordersSizeXMultiplier, normalCameraBorderX * _cameraBordersSizeXMultiplier),
            new Vector2(-normalCameraBorderY * _cameraBordersSizeYMultiplier, normalCameraBorderY * _cameraBordersSizeYMultiplier));
    }

    private void Initialize()
    {
        GenericInitialize();
        
        for (int i = 0; i < _backgroundObjects.Length; i++)
        {
            InitializeObject(_backgroundObjects[i]);
        }

        ModBG(_backgroundColor);

        if (_hasStation)
            GameObject.FindWithTag("BG-Station").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(_stationSpriteFilename);

        //GameObject.FindWithTag("BG-DCM").GetComponent<SpriteRenderer>().color = _downColorMult;
        GameObject.FindWithTag("BG-UCM").GetComponent<SpriteRenderer>().color = _upperColorMult;

        PreparePS();
    }

    private void BeaconInitialize()
    {
        GenericInitialize();

        List<Color> colorCheme = new List<Color>();

        float startPickValue = GameSessionInfoHandler.GetSessionSave().BeaconColorValue;
        float stepColor = 0;
        int maxcounter = 0;
        int counter = 0;

        switch (GameSessionInfoHandler.GetSessionSave().BeaconFlipState)
        {
            case 0:
                stepColor = 0.15f;
                maxcounter = 6;
                break;
            case 1:
                stepColor = 0.25f;
                maxcounter = 4;
                break;
            case 2:
                stepColor = 0.33f;
                maxcounter = 3;
                break;
            case 3:
                stepColor = 0.5f;
                maxcounter = 2;
                break;
            default:
                throw new System.Exception("Невозможное состояние цвета.");
        }

        for (int i = 0; i < maxcounter; i++)
        {
            //print($"Pick value : {startPickValue}");
            colorCheme.Add(SceneStatics.CombineGradient.Evaluate(startPickValue % 1));
            startPickValue += stepColor;
        }  

        ModBG(colorCheme[counter]);

        for (int i = 0; i < _objectsToBeaconLevel; i++)
        {
            InitializeObject(_backgroundObjects[i], colorCheme[counter]);    
            counter = ClampCounter(counter, maxcounter);   
        }

        counter = ClampCounter(counter, maxcounter);

        // SetColorNotAlpha(GameObject.FindWithTag("BG-DCM").GetComponent<SpriteRenderer>(), colorCheme[counter]);
        // counter = ClampCounter(counter, maxcounter);

        SetColorNotAlpha(GameObject.FindWithTag("BG-UCM").GetComponent<SpriteRenderer>(), colorCheme[counter]);
        counter = ClampCounter(counter, maxcounter);

        PreparePS();

        BeaconBGLoader bbgl = MonoBehaviour.Instantiate(Resources.Load<BeaconBGLoader>("Beacon/BeaconBG"));
        bbgl.Initialize();
    }

    private void ModBG(Color col)
    {
        SpriteRenderer bgsr =  GameObject.FindWithTag("BG-Background").GetComponent<SpriteRenderer>();
        bgsr.sprite = Resources.Load<Sprite>(_backgroundSpriteFilename);
        bgsr.color = col;
        bgsr.flipX = _flipX;
        bgsr.flipY = _flipY;
    }

    private int ClampCounter(int input, int maxVal)
    {
        int temp = input + 1;
        if (temp >= maxVal)
            return 0;
        else
            return temp;
    }

    private void InitializeObject(BackgroundObject backgroundObject)
    {
        var obj = MonoBehaviour.Instantiate(backgroundObject.SpawnObject, backgroundObject.Position, Quaternion.Euler(0, 0, backgroundObject.Rotation));

        if (!obj.GetComponent<CameraObjects>())
        {
            obj.transform.SetParent(_bgTransform);
        }

        obj.GetComponent<SpriteRenderer>().color = backgroundObject.SpawnColor;
    }

    private void InitializeObject(BackgroundObject backgroundObject, Color color)
    {
        var obj = MonoBehaviour.Instantiate(backgroundObject.SpawnObject, backgroundObject.Position, Quaternion.Euler(0, 0, backgroundObject.Rotation));

        if (!obj.GetComponent<CameraObjects>())
        {
            obj.transform.SetParent(_bgTransform);
            Debug.Log($"bindToBG: {obj.gameObject.name}");
        }

        SetColorNotAlpha(obj.GetComponent<SpriteRenderer>(), color);
    }

    private void SetColorNotAlpha(SpriteRenderer sr, Color color)
    {
        float brightMult = (sr.color.r + sr.color.g + sr.color.b) / 3f;
        sr.color = new Color(color.r * brightMult, color.g * brightMult, color.b * brightMult, sr.color.a * sr.color.a);
    }

    private void PreparePS()
    {
        GameObject.FindWithTag("Dust").GetComponent<Transform>().localScale = new Vector3(
            (4.6f + (8.75f * _cameraBordersSizeXMultiplier)) / normalCameraSizeX,
            (8.2f + (5f * _cameraBordersSizeYMultiplier)) / normalCameraSizeY, 1f);

        GameObject.FindWithTag("Fog").GetComponent<Transform>().localScale = new Vector3(
            (4.6f + (8.75f * _cameraBordersSizeXMultiplier)) / normalCameraSizeX,
            (8.2f + (5f * _cameraBordersSizeYMultiplier)) / normalCameraSizeY, 1f);

        ParticleSystem ps = GameObject.FindWithTag("Dust").GetComponent<ParticleSystem>();
        var col = ps.colorOverLifetime;
        col.color = _dustGradient; 
        var em = ps.emission;
        em.rateOverTime = 20f * _dustEmissionMultiplier;
        var vel = ps.velocityOverLifetime;
        vel.speedModifier = _windSpeed;

        ps = GameObject.FindWithTag("Fog").GetComponent<ParticleSystem>();
        col = ps.colorOverLifetime;
        col.color = _fogGradient; 
        em = ps.emission;
        em.rateOverTime = 1.25f * _fogEmissionMultiplier;
        vel = ps.velocityOverLifetime;
        vel.speedModifier = _windSpeed;
    }
}

[System.Serializable]
public class BackgroundObject
{
    [SerializeField] private Transform _object;
    [SerializeField] private Color _color;
    [SerializeField] private Vector3 _position;
    [SerializeField] private float _rotation;
    [SerializeField][Range(0, 180f)] private float _rotationSpread;

    public Transform SpawnObject => _object;
    public Color SpawnColor => _color;
    public Vector3 Position => _position;
    public float Rotation => _rotation + Random.Range(-_rotationSpread, _rotationSpread);
}
