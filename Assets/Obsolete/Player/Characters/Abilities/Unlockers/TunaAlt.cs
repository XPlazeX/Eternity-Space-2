using UnityEngine;

public class TunaAlt : MonoBehaviour
{
    private const int achievement_id = 900;

    private float _lastTime;

    private void Start() {
        if (Unlocks.HasUnlock(achievement_id))
        {
            Destroy(this);
        }

        _lastTime = Time.time;
    }

    private void OnEnable() {
        Nuke.NukeExploded += OnNukeExploded;
    }

    private void OnDisable() {
        Nuke.NukeExploded -= OnNukeExploded;
    }

    private void OnNukeExploded()
    {
        float time = Time.time;

        if (Mathf.Abs(time - _lastTime) <= 10f)
        {
            Unlocks.NewUnlock(achievement_id);
            Destroy(this);
        }

        _lastTime = time;
    }
}
