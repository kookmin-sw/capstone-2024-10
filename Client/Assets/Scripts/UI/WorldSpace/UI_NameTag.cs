using UnityEngine;
using TMPro;

public class UI_NameTag : UI_Base
{
    #region UI 목록들
    public enum Buttons
    {
    }

    public enum Images
    {
    }

    public enum Texts
    {
        Nickname,
    }

    public enum GameObjects
    {
    }
    #endregion

    public TMP_Text Nickname { get; private set; }
    public Player Player { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TMP_Text>(typeof(Texts));

        Nickname = GetText((int)Texts.Nickname);

        transform.localPosition = new Vector3(0, 2.0f, 0);
        transform.localRotation = Quaternion.Euler(0, 0, 0);

        Player = transform.parent.GetComponent<Player>();
        Player.OnPlayerNameUpdate += (name) => Nickname.text = name;

        return true;
    }

    private void Update()
    {
        if (Camera.main == null)
            return;

        Nickname.transform.LookAt(Camera.main.transform);
    }
}
