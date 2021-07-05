using UnityEngine;

public static class Utils
{
    public static Vector3 DeserializeVector3(string serializedVector)
    {
        var parts = serializedVector.Replace("(", "").Replace(")", "").Split(',');
        return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
    }
}
