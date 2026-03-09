using UnityEngine;

public class Request
{
    public virtual string ToString()
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
