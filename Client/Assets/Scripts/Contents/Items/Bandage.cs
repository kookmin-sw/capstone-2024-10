using UnityEngine;

public class Bandage : BaseItem
{
    public override void SetInfo(int templateId, Crew owner)
    {
        base.SetInfo(templateId, owner);

        ItemGameObject.transform.localPosition = new Vector3(-0.1f, 0.03f, -0.04f);
        ItemGameObject.transform.localEulerAngles = new Vector3(2f, 92f, 0f);
        ItemGameObject.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        ItemGameObject.SetActive(false);
    }
    public override bool CheckAndUseItem()
    {
        if (Owner.CrewStat.Hp >= Owner.CrewStat.MaxHp)
            return false;

        UseItem();
        return true;
    }

    protected override void UseItem()
    {
        Owner.CrewStat.ChangeHp((int)ItemData.Value);
    }
}
