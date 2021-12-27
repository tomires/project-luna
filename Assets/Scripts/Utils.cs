using UnityEngine;
using System.Linq;

public static class Utils
{
    public static Vector3 DeserializeVector3(string serializedVector)
    {
        var parts = serializedVector.Replace("(", "").Replace(")", "").Split(',');
        return new Vector3(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
    }

    public static Texture2D ToTexture2D (this RenderTexture texture)
    {
        Texture2D tex = new Texture2D(texture.width, texture.height, TextureFormat.RGB24, false);
        RenderTexture.active = texture;
        tex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        tex.Apply();
        return tex;
    }

    public static string GetFilename(string path)
    {
        return path.Split('\\').Last();
    }
}
