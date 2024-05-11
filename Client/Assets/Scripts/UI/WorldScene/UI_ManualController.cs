using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UI_Loading;
using UnityEngine.UI;

public class UI_ManualController : UI_Base
{
    private UI_LobbyController _controller;
    private UI_Manual _currentManual;
    private UI_Manual _crewManual;
    private UI_Manual _alienManual;
    private int _currentPage = 0;

    enum GameObjects
    {
        CrewManual,
        AlienManual,
    }

    enum Buttons
    {
        Btn_Return,
        Btn_CrewManual,
        Btn_AlienManual,
        Btn_NextPage,
        Btn_PrevPage,
    }

    enum Texts
    {
        CurrentPage,
    }


    private class UI_Manual : UI_Base
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

        _controller = FindObjectOfType<UI_LobbyController>();
        _crewManual = GetObject(GameObjects.CrewManual).GetOrAddComponent<UI_Manual>();
        _alienManual = GetObject(GameObjects.AlienManual).GetOrAddComponent<UI_Manual>();

        GetButton(Buttons.Btn_Return).gameObject.BindEvent((e) => { _controller.Position1(); }, Define.UIEvent.Click);
        GetButton(Buttons.Btn_CrewManual).onClick.AddListener(ShowCrewManual);
        GetButton(Buttons.Btn_AlienManual).onClick.AddListener(ShowAlienManual);
        GetButton(Buttons.Btn_NextPage).onClick.AddListener(NextPage);
        GetButton(Buttons.Btn_PrevPage).onClick.AddListener(PrevPage);

        _crewManual.Init();
        _alienManual.Init();

        ShowCrewManual();

        return true;
    }

    private void ShowCrewManual()
    {
        _alienManual.Hide();
        _crewManual.Show();
        _currentManual = _crewManual;
        _currentPage = 0;
        UpdateCurrentPageText();
    }

    private void ShowAlienManual()
    {
        _crewManual.Hide();
        _alienManual.Show();
        _currentManual = _alienManual;
        _currentPage = 0;
        UpdateCurrentPageText();
    }

    private void NextPage()
    {
        if (_currentPage + 1 >= _currentManual.Pages.Count)
            return;

        _currentPage++;
        _currentManual.SetPage(_currentPage);
        UpdateCurrentPageText();
    }

    private void PrevPage()
    {
        if (_currentPage - 1 < 0)
            return;

        _currentPage--;
        _currentManual.SetPage(_currentPage);
        UpdateCurrentPageText();
    }

    private void UpdateCurrentPageText()
    {
        GetText(Texts.CurrentPage).text = $"Page {_currentPage + 1}/{_currentManual.Pages.Count}";
    }

}
