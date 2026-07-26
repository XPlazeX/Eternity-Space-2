using System.Collections;
using UnityEngine;

public class JinPlanetAnimation : MonoBehaviour
{
    private const string planet_animation_trigger = "Fire";
    private const int level_completed_unlock = 41;

    [SerializeField] private int _targetLevel;
    [SerializeField] private float _delayTime;
    [SerializeField] private string _triggeringDialogueOnEnd;
    [SerializeField] private Sprite _explodedSprite;

    private void Start() {
        print(GameSessionInfoHandler.CurrentLevel > _targetLevel);
        if (Unlocks.HasUnlock(level_completed_unlock) || (GameSessionInfoHandler.CurrentLevel > _targetLevel))
        {
            GetComponent<SpriteRenderer>().sprite = _explodedSprite;
            GetComponent<Animator>().enabled = false;
            return;
        }

        if (GameSessionInfoHandler.CurrentLevel != _targetLevel)
            return;

        StartCoroutine(Countdown());
    }

    public void TriggerPlanetAnimation()
    {
        print("TriggerAnimation");
        GetComponent<Animator>().SetTrigger(planet_animation_trigger);
    }

    public void TriggerEndDialogue()
    {
        GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>().TriggerDialogue(_triggeringDialogueOnEnd);
    }

    private IEnumerator Countdown()
    {
        print("StartPlanetCountdown");
        float timer = _delayTime;

        while (timer > 0)
        {
            timer -= ESTime.worldDeltaTime;
            yield return null;
        }

        TriggerPlanetAnimation();
    }

}
