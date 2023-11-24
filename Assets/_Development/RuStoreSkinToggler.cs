using UnityEngine;

public class RuStoreSkinToggler : TogglerButton
{
    public void Initialize()
    {
        if (!Dev.RuStoreVersion)
            return;

        if (PlayerPrefs.GetInt("RuStoreSkins", 0) == 1)
        {
            Toggle();
        }
    }

    public override void Toggle()
    {
        if (!Dev.RuStoreVersion)
            return;

        base.Toggle();

        if (ON)
        {
            Dev.RuStoreVersionSprites = true;
            PlayerPrefs.SetInt("RuStoreSkins", 1);
        }
        
        else
        {
            Dev.RuStoreVersionSprites = false;
            PlayerPrefs.SetInt("RuStoreSkins", 0);
        }
    }
}
