using UnityEngine;

public class SimpleCoreV1 : MonoBehaviour
{
    public static event System.Action<int> RadiantLevelChanged;

    [SerializeField] private AnimationCurve downgradeChanceByDamageTaken = new AnimationCurve(new Keyframe(0, 0.25f), new Keyframe(50, 0.85f));
    [Header("Messages")]
    [SerializeField] private string powerupMessageID;
    [SerializeField] private string overchargedMessageID;
    [SerializeField] private string dmgPowerdownMessageID;

    private MainWeaponHandler _mwh;
    private int _radiantLevel = 0;

    public static int RadiantLevel => Instance._radiantLevel;
    
    private static SimpleCoreV1 Instance;

    private void Awake() 
    {
        Instance = this;
        RadiantLevelChanged?.Invoke(Instance._radiantLevel);
    }

    void Start()
    {
        _mwh = FindAnyObjectByType<MainWeaponHandler>();
    }

    private void OnEnable() {
        PlayerShipData.HealthDamageTaked += OnHealthDamageTaked;
    }

    void OnDisable()
    {
        PlayerShipData.HealthDamageTaked -= OnHealthDamageTaked;
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.P))
        {
            RadiantPowerup();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            RadiantPowerdown();
        }
        #endif
    }

    public static void RadiantPowerup()
    {
        Instance._mwh.RadiantUpgrade(out bool overcharged);

        if (!overcharged)
        {
            Instance._radiantLevel++;
            RadioManager.RequestMessage(new RadioMessageRequest(Instance.powerupMessageID, RadioChannel.Commentary));
        }else
        {
            RadioManager.RequestMessage(new RadioMessageRequest(Instance.overchargedMessageID, RadioChannel.Commentary));
        }

        RadiantLevelChanged?.Invoke(Instance._radiantLevel);
    }

    private void RadiantPowerdown()
    {
        _mwh.RadiantDowngrade(out bool overzero);

        if (!overzero)
        {
            Instance._radiantLevel--;
            RadioManager.RequestMessage(new RadioMessageRequest(Instance.dmgPowerdownMessageID, RadioChannel.Reaction));
        }

        RadiantLevelChanged?.Invoke(Instance._radiantLevel);
    }

    private void OnHealthDamageTaked(int damage)
    {
        float chance = downgradeChanceByDamageTaken.Evaluate(damage);
        if (Random.value < chance)
        {
            RadiantPowerdown();
        } else
        {
            
        }
    }
}
