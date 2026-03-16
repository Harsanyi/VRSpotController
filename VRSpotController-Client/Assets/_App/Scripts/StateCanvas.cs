using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StateCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI powerValue;
    [SerializeField] private TextMeshProUGUI batteryValue;
    [SerializeField] private TextMeshProUGUI standingValue;

    static Dictionary<SpotState.Power, Color> powerColors = new()
    {
        { SpotState.Power.UNKNOWN , Color.grey},
        { SpotState.Power.ERROR , Color.red},
        { SpotState.Power.OFF , Color.blue},
        { SpotState.Power.ON , Color.green},
        { SpotState.Power.POWERING_ON , Color.yellow},
        { SpotState.Power.POWERING_OFF , Color.yellow}
    }; 
    
    private void Update()
    {
        var state = Spot.instance.state;

        batteryValue.text = ((int)(state.battery * 100)).ToString().Color(Color.Lerp(Color.red, Color.green, state.battery));
        powerValue.text = state.power.ToString().Color(powerColors[state.power]);
    }
}
