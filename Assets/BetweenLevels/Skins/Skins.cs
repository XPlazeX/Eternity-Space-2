using UnityEngine;

public static class Skins
{
    public static int SOCurrentSkin()
    {
        if (Unlocks.ValueOfUnlock(590) == 1)
            return 2;

        if (Dev.RuStoreVersionSprites)
            return 1;

        return 0;
    }
}
