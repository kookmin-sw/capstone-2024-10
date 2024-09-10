using UnityEngine;

public class UI_Ingame : UI_Scene
{
    public Creature Creature { get; set; }
    public UI_WorkProgressBar WorkProgressBarUI { get; private set; }
    public UI_InteractInfo InteractInfoUI { get; private set; }
    public UI_ErrorText ErrorTextUI { get; private set; }
    public UI_CurrentSector CurrentSectorUI { get; private set; }
    public UI_ObjectName ObjectNameUI { get; private set; }
    public UI_RemainPerson RemainPersonUI { get; private set; }
    public UI_Notification NotificationUI { get; private set; }
    public UI_Map MapUI { get; private set; }
    public Canvas Canvas { get; protected set; }
    public Camera Camera { get; protected set; }

    protected enum SubItemUIs
    {
        UI_WorkProgressBar,
        UI_InteractInfo,
        UI_ErrorText,
        UI_CurrentSector,
        UI_ObjectName,
        UI_RemainPerson,
        UI_Notification,
        UI_Map,
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
        RemainPersonUI = Get<UI_Base>(SubItemUIs.UI_RemainPerson) as UI_RemainPerson;
        NotificationUI = Get<UI_Base>(SubItemUIs.UI_Notification) as UI_Notification;
        MapUI = Get<UI_Base>(SubItemUIs.UI_Map) as UI_Map;

        return true;
    }

    public virtual void InitAfterNetworkSpawn(Creature creature)
    {
        Creature = creature;
        CurrentSectorUI.SetSector(Creature.CurrentSector);
        MapUI.Hide();
    }

    public virtual void HideUi()
    {
        WorkProgressBarUI.Hide();
        InteractInfoUI.Hide();
        ErrorTextUI.Hide();
        CurrentSectorUI.Hide();
        ObjectNameUI.Hide();
        RemainPersonUI?.Hide();
        NotificationUI?.Hide();
    }
}
