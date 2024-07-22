using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuideController : UI_Base
{
    private UI_LobbyController _controller;
    private Dictionary<GuideNames, UI_Guide> _guides = new(); // 모든 가이드 목록
    private UI_Guide _currentGuide;
    private int _currentPage = 0;

    enum GuideNames
    {
        CrewGuide,
        AlienGuide,
        Map,
    }

    enum Buttons
    {
        Btn_Return,
        Btn_CrewGuide,
        Btn_AlienGuide,
        Btn_Map,
        Btn_NextPage,
        Btn_PrevPage,
    }

    enum Texts
    {
        CurrentPage,
    }

    // 가이드 1개를 나타내는 클래스
    private class UI_Guide : UI_Base
    {
        public List<GameObject> Pages { get; set; } = new();

        enum Manual_GameObjects
        {
            Pages,
        }

        public new void Init()
        {
            Bind<GameObject>(typeof(Manual_GameObjects));

            // 가이드 페이지 등록
            for (int i = 0; i < GetObject(Manual_GameObjects.Pages).transform.childCount; i++)
            {
                Pages.Add(GetObject(Manual_GameObjects.Pages).transform.GetChild(i).gameObject);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            SetPage(0);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetPage(int pageNum)
        {
            foreach (var page in Pages)
            {
                page.SetActive(false);
            }
            Pages[pageNum].SetActive(true);
        }
    }

    public override bool Init()
    {
        if (!base.Init())
            return false;

        Bind<GameObject>(typeof(GuideNames));
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));

        _controller = FindObjectOfType<UI_LobbyController>();
        _guides.Add(GuideNames.CrewGuide, GetObject(GuideNames.CrewGuide).GetOrAddComponent<UI_Guide>());
        _guides.Add(GuideNames.AlienGuide, GetObject(GuideNames.AlienGuide).GetOrAddComponent<UI_Guide>());
        _guides.Add(GuideNames.Map, GetObject(GuideNames.Map).GetOrAddComponent<UI_Guide>());

        GetButton(Buttons.Btn_Return).gameObject.BindEvent((e) => { _controller.Position1(); }, Define.UIEvent.Click);
        GetButton(Buttons.Btn_CrewGuide).onClick.AddListener(()=>ShowGuide(GuideNames.CrewGuide));
        GetButton(Buttons.Btn_AlienGuide).onClick.AddListener(() => ShowGuide(GuideNames.AlienGuide));
        GetButton(Buttons.Btn_Map).onClick.AddListener(() => ShowGuide(GuideNames.Map));
        GetButton(Buttons.Btn_NextPage).onClick.AddListener(NextPage);
        GetButton(Buttons.Btn_PrevPage).onClick.AddListener(PrevPage);

        foreach (var guide in _guides.Values)
        {
            guide.Init();
            guide.Hide();
        }

        _currentGuide = _guides[GuideNames.CrewGuide];
        ShowGuide(GuideNames.CrewGuide);

        return true;
    }

    private void ShowGuide(GuideNames guideName)
    {
        var guide = _guides[guideName];

        _currentGuide.Hide();
        guide.Show();
        _currentGuide = guide;
        _currentPage = 0;
        UpdateCurrentPageText();
    }

    private void NextPage()
    {
        if (_currentPage + 1 >= _currentGuide.Pages.Count)
            return;

        _currentPage++;
        _currentGuide.SetPage(_currentPage);
        UpdateCurrentPageText();
    }

    private void PrevPage()
    {
        if (_currentPage - 1 < 0)
            return;

        _currentPage--;
        _currentGuide.SetPage(_currentPage);
        UpdateCurrentPageText();
    }

    private void UpdateCurrentPageText()
    {
        GetText(Texts.CurrentPage).text = $"Page {_currentPage + 1}/{_currentGuide.Pages.Count}";
    }

}
