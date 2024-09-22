using UnityEngine;

public class EternityClock : MonoBehaviour
{
    [SerializeField] private Transform _minuteHand;
    [SerializeField] private Transform _hourHand;
    [SerializeField] private float _clockSpeed;
    [SerializeField] private SpriteRenderer _daytimeIndicator;
    [SerializeField] private Sprite _daySprite;
    [SerializeField] private Sprite _nightSprite;
    [Space()]
    [SerializeField] private EternityClockTrigger _startTrigger;
    [SerializeField] private EternityClockTrigger _endTrigger;
    [Space()]
    [SerializeField] private EternityClockCondition[] _eternityConditions;

    private float _minute;
    private float _hour;
    private bool _span = false;

    public static bool Parsing => GlobalSaveHandler.GetSave().EternityParse;

    private void Start() 
    {
        GlobalSave gsave = GlobalSaveHandler.GetSave();

        _minute = gsave.EternityMinute;
        _hour = gsave.EternityHour;

        if (gsave.EternityParse)
        {
            StartSpan();
            return;   
        }

        if (_minute <= 0f)
        {
            System.DateTime nowTime = System.DateTime.Now;

            _minute = nowTime.Minute;
            _hour = nowTime.Hour + (1 / 60f) * nowTime.Minute;

            SaveTime();
        }

        SetHands();

        _startTrigger.ToggleInteractable(true);
        _endTrigger.ToggleInteractable(false);
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            GlobalSave gsave = GlobalSaveHandler.GetSave();

            gsave.EternityHour = 0f;
            gsave.EternityMinute = 0f;

            GlobalSaveHandler.RewriteSave(gsave);
        }
        #endif

        if (!_span)
            return;

        _minute += _clockSpeed * Time.deltaTime;
        _hour += _clockSpeed / 60f * Time.deltaTime;

        if (_minute > 60f)
            _minute = 0f;

        if (_hour > 24f)
            _hour = 0f;

        SetHands();
    }

    private void SetHands()
    {
        _hourHand.transform.rotation = Quaternion.Euler(0, 0, -360f * ((_hour % 12) / 12f) + ((_minute / 60f / 12f)));
        _minuteHand.transform.rotation = Quaternion.Euler(0, 0, -360f * (_minute / 60f));

        _daytimeIndicator.sprite = _hour > 12f ? _daySprite : _nightSprite;

        //Debug.Log($"Hour: {_hour} Minute: {_minute}");
    }

    public void StartSpan()
    {
        _span = true;

        GlobalSave gsave = GlobalSaveHandler.GetSave();

        gsave.EternityParse = true;

        GlobalSaveHandler.RewriteSave(gsave);

        _startTrigger.ToggleInteractable(false);
        _endTrigger.ToggleInteractable(true);
    }

    public void EndSpan()
    {
        _span = false;
        SaveTime();

        _endTrigger.ToggleInteractable(false);
    }

    private void SaveTime()
    {
        GlobalSave gsave = GlobalSaveHandler.GetSave();

        gsave.EternityHour = _hour;
        gsave.EternityMinute = _minute;
        gsave.EternityParse = false;

        GlobalSaveHandler.RewriteSave(gsave);

        Debug.Log($"<color=magenta>Hour: {_hour} Minute: {_minute}</color>");

        for (int i = 0; i < _eternityConditions.Length; i++)
        {
            _eternityConditions[i].CheckTime(_hour, _minute);
        }
    }

    [System.Serializable]
    private struct EternityClockCondition
    {
        public int writtenUnlockCode;
        public bool toggle_1_0;
        public float targetHour;
        public float targetMinute;
        public float errorRate;
        public string lobbyDialogue;

        public void CheckTime(float hour, float minute)
        {
            bool timing = Mathf.Abs((Mathf.Floor(targetHour) * 60f + targetMinute) - (Mathf.Floor(hour) * 60f + minute)) <= errorRate;
            print($"{Mathf.Floor(targetHour)} * 60 + {targetMinute} - {Mathf.Floor(hour)} * 60 + {minute} = {Mathf.Abs((Mathf.Floor(targetHour) * 60f + targetMinute) - (Mathf.Floor(hour) * 60f + minute))}");

            if (toggle_1_0)
            {
                Unlocks.RewriteUnlockProgress(writtenUnlockCode, timing ? 1 : 0);
            } else
            {
                if (timing)
                    Unlocks.NewUnlock(writtenUnlockCode);
            }

            if (!string.IsNullOrEmpty(lobbyDialogue) && timing)
            {
                GlobalSaveHandler.GetSave().LobbyDialogue = lobbyDialogue;
            }
        }
    }
}
