using UnityEngine;
using Data;
using Fusion;
using UnityEngine.EventSystems;

public class TutorialCrew : Crew
{
    public TutorialCrewUI CrewTutorialUI => IngameUI as TutorialCrewUI;

    protected override void Init()
    {
        base.Init();


    }
}
