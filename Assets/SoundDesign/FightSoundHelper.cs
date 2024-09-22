using UnityEngine;

public class FightSoundHelper : MonoBehaviour
{
    private const int default_bullet_sound_id = 1;

    [SerializeField] private SoundObject[] _soundObjects;
    [Space()]
    [SerializeField] private BulletSoundPreset[] _bulletPresets;
    [SerializeField] private SoundObject _defaultSound;

    private static FightSoundHelper instance;

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public static void PlaySound(int id, Vector3 position)
    {
        SoundPlayer.PlaySound(instance._soundObjects[id], position);
    }

    public static void PlayBulletSound(int bulletID, int count, Vector3 position)
    {
        if (bulletID >= instance._bulletPresets.Length)
        {
            throw new System.ArgumentOutOfRangeException("Несуществующий ID звука пули (значение больше размера коллекции).");
        }

        if (instance._bulletPresets[bulletID].silent)
            return;

        if (instance._bulletPresets[bulletID].GetSoundByCount(count) == null)
        {
            PlaySound(default_bullet_sound_id, position);
            return;
        }

        SoundPlayer.PlaySound(instance._bulletPresets[bulletID].GetSoundByCount(count), position);
    }

    [System.Serializable]
    private struct BulletSoundPreset
    {
        public bool silent;
        public SoundObject mainSound;
        public bool boostingSound;
        public int boostingBulletCount;
        public SoundObject boostedSound;

        public SoundObject GetSoundByCount(int count)
        {
            if (boostingSound && count >= boostingBulletCount)
                return boostedSound;
            else
                return mainSound;
        }
    }
}
