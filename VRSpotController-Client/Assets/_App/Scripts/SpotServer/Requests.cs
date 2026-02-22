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
