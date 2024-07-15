using UnityEngine;
using Data;
using Fusion;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class TutorialCrew : Crew
{
    public UI_CrewTutorial CrewTutorialUI => IngameUI as UI_CrewTutorial;

    protected override void Init()
    {
        base.Init();
    }

    public override void OnWin()
    {
        CrewTutorialUI.TutorialPlanUI.SetActive(false);
        base.OnWin();
    }
}
