using UnityEngine;

[System.Serializable]
public class Response
{
    public bool success = false;
    public string message = string.Empty;
}

[System.Serializable]
public class StateResponse : Response
{
    public SpotState state = null;
}
