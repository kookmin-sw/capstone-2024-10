using System.Text.RegularExpressions;
using UnityEngine;

public class Util : MonoBehaviour
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; ++i)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (component.name == name)
                {
                    if (string.IsNullOrEmpty(name) || component.name == name)
                        return component;
                }
            }
        }

        return null;
    }

    // 대문자 앞에 띄어쓰기 추가 (첫 글자 제외)
    public static string AddSpaceInText(string text)
    {
        // 언더바 제거
        text = text.Replace("_", "");

        // 대문자 앞에 띄어쓰기 추가 (첫 글자 제외)
        text = Regex.Replace(text, "(?<!^)([A-Z])", " $1");

        text = text.Replace("F1", "1F");
        text = text.Replace("F2", "2F");

        return text;
    }

    public static void ClearAllUI()
    {
        if (Managers.UIMng.SceneUI != null)
        {
            Destroy(Managers.UIMng.SceneUI.gameObject);
        }

        if (Managers.UIMng.PanelUI is UI_Loading)
        {
            Destroy(Managers.UIMng.PanelUI.gameObject);
            Managers.UIMng.PanelUI = null;
        }

        Managers.UIMng.Clear();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void ClearUIAndSound()
    {
        ClearAllUI();
        Managers.SoundMng.Stop(Define.SoundType.Bgm);
        Managers.SoundMng.Stop(Define.SoundType.Environment);
        Managers.SoundMng.Stop(Define.SoundType.Effect);
        Managers.SoundMng.Stop(Define.SoundType.Facility);
    }
}
