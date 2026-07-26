using UnityEngine;

public class SceneStatics : MonoBehaviour
{
    public delegate void loadProcess();
    public static event loadProcess CoresLoaded;

    public static float ChaosMultiplier {get; set;} = 0.15f;
    public static float LevelEnemyHPMultiplier {get; set;} = 1f;

    public static float GameTimer {get; private set;} = 0f;
    public static bool CoresFinded {get; private set;} = false;

    public static GameObject SceneCore {get; private set;}
    public static GameObject CharacterCore {get; private set;}
    public static GameObject UICore {get; private set;}
    public static GameObject AudioCore {get; private set;}
    public static Lab AngarLab {get; private set;}

    public static Color ScoutColor => new Color(0.87f, 0.39f, 1f);
    public static Color StormtrooperColor => new Color(1f, 0.82f, 0.3f);
    public static Color FighterColor => new Color(0.39f, 1f, 0.49f);
    public static Color BomberColor => new Color(1f, 0.39f, 0.39f);
    public static Color ScientificColor => new Color(0.38f, 0.88f, 1f);
    public static Color UniversalColor => new Color(0.7f, 0.7f, 0.7f);
    public static Color UniqueColor => new Color(0.53f, 0.11f, 0.35f);

    public static float MultiplyByChaos (float input) => Random.Range(input - (input * ChaosMultiplier), input + (input * ChaosMultiplier));
    public static Vector2 FlatVector(Vector3 vec) => new Vector2(vec.x, vec.y);

    [SerializeField] private Gradient _healthGradient;
    public static Gradient HealthGradient = null;

    [SerializeField] private Gradient _eliteHealthGradient;
    public static Gradient EliteHealthGradient = null;

    [SerializeField] private Gradient _combineGradient;
    public static Gradient CombineGradient = null;

    private void Awake() 
    {
        GameTimer = 0f;
        HealthGradient = _healthGradient;
        EliteHealthGradient = _eliteHealthGradient;
        CombineGradient = _combineGradient;

        CoresInitialization();
    }

    private void Update()
    {
        GameTimer += ESTime.worldDeltaTime;
    }

    public static Color GetClassColor(GearCompatibility.ShipClass shipClass)
    {
        switch (shipClass)
        {
            case GearCompatibility.ShipClass.Scout:
                return ScoutColor;

            case GearCompatibility.ShipClass.Stormtrooper:
                return StormtrooperColor;

            case GearCompatibility.ShipClass.Fighter:
                return FighterColor;

            case GearCompatibility.ShipClass.Bomber:
                return BomberColor;

            case GearCompatibility.ShipClass.Scientific:
                return ScientificColor;

            case GearCompatibility.ShipClass.Universal:
                return UniversalColor;

            case GearCompatibility.ShipClass.Unique:
                return UniqueColor;
            
            default:
                return Color.white;
        }
    }

    private void CoresInitialization()
    {
        SceneCore = GameObject.FindWithTag("SceneCore");
        CharacterCore = GameObject.FindWithTag("CharacterCore");
        UICore = GameObject.FindWithTag("UICore");
        AudioCore = GameObject.FindWithTag("AudioCore");

        AudioCore.GetComponent<SoundPlayer>().Initialize();

        GameObject lab = GameObject.FindWithTag("Lab");

        if (lab != null)
            AngarLab = lab.GetComponent<Lab>();

        CoresFinded = true;
        CoresLoaded?.Invoke();

    }

    private void OnDisable() {
        CoresLoaded = null;
        CoresFinded = false;
    }

    public static string PreferFloatView(float input, int accuracy = 2)
    {
        return (Mathf.Floor(input * (Mathf.Pow(10f, accuracy))) / (Mathf.Pow(10f, accuracy))).ToString().Replace(',', '.');
    }
}
