using UnityEngine;
using System.Collections.Generic;

public class BossDistributor : MonoBehaviour
{
    [SerializeField] private BossBar _bossBarSample;

    private List<bool> BossbarSlots = new List<bool>();

    public BossBar InitializeBossBar(int hp)
    {
        int slot = -1;

        for (int i = 0; i < BossbarSlots.Count; i++)
        {
            if (!BossbarSlots[i])
            {
                slot = i;
                BossbarSlots[i] = true;
            }
        }

        if (slot == -1)
        {
            BossbarSlots.Add(true);
            slot = BossbarSlots.Count - 1;
        }

        BossBar bb = Instantiate(_bossBarSample);
        bb.Initialize(hp, slot);

        return bb;
    }

    public void UnregisterBossBar(int pos)
    {
       BossbarSlots[pos] = false;
    }
}
