using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public delegate void damageBodyOperationHandler(DamageBody db);
    public static event damageBodyOperationHandler DamageBodySpawned;

    [SerializeField] private EnemyHealthBar _enemyHealthBar;
    [SerializeField] private Text _waveIndexLabel;
    [SerializeField] private EnemyCountUI _enemyCountUI;

    private static PullForObjects _HPBarsPool;
    public static void InitializeHPBars(EnemyHealthBar ehb)
    {
        _HPBarsPool = new PullForObjects(ehb);
    }

    public void Initialize()
    {
        PrintCountUI(0);
        PrintBonusCountUI(0);
        _HPBarsPool = new PullForObjects(_enemyHealthBar);
    }

    public static DamageBody SpawnDamageBody(DamageBody dbSample, Vector3 spawnPosition = default(Vector3))
    {
        Vector3 spawningPosition = spawnPosition;

        if (spawnPosition == default(Vector3))
        {
            spawningPosition = new Vector3(
                Random.Range(CameraController.Borders_xXyY.x + 2f, CameraController.Borders_xXyY.y - 2f),
                (CameraController.Borders_xXyY.w + 3f), 0f);
        }

        DamageBody db = Instantiate(dbSample, spawningPosition, Quaternion.Euler(0, 0, 180f)).GetComponent<DamageBody>();

        DamageBodySpawned?.Invoke(db);

        return db;
    }

    public static void InitializeHPBar(DamageBody targetBody)
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

        hpBar.Initialize(targetBody.HitPoints);
    }

    public void PrintProgressUI(string text) => _waveIndexLabel.text = text;
    public void PrintCountUI(int count) => _enemyCountUI.SetCount(count);
    public void PrintBonusCountUI(int count) => _enemyCountUI.SetBonusCount(count);
    public void HideLabel() => _enemyCountUI.HideHead();
    public void SetWaveHeadText(string text) => _enemyCountUI.SetHead(text);
}
