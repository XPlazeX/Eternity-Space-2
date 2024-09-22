using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageBody))]
public class BossHPEvents : MonoBehaviour
{
    public const string boss_hp_data_collection = "BossHPEvent";

    [Header("Оставьте пустым, если не нужны диалоги")]
    [SerializeField] private string _dialoguePrefix;
    [SerializeField] private BossHPEvent[] _bossHpEvents;

    private DamageBody _damageBody;

    private void OnEnable() {
        _damageBody = GetComponent<DamageBody>();
        _damageBody.DamageTaking += CheckHP;

        for (int i = 0; i < _bossHpEvents.Length; i++)
        {
            _bossHpEvents[i].SetDialogprefix(_dialoguePrefix);
        }
    }

    private void CheckHP(int n)
    {
        float percentage = (float)_damageBody.HitPoints / (float)_damageBody.StartHP;

        for (int i = 0; i < _bossHpEvents.Length; i++)
        {
            _bossHpEvents[i].TryTrigger(percentage, i);
        }
    }
    
    [System.Serializable]
    public class BossHPEvent
    {
        [Range(0, 1f)] public float targetPercentageOfFull;
        public string dialogue;
        public MonoBehaviour[] toggleStateBehaviours;
        public GameObject[] toggleActiveGameObjects;

        private bool triggered = false;
        private string dialogDataPrefix;

        public void SetDialogprefix(string prefix) => dialogDataPrefix = prefix;

        public void TryTrigger(float percentage, int dialogueID = 0)
        {
            if (triggered || percentage > targetPercentageOfFull)
                return;

            if (!string.IsNullOrEmpty(dialogue) && !GameSessionInfoHandler.GetDataCollection(dialogDataPrefix + boss_hp_data_collection).Contains(dialogueID))
            {
                GameObject.FindWithTag("BetweenScenes").GetComponent<DialogueOpener>().TriggerDialogue(dialogue);
                GameSessionInfoHandler.AddValueToCollection(dialogDataPrefix + boss_hp_data_collection, dialogueID);
                ParryingHandler.ConstParry();
            }

            for (int i = 0; i < toggleStateBehaviours.Length; i++)
            {
                toggleStateBehaviours[i].enabled = !toggleStateBehaviours[i].enabled;
            }

            for (int i = 0; i < toggleActiveGameObjects.Length; i++)
            {
                toggleActiveGameObjects[i].SetActive(!toggleActiveGameObjects[i].activeInHierarchy);
            }

            triggered = true;
        }
    }
}
