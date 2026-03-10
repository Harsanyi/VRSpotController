using UnityEngine;

[System.Serializable]
public class SpotState
{
    public int poseState;
    public int powerState;
    public float battery;
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ, rotW;
    public float armPosX, armPosY, armPosZ;
    public float armRotX, armRotY, armRotZ, armRotW;
    
    public Vector3 pos => new Vector3(-posY, posZ, posX);
    public Quaternion rot => new Quaternion(rotY, -rotZ, -rotX, rotW);
    
    public Vector3 armPos => new Vector3(-armPosY, armPosZ, armPosX);
    public Quaternion armRot => new Quaternion(armRotY, -armRotZ, -armRotX, armRotW);

    public override string ToString()
    {
        return $"IsStanding: {poseState}," +
               $" IsPowered: {powerState}," +
               $" battery: {battery}," +
               $" pos:{pos.ToString()}," +
               $" rot:{rot.ToString()}" +
               $" armPos:{armPos.ToString()}," +
               $" armRot:{armRot.ToString()}";
    }
}
