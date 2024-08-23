using Fusion;
using UnityEngine;

public class ItemKit : BaseWorkStation
{
    [Header("ItemId")]
    [SerializeField] private int _itemId;

    public Transform Transform { get; protected set; }

    public ItemKitCover Cover { get; protected set; }

    protected override void Init()
    {
        base.Init();

        Transform = gameObject.GetComponent<Transform>();

        Cover = Util.FindChild(gameObject, "Cover", true).GetComponent<ItemKitCover>();
        Cover.ItemKit = this;

        Description = "Open ItemKit";
        CrewActionType = Define.CrewActionType.OpenItemKit;
        CanRememberWork = false;
        IsCompleted = false;

        TotalWorkAmount = 5f;
    }

    public override bool IsInteractable(Creature creature)
    {
        creature.IngameUI.ErrorTextUI.Hide();
        creature.IngameUI.InteractInfoUI.Hide();

        if (Worker != null) return false;

        if (creature.CreatureState == Define.CreatureState.Interact) return false;

        if (creature is not Crew)
        {
            return false;
        }

        if (IsCompleted)
        {
            return false;
        }

        if (WorkerCount > 0 && Worker == null)
        {
            creature.IngameUI.ErrorTextUI.Show("Another Crew is in Use");
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);

        return true;
    }

    protected override void WorkComplete()
    {
        NetworkObject no = Managers.ObjectMng.SpawnItemObject(_itemId, Transform.position + Transform.up * 0.1f, Quaternion.identity, true);

        Rpc_WorkComplete();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted)
            return;
        IsCompleted = true;

        Cover.gameObject.SetActive(false);
        gameObject.SetLayerRecursive(LayerMask.NameToLayer("MapObject"));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/ItemKit", 0.7f, 0.9f, isLoop: true);
    }
}
