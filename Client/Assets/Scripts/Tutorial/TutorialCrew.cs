using UnityEngine;
using Data;
using Fusion;
using UnityEngine.EventSystems;

public class TutorialCrew : Crew
{
    public UI_CrewTutorial CrewTutorialUI => IngameUI as UI_CrewTutorial;

    protected override void Init()
    {
        base.Init();
    }
}
