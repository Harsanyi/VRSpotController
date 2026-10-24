using UnityEngine;

public class AppManager : MonoBehaviour
{
    public static bool IsDevMode()
    {
        #if UNITY_EDITOR
        return true;
        #else
        return Debug.isDebugBuild;
        #endif
    }
}
