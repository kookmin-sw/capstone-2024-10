using DG.Tweening;
using Fusion;
using UnityEngine;

public class FlashBang : BaseItem
{
    public override void SetInfo(int templateId, Crew owner)
    {
        base.SetInfo(templateId, owner);

        ItemGameObject.transform.localPosition = new Vector3(0.02f, 0.01f, -0.043f);
        ItemGameObject.transform.localEulerAngles = new Vector3(-9f, 7f, 83f);
        ItemGameObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);

        ItemGameObject.SetActive(false);
    }
    public override bool CheckAndUseItem()
    {
        UseItem();
        return true;
    }

    protected override void UseItem()
    {
        Owner.CreatureState = Define.CreatureState.Use;
        Owner.CreaturePose = Define.CreaturePose.Stand;

        Owner.CrewAnimController.PlayAnim(Define.CrewActionType.Throw);

        DOVirtual.DelayedCall(0.5f, () =>
        {
            NetworkObject no = Managers.ObjectMng.SpawnItemObject(Define.ITEM_FLASHBANG_ID, Owner.CreatureCamera.Transform.position, false);
            no.GetComponent<BoxCollider>().enabled = false;
            no.GetComponent<Rigidbody>().AddForce(Owner.CreatureCamera.Transform.forward * 500f);

            DOVirtual.DelayedCall(1.5f, () =>
            {
                no.GetComponent<FlashBangObject>().Rpc_PlaySound();
                Explode(no.transform.position);
            });
        });

        Owner.ReturnToIdle(1f);
    }

    protected void Explode(Vector3 attackPosition)
    {
        Collider[] hitColliders = new Collider[4];

        if (Physics.OverlapSphereNonAlloc(attackPosition, 6f, hitColliders,
                LayerMask.GetMask("Crew", "Alien")) > 0)
        {
            foreach (var hitCollider in hitColliders)
                if (hitCollider.gameObject.TryGetComponent(out Creature creature))
                    creature.Rpc_OnBlind(ItemData.Value, 3f);
        }
    }
}
