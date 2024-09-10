using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GuidePanel : UI_CameraPanel
{
    private UI_Guide _currentGuide;
    private UI_Guide _crewGuide;
    private UI_Guide _alienGuide;
    private UI_Guide _mapGuide;
    private int _currentPage = 0;

    enum GameObjects
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
        Btn_MapGuide,
        Btn_NextPage,
        Btn_PrevPage,
    }

    enum Texts
    {
        CurrentPage,
    }


    private class UI_Guide : UI_Base
    {
        public List<GameObject> Pages { get; set; } = new();

        enum Manual_GameObjects
        {
            Pages,
        }

        public override bool Init()
        {
            if (!base.Init())
                return false;

            Bind<GameObject>(typeof(Manual_GameObjects));

            if (Pages.Count == 0)
            {
                for (int i = 0; i < GetObject(Manual_GameObjects.Pages).transform.childCount; i++)
                {
                    Pages.Add(GetObject(Manual_GameObjects.Pages).transform.GetChild(i).gameObject);
                }
            }

            return true;
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

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));

        _crewGuide = GetObject(GameObjects.CrewGuide).GetOrAddComponent<UI_Guide>();
        _alienGuide = GetObject(GameObjects.AlienGuide).GetOrAddComponent<UI_Guide>();
        _mapGuide = GetObject(GameObjects.Map).GetOrAddComponent<UI_Guide>();

        GetButton(Buttons.Btn_Return).onClick.AddListener(() =>
        {
            ClosePanelUI();
        });
        GetButton(Buttons.Btn_CrewGuide).onClick.AddListener(ShowCrewManual);
        GetButton(Buttons.Btn_AlienGuide).onClick.AddListener(ShowAlienManual);
        GetButton(Buttons.Btn_MapGuide).onClick.AddListener(ShowMapManual);
        GetButton(Buttons.Btn_NextPage).onClick.AddListener(NextPage);
        GetButton(Buttons.Btn_PrevPage).onClick.AddListener(PrevPage);

        _crewGuide.Init();
        _alienGuide.Init();
        _mapGuide.Init();

        ShowCrewManual();

        return true;
    }

    private void ShowCrewManual()
    {
        _alienGuide.Hide();
        _crewGuide.Show();
        _mapGuide.Hide();
        _currentGuide = _crewGuide;
        _currentPage = 0;
        UpdateCurrentPageText();
    }

    private void ShowAlienManual()
    {
        _crewGuide.Hide();
        _mapGuide.Hide();
        _alienGuide.Show();
        _currentGuide = _alienGuide;
        _currentPage = 0;
        UpdateCurrentPageText();
    }

    private void ShowMapManual()
    {
        _crewGuide.Hide();
        _alienGuide.Hide();
        _mapGuide.Show();
        _currentGuide = _mapGuide;
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
