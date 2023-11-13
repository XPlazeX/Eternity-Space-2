using UnityEngine;

public class BossDistributor : MonoBehaviour
{
    [SerializeField] private BossBar _bossBarSample;

    private int _activeBosses = 0;

    public BossBar InitializeBossBar(int hp)
    {
        BossBar bb = Instantiate(_bossBarSample);
        bb.Initialize(hp, _activeBosses);

        _activeBosses ++;
        return bb;
    }

    public void UnregisterBossBar()
    {
        _activeBosses --;
    }
}
