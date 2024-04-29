using UnityEngine;

public class USBKey : BaseItem
{
    public override void SetInfo(int templateId, Crew owner)
    {
        base.SetInfo(templateId, owner);

        ItemGameObject.transform.localPosition = new Vector3(-0.052f, 0.016f, -0.02f);
        ItemGameObject.transform.localEulerAngles = new Vector3(4f, -60f, 15f);
        ItemGameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
        ItemGameObject.SetActive(false);
    }

    public override bool CheckAndUseItem()
    {
        return false;
    }
}
