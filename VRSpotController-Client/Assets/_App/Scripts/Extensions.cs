using UnityEngine;

public static class Extensions
{
    public static string Color(this string str, Color col)
    {
        return $"<color=#{ColorUtility.ToHtmlStringRGBA(col)}>{str}</color>";
    }
}
