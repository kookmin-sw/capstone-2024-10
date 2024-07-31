using DG.Tweening;
using UnityEngine;

public class Adrenaline : BaseItem
{
    public override void SetInfo(int templateId, Crew owner)
    {
        base.SetInfo(templateId, owner);

        ItemGameObject.transform.localPosition = new Vector3(-0.03f, 0f, -0.03f);
        ItemGameObject.transform.localEulerAngles = new Vector3(-6f, -70f, 88f);
        ItemGameObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        ItemGameObject.SetActive(false);
    }
    public override bool CheckAndUseItem()
    {
        if (Owner.CrewStat.Doped)
            return false;

        UseItem();
        return true;
    }

    protected override void UseItem()
    {
        Owner.CrewSoundController.PlaySound(Define.CrewActionType.Adrenaline);

        Owner.CrewStat.Doped = true;

        DOVirtual.DelayedCall(ItemData.Value, () =>
        {
            Owner.CrewStat.Doped = false;
        });

        Owner.Inventory.RemoveItem();
    }
}
