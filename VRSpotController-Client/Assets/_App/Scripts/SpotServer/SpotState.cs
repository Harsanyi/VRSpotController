using UnityEngine;

[System.Serializable]
public class SpotState
{
    public bool IsStanding;
    public bool IsPowered;
    public float battery;

    public override string ToString()
    {
        return $"IsStanding: {IsStanding}, IsPowered: {IsPowered}, battery: {battery}";
    }
}
