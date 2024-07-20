public class TutorialCrew : Crew
{
    public UI_CrewTutorial CrewTutorialUI => IngameUI as UI_CrewTutorial;

    public override void OnWin()
    {
        CrewTutorialUI.TutorialPlanUI.SetActive(false);
        base.OnWin();
    }
}
