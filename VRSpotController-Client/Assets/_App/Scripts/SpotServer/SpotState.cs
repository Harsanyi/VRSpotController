using UnityEngine;

[System.Serializable]
public class SpotState
{
    public int poseState;
    public int powerState;
    public float battery;
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ, rotW;
    
    public Vector3 pos => new Vector3(posX, posY, posZ);
    public Quaternion rot => new Quaternion(rotX, rotY, rotZ, rotW);

    public override string ToString()
    {
        return $"IsStanding: {poseState}, IsPowered: {powerState}, battery: {battery}, pos:{pos.ToString()}, rot:{rot.ToString()}";
    }
}
