using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ElitizerManager))]
public class Spawner : MonoBehaviour
{
    public delegate void damageBodyOperationHandler(DamageBody db);
    public delegate void damageBodyEvent();
    public static event damageBodyOperationHandler DamageBodySpawned;
    public static event damageBodyEvent DamageBodyDeathed;

    [SerializeField] private EnemyHealthBar _enemyHealthBar;
    [SerializeField] private Text _waveIndexLabel;
    [SerializeField] private EnemyCountUI _enemyCountUI;

    private static PullForObjects _HPBarsPool;
    private static ElitizerManager _elitizerManager;

    public static void InitializeHPBars(EnemyHealthBar ehb)
    {
        _HPBarsPool = new PullForObjects(ehb);
    }

    public static int EnemyCount {get; private set;} = 0;

    public void Initialize()
    {
        PrintCountUI(0);
        PrintBonusCountUI(0);
        _HPBarsPool = new PullForObjects(_enemyHealthBar);
        _elitizerManager = GetComponent<ElitizerManager>();
    }

    public static DamageBody SpawnDamageBody(DamageBody dbSample, Vector3 spawnPosition = default(Vector3), bool eliteSpawn = false)
    {
        if (dbSample == null)
        {
            Debug.Log("Empty damageBody!");
            return null;
        }

        Vector3 spawningPosition = spawnPosition;

        if (spawnPosition == default(Vector3))
        {
            spawningPosition = new Vector3(
                Random.Range(ArenaLocal.WNegX + 2f, ArenaLocal.WPosX - 2f),
                ArenaLocal.WPosY + 3f, 0f);
        }

        DamageBody db = Instantiate(dbSample, spawningPosition, Quaternion.Euler(0, 0, 180f)).GetComponent<DamageBody>();

        EnemyCount ++;
        db.Deathed += SubstractEnemyCount;

        bool elite = eliteSpawn;

        if (!elite)
        {
            elite = _elitizerManager.WillElitize();
        }

        if (elite)
        {
            _elitizerManager.Elitize(db);
        }

        if (db.GetComponent<Boss>() == null)
        {
            InitializeHPBar(db, elite);
        }

        DamageBodySpawned?.Invoke(db);

        return db;
    }

    private static void SubstractEnemyCount()
    {
        EnemyCount --;
        DamageBodyDeathed?.Invoke();
    }

    public static void InitializeHPBar(DamageBody targetBody, bool elite = false)
    {
        if (targetBody.GetComponent<Boss>() != null)
            return;

        EnemyHealthBar hpBar = _HPBarsPool.GetGameObject().GetComponent<EnemyHealthBar>();
        hpBar.transform.SetParent(targetBody.transform);

        Vector3 offsetBar = Vector3.up;
        if (targetBody.GetComponent<BoxCollider2D>() != null)
        {
            offsetBar = Vector3.up * (targetBody.GetComponent<BoxCollider2D>().size.y + 0.1f);
        }

        hpBar.transform.position = targetBody.transform.position + offsetBar;
        targetBody.DamageTaking += hpBar.SetHP;
        targetBody.HealthModified += hpBar.OnHealthModified;

        hpBar.Initialize(targetBody.HitPoints, elite);
    }

    public void HideAll()
    {
        _waveIndexLabel.color = Color.clear;
        _enemyCountUI.Clear();
    }
    public void PrintProgressUI(string text) => _waveIndexLabel.text = text;
    public void PrintCountUI(int count) => _enemyCountUI.SetCount(count);
    public void PrintBonusCountUI(int count) => _enemyCountUI.SetBonusCount(count);
    public void HideLabel() => _enemyCountUI.HideHead();
    public void SetWaveHeadText(string text) => _enemyCountUI.SetHead(text);
    public void ShowReinforcementUI() => _enemyCountUI.ShowReinforcements();
    public void SetReinforcementDelay(int secs) => _enemyCountUI.SetReinforcementsDelay(secs);
}
