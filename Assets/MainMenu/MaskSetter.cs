using UnityEngine;

public class MaskSetter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < srs.Length; i++)
        {
            srs[i].maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
        }        
    }

}
