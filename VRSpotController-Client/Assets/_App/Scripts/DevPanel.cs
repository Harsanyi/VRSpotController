using UnityEngine;

public class DevPanel : MonoBehaviour
{
    void Start()
    {
        if (!AppManager.IsDevMode())
        {
            Destroy(this.gameObject);
        }
    }

    public void DeleteConfig()
    {
        Config.Delete();
    }

    public void ServerCheck()
    {
        new SpotServer().IsAvailable();
    }

    public void SayHello()
    {
        new SpotServer().SayHello();
    }
}
