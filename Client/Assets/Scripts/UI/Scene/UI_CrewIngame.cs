using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_CrewIngame : UI_Scene
{
    public Crew Crew { get; set; }
    enum Buttons
    {

    }

    enum Images
    {

    }

    enum Texts
    {
        
    }

    enum SubItemUIs
    {
       UI_WorkProgressBar,
       UI_CrewHP,
       UI_CrewStamina
    }


    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<UI_Base>(typeof(SubItemUIs));

        StartCoroutine(AssignCrew());
        return true;
    }

    // Crew를 Assign하는 방식은 추후 리팩토링 필요
    // Crew 쪽에서 Assign 하는 게 나을 수 있음
    private IEnumerator AssignCrew()
    {
        while (Managers.ObjectMng.MyCreature == null)
        {
            yield return null;
        }
        Crew = Managers.ObjectMng.MyCreature as Crew;

        (Get<UI_Base>((int)SubItemUIs.UI_CrewHP) as UI_CrewHP).Crew = Crew;
        (Get<UI_Base>((int)SubItemUIs.UI_CrewStamina) as UI_CrewStamina).Crew = Crew;
    }

    public UI_WorkProgressBar ShowWorkProgressBar(string workDescription, float requiredWorkAmount)
    {
        UI_WorkProgressBar bar = Get<UI_Base>((int)SubItemUIs.UI_WorkProgressBar) as UI_WorkProgressBar;
        bar.gameObject.SetActive(true);
        bar.SetInfo(workDescription, requiredWorkAmount);
        return bar;
    }
}
