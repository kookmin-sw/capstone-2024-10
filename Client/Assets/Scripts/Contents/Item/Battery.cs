using UnityEngine;

public class Battery : BaseItem
{
    public override void SetInfo(int templateId, Crew owner)
    {
        base.SetInfo(templateId, owner);

        ItemGameObject = Managers.ResourceMng.Instantiate("Items/Battery", Owner.LeftHand.transform);
        ItemGameObject.transform.localPosition = new Vector3(-0.1f, -0.01f, -0.05f);
        ItemGameObject.transform.localEulerAngles = new Vector3(68f, 144f, 55f);
        ItemGameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        ItemGameObject.SetActive(false);
    }

    public override bool CheckAndUseItem(Crew crew)
    {
        return false;
    }
}
