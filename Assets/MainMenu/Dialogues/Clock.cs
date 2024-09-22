using UnityEngine;
using System;

public class Clock : MonoBehaviour
{
    public enum ClockType
    {
        Original = 0,
        EternityClock = 1
    }
    [SerializeField] private ClockType _clockType;
    [SerializeField] private Transform _miliSecondHand;
    [SerializeField] private Transform _secondHand;
    [SerializeField] private HandCopier[] _secondCopiers;
    [SerializeField] private Transform _minuteHand;
    [SerializeField] private HandCopier[] _minuteCopiers;
    [SerializeField] private Transform _hourHand;
    [SerializeField] private HandCopier[] _hourCopiers;

    private float _timeScale = 1f;

    // Update is called once per frame
    private void OnEnable() 
    {
        switch (_clockType)
        {
            case ClockType.Original:
                var date = DateTime.Now;
                SetHands(date.Hour, date.Minute, date.Second);
                break;
            case ClockType.EternityClock:
                if (GlobalSaveHandler.GetSave().EternityHour == -1f)
                {
                    var date1 = DateTime.Now;
                    SetHands(date1.Hour, date1.Minute, date1.Second);
                    _timeScale = 0f;
                    break;
                }
                if (EternityClock.Parsing)
                {
                    SetHands(UnityEngine.Random.Range(0, 24), UnityEngine.Random.Range(0, 60), UnityEngine.Random.Range(0, 60));
                    _timeScale = 396f;
                } else
                {
                    SetHands(Mathf.FloorToInt(GlobalSaveHandler.GetSave().EternityHour), Mathf.FloorToInt(GlobalSaveHandler.GetSave().EternityMinute), UnityEngine.Random.Range(0, 60));
                    _timeScale = 0f;
                }
                break;
            default:
                break;
        }
    }

    void FixedUpdate()
    {
        _hourHand.transform.eulerAngles = new Vector3(0, 0f, _hourHand.transform.eulerAngles.z - 360f * (Time.deltaTime / 43200f) * _timeScale);
        _minuteHand.transform.eulerAngles = new Vector3(0, 0f, _minuteHand.transform.eulerAngles.z - 360f * (Time.deltaTime / 3600f) * _timeScale);
        _secondHand.transform.eulerAngles = new Vector3(0, 0f, _secondHand.transform.eulerAngles.z - 360f * (Time.deltaTime / 60f) * _timeScale);
        _miliSecondHand.transform.eulerAngles = new Vector3(0f, 0f, _miliSecondHand.transform.eulerAngles.z - (360f * Time.fixedDeltaTime * _timeScale));

        for (int i = 0; i < _secondCopiers.Length; i++)
        {
            _secondCopiers[i].transform.rotation = _secondCopiers[i].mirror ? Quaternion.Euler(0, 0, -_secondHand.eulerAngles.z) : _secondHand.transform.rotation;
        }
        for (int i = 0; i < _minuteCopiers.Length; i++)
        {
            _minuteCopiers[i].transform.rotation = _minuteCopiers[i].mirror ? Quaternion.Euler(0, 0, -_minuteHand.eulerAngles.z) : _minuteHand.transform.rotation;
        }
        for (int i = 0; i < _hourCopiers.Length; i++)
        {
            _hourCopiers[i].transform.rotation = _hourCopiers[i].mirror ? Quaternion.Euler(0, 0, -_hourHand.eulerAngles.z) : _hourHand.transform.rotation;
        }
    }

    private void SetHands(int hour, int minute, int seconds)
    {
        _hourHand.transform.rotation = Quaternion.Euler(0, 0, -360f * (((hour % 12) / 12f) + (((float)minute / 60f / 12f)) + ((float)seconds / 60f / 720f)));
        _minuteHand.transform.rotation = Quaternion.Euler(0, 0, -360f * ((minute / 60f) + ((float)seconds / 60f / 60f)));
        _secondHand.transform.rotation = Quaternion.Euler(0, 0, -360f * (seconds / 60f));
    }

    [System.Serializable]
    private struct HandCopier
    {
        public Transform transform;
        public bool mirror;
    }
}
