using UnityEngine;

public class UI_Ingame : UI_Scene
{
    public Creature Creature { get; set; }
    public UI_WorkProgressBar WorkProgressBarUI { get; private set; }
    public UI_InteractInfo InteractInfoUI { get; private set; }
    public UI_ErrorText ErrorTextUI { get; private set; }
    public UI_CurrentSector CurrentSectorUI { get; private set; }
    public UI_ObjectName ObjectNameUI { get; private set; }

    public Canvas Canvas { get; protected set; }
    public Camera Camera { get; protected set; }

    protected enum SubItemUIs
    {
        UI_WorkProgressBar,
        UI_InteractInfo,
        UI_ErrorText,
        UI_CurrentSector,
        UI_ObjectName,
    }

    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<UI_Base>(typeof(SubItemUIs));

        WorkProgressBarUI = Get<UI_Base>(SubItemUIs.UI_WorkProgressBar) as UI_WorkProgressBar;
        InteractInfoUI = Get<UI_Base>(SubItemUIs.UI_InteractInfo) as UI_InteractInfo;
        ErrorTextUI = Get<UI_Base>(SubItemUIs.UI_ErrorText) as UI_ErrorText;
        CurrentSectorUI = Get<UI_Base>(SubItemUIs.UI_CurrentSector) as UI_CurrentSector;
        ObjectNameUI = Get<UI_Base>(SubItemUIs.UI_ObjectName) as UI_ObjectName;

        return true;
    }

    public virtual void InitAfterNetworkSpawn(Creature creature)
    {
        Creature = creature;
        CurrentSectorUI.SetSector(Creature.CurrentSector);
    }

    public void EndGame()
    {
        Canvas.renderMode = RenderMode.ScreenSpaceCamera;
        Transform[] children = gameObject.GetComponentsInChildren<Transform>();

        // 자식 객체들 중에서 Camera 컴포넌트를 가진 객체 찾기
        foreach (Transform child in children)
        {
            // 자식 객체가 Camera 컴포넌트를 가지고 있는지 확인
            Camera childCamera = child.GetComponent<Camera>();
            if (childCamera != null)
            {
                Camera = childCamera;
            }
        }
        Canvas.worldCamera = Camera;
    }
}
