using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO; 

public class CaptureItem : MonoBehaviour
{
    public Camera Camera;
    public RenderTexture rt;
    public Image image;
    public GameObject item;
    public int idx;

    private void Start()
    {
        Camera = Camera.main;
        idx = 0;
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

        var data = texture.EncodeToPNG();
        string name = idx.ToString();
        string extention = ".png";
        string path = $"Assets/Resources/Images/";
        idx++;
        Debug.Log(path);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        File.WriteAllBytes(path + name + extention, data);

        yield return null;
    }
}
