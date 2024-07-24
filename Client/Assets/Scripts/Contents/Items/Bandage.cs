using UnityEngine;

public class Bandage : BaseItem
{
    public override void SetInfo(int templateId, Crew owner)
    {
        base.SetInfo(templateId, owner);

        ItemGameObject.transform.localPosition = new Vector3(0.02f, 0.01f, -0.043f);
        ItemGameObject.transform.localEulerAngles = new Vector3(-9f, -83f, 83f);
        ItemGameObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        ItemGameObject.SetActive(false);

        CrewActionType = Define.CrewActionType.Bandage;
        TotalWorkAmount = 10f;
    }
    public override bool CheckAndUseItem()
    {
        if (Owner.CrewStat.Hp >= Owner.CrewStat.MaxHp)
            return false;

        UseItem();
        return true;
    }

    protected override void UseItemComplete()
    {
        Owner.CrewStat.ChangeHp((int)ItemData.Value);
    }
}
