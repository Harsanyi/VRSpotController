using UnityEngine;

public class Request
{
    public override string ToString()
    {
        return JsonUtility.ToJson(this);
    }
}

public class HelloRequest : Request
{
    public string name = string.Empty;

    public HelloRequest(string name)
    {
        this.name = name;
    }
}

public class MoveRequest : Request
{
    public Vector2 move;
    public float yaw;

    public MoveRequest(Vector2 move, float yaw)
    {
        this.move = move;
        this.yaw = yaw;
    }
}

public class ArmRequest : Request
{
    public float posX;
    public float posY;
    public float posZ;
    
    public float rotX;
    public float rotY;
    public float rotZ;
    public float rotW;

    public ArmRequest(float posX, float posY, float posZ, float rotX, float rotY, float rotZ, float rotW)
    {
        this.posX = posX;
        this.posY = posY;
        this.posZ = posZ;
        this.rotX = rotX;
        this.rotY = rotY;
        this.rotZ = rotZ;
        this.rotW = rotW;
    }
}

public class PitchRequest : Request
{
    public float pitch;

    public PitchRequest(float pitch)
    {
        this.pitch = pitch;
    }
}

public class GripRequest : Request
{
    public float open;

    public GripRequest(float open)
    {
        this.open = open;
    }
}

public class StandRequest : Request
{
    public bool stand = false;

    public StandRequest(bool stand)
    {
        this.stand = stand;
    }
}

public class PowerOnRequest : Request
{
    public bool powerOn = false;

    public PowerOnRequest(bool powerOn)
    {
        this.powerOn = powerOn;
    }
}

public class ImageRequest : Request
{
    public string source;

    public ImageRequest(string source)
    {
        this.source = source;
    }
}
