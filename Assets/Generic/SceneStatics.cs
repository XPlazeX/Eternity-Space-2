using UnityEngine;

public class SceneStatics : MonoBehaviour
{
    public delegate void loadProcess();
    public static event loadProcess CoresLoaded;

    public static float ChaosMultiplier {get; set;} = 0.15f;
    public static float LevelEnemyHPMultiplier {get; set;} = 1f;

    public static bool CoresFinded {get; private set;} = false;

    public static GameObject SceneCore {get; private set;}
    public static GameObject CharacterCore {get; private set;}
    public static GameObject UICore {get; private set;}
    public static GameObject AudioCore {get; private set;}
    public static Lab AngarLab {get; private set;}

    public static float MultiplyByChaos (float input) => Random.Range(input - (input * ChaosMultiplier), input + (input * ChaosMultiplier));
    public static Vector2 FlatVector(Vector3 vec) => new Vector2(vec.x, vec.y);

    [SerializeField] private Gradient _healthGradient;
    public static Gradient HealthGradient = null;

    [SerializeField] private Gradient _combineGradient;
    public static Gradient CombineGradient = null;

    private void Awake() 
    {
        HealthGradient = _healthGradient;
        CombineGradient = _combineGradient;
        CoresInitialization();
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
}
