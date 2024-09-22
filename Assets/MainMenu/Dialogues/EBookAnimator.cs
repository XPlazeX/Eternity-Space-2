using UnityEngine;

public class EBookAnimator : MonoBehaviour
{
    private EBook _ebook;

    private void Start() {
        _ebook = SceneStatics.SceneCore.GetComponent<EBook>();
    }

    public void CardInserted()
    {
        _ebook.CardInserted();
    }

    public void CardSpriteSwitch()
    {
        _ebook.CardSpriteSwitch();
    }
}
