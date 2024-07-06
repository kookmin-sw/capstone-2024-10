using Data;
using DG.Tweening;
using Fusion;
using UnityEngine;

public abstract class BaseItem
{
    public int DataId { get; protected set; }
    public ItemData ItemData { get; protected set; }

    public Crew Owner { get; protected set; }

    public GameObject ItemGameObject { get; protected set; }

    protected Define.CrewActionType CrewActionType { get; set; }

    private Tweener _useItemTweener;

    public float TotalWorkAmount { get; protected set; }

    public virtual void SetInfo(int templateId, Crew owner)
    {
        DataId = templateId;
        ItemData = Managers.DataMng.ItemDataDict[templateId];
        Owner = owner;

        string className = Managers.DataMng.ItemDataDict[templateId].Name;
        ItemGameObject = Managers.ResourceMng.Instantiate($"{Define.ITEM_PATH}/{className}", Owner.LeftHand.transform);
    }

    public abstract bool CheckAndUseItem();

    protected virtual void UseItem()
    {
        Owner.CreatureState = Define.CreatureState.Use;
        Owner.CreaturePose = Define.CreaturePose.Stand;

        Owner.CrewAnimController.PlayAnim(CrewActionType);
        Owner.CrewSoundController.PlaySound(CrewActionType);

        Owner.IngameUI.InteractInfoUI.Hide();
        Owner.IngameUI.InteractInfoUI.Show("Cancel");
        Owner.IngameUI.WorkProgressBarUI.Show("Use " + ItemData.Name, 0f, TotalWorkAmount);

        _useItemTweener = DOVirtual.Float(0, TotalWorkAmount, TotalWorkAmount, value =>
        {
            if (Owner.CreatureState != Define.CreatureState.Use)
                _useItemTweener.Kill();

            Owner.IngameUI.WorkProgressBarUI.CurrentWorkAmount = value;
        });

        _useItemTweener.onComplete -= UseItemCompleteInterrupt;
        _useItemTweener.onComplete += UseItemCompleteInterrupt;
        _useItemTweener.onKill -= UseItemKilledInterrupt;
        _useItemTweener.onKill += UseItemKilledInterrupt;
    }

    protected void UseItemCompleteInterrupt()
    {
        Owner.IngameUI.InteractInfoUI.Hide();
        Owner.IngameUI.WorkProgressBarUI.Hide();
        UseItemComplete();
        Owner.Inventory.RemoveItem();
        Owner.ReturnToIdle(0.2f);
    }

    protected void UseItemKilledInterrupt()
    {
        Owner.IngameUI.InteractInfoUI.Hide();
        Owner.IngameUI.WorkProgressBarUI.Hide();
        Owner.ReturnToIdle(0.2f);
    }

    protected virtual void UseItemComplete() {}
}
