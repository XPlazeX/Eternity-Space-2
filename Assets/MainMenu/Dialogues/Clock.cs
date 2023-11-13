using UnityEngine;
using System;

public class Clock : MonoBehaviour
{
    [SerializeField] private float _hourOffset;
    [SerializeField] private Transform _secondHand;
    [SerializeField] private Transform _minuteHand;
    [SerializeField] private Transform _hourHand;

    // Update is called once per frame
    void FixedUpdate()
    {
        var date = DateTime.Now.AddHours(_hourOffset);

        _hourHand.transform.rotation = Quaternion.Euler(0, 0, -360 * ((date.Hour % 12) / 12f));
        _minuteHand.transform.rotation = Quaternion.Euler(0, 0, -360 * (date.Minute / 60f));
        _secondHand.transform.rotation = Quaternion.Euler(0, 0, -360 * (date.Second / 60f));
    }
}
