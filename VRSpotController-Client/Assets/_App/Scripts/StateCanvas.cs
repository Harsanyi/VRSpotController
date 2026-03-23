using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StateCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI powerValue;
    [SerializeField] private TextMeshProUGUI batteryValue;
    [SerializeField] private TextMeshProUGUI standingValue;
    [SerializeField] private TextMeshProUGUI positionValue;
    [SerializeField] private TextMeshProUGUI rotationValue;

    static Dictionary<SpotState.Power, Color> powerColors = new()
    {
        { SpotState.Power.UNKNOWN , Color.grey},
        { SpotState.Power.ERROR , Color.red},
        { SpotState.Power.OFF , Color.blue},
        { SpotState.Power.ON , Color.green},
        { SpotState.Power.POWERING_ON , Color.yellow},
        { SpotState.Power.POWERING_OFF , Color.yellow}
    };

    private static Dictionary<SpotState.Pose, Color> poseColors = new()
    {
        { SpotState.Pose.UNKNOWN , Color.grey},
        { SpotState.Pose.NOT_READY, Color.red},
        { SpotState.Pose.TRANSITION , Color.yellow},
        { SpotState.Pose.STANDING , Color.green},
        { SpotState.Pose.STEPPING , Color.blue},
    };
    
    private void Update()
    {
        var state = Spot.instance.state;
        if(state == null)return;
        
        batteryValue.text = state.battery.ToString().Color(Color.Lerp(Color.red, Color.green, state.battery / 100f));
        powerValue.text = state.power.ToString().Color(powerColors[state.power]);
        standingValue.text = state.pose.ToString().Color(poseColors[state.pose]);
        positionValue.text = state.pos.ToString("n2");
        rotationValue.text = state.rot.eulerAngles.ToString("n2");
    }
}