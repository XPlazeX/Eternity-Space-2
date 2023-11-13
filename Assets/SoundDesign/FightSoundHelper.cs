using UnityEngine;

public class FightSoundHelper : MonoBehaviour
{
    [SerializeField] private SoundObject[] _soundObjects;

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
}
