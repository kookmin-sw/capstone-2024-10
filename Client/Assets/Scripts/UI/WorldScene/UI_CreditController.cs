using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CreditController : UI_Base
{
    private UI_LobbyController _controller;
    private float scrollSpeed = 150f;
    private RectTransform textscroll;

    enum Buttons
    {
        Btn_Return,
    }
    enum Texts
    {
        Credit1,
        Credit2
    }
    enum GameObjects
    {
        CREDIT,
    }
    public override bool Init()
    {
        if (!base.Init())
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        _controller = FindObjectOfType<UI_LobbyController>();
        GetButton(Buttons.Btn_Return).gameObject.BindEvent((e) => { _controller.Position1(); }, Define.UIEvent.Click);
        textscroll = GetObject(GameObjects.CREDIT).GetComponent<RectTransform>();


        return true;
    }

    public IEnumerator ScrollCredits()
    {
        // 스크롤 시작 전 대기
        yield return new WaitForSeconds(1f);

        while (textscroll.anchoredPosition.y < 6000f)
        {
            textscroll.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
            yield return null; // 다음 프레임까지 대기
        }

    }

    public void ResetCredit()
    {
        textscroll.anchoredPosition = new Vector2(0, -900f);
    }
}
