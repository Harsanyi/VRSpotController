using UnityEngine;

[System.Serializable]
public class SpotState
{
    public int poseState;
    public int powerState;
    public float battery;

    public override string ToString()
    {
        return $"IsStanding: {poseState}, IsPowered: {powerState}, battery: {battery}";
    }
}
