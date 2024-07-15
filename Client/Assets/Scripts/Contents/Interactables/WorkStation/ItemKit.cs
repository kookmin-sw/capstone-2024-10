using Fusion;
using UnityEngine;

public class ItemKit : BaseWorkStation
{
    [Header("Item")]
    [SerializeField] private int _itemId;

    public Transform Transform { get; protected set; }

    public GameObject Cover { get; protected set; }

    protected override void Init()
    {
        base.Init();

        Cover = Util.FindChild(gameObject, "Cover", true);
        Transform = gameObject.GetComponent<Transform>();
        AudioSource = gameObject.GetComponent<AudioSource>();

        Description = "Open ItemKit";
        CrewActionType = Define.CrewActionType.OpenItemKit;
        CanRememberWork = false;
        IsCompleted = false;

        TotalWorkAmount = 6f;
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

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
        NetworkObject no = Managers.ObjectMng.SpawnItemObject(_itemId, Transform.position + Transform.up * 0.1f, true);

        Rpc_WorkComplete();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted)
            return;
        IsCompleted = true;

        Cover.SetActive(false);
        gameObject.SetLayerRecursive(LayerMask.NameToLayer("MapObject"));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/ItemKit", 0.7f, 0.7f, isLoop: true);
    }
}
