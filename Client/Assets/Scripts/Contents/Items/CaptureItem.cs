using System.Collections;
using UnityEngine;
using System.IO;

public class CaptureItem : MonoBehaviour
{
    public RenderTexture rt;
    public int idx;

    private void Start()
    {
    }

    public void Create()
    {
        StartCoroutine(CaptureImage());
    }

    IEnumerator CaptureImage()
    {
        yield return null;

        Texture2D texture = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false, true);
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);

        yield return null;

        string path = $"Assets/Resources/Images";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        File.WriteAllBytes($"{path}/{++idx}.png", texture.EncodeToPNG());

        yield return null;
    }
}
