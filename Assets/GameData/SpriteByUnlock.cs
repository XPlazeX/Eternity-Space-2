using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteByUnlock : MonoBehaviour
{
    [Header("Спрайты заменяются с учётом порядка")]
    [SerializeField] private SpriteByUnlockRequire[] _spritesByUnlockRequires;

    private void Start() 
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        for (int i = 0; i < _spritesByUnlockRequires.Length; i++)
        {
            if (_spritesByUnlockRequires[i].available)
                sr.sprite = _spritesByUnlockRequires[i].sprite;
        }
    }

    [System.Serializable]
    private struct SpriteByUnlockRequire
    {
        public Sprite sprite;
        public UnlockRequire[] unlockRequires;

        public bool available => Unlocks.HasUnlocks(unlockRequires);
    }
}
