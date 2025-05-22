using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClockTimeSetter : MonoBehaviour
{
    public Transform hourHand;
    public Transform minuteHand;
    public Transform secondHand;

    void Update()
    {
        DateTime currentTime = DateTime.Now;

        float hour = currentTime.Hour % 12;
        float minute = currentTime.Minute;
        float second = currentTime.Second;

        // Degrees per unit of time
        float hourAngle = (hour + minute / 60f) * 30f;      // 30° per hour
        float minuteAngle = minute * 6f;                    // 6° per minute
        float secondAngle = second * 6f;                    // 6° per second

        // Rotate on X axis (clockwise)
        hourHand.localRotation = Quaternion.Euler(hourAngle + 90, 0, -90);
        minuteHand.localRotation = Quaternion.Euler(minuteAngle + 90, 0, -90);
        secondHand.localRotation = Quaternion.Euler(secondAngle + 90, 0, -90);
    }
}
