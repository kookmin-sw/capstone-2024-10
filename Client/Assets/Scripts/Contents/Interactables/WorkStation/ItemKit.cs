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

        Description = "Open";
        CrewActionType = Define.CrewActionType.OpenItemKit;
        CanRememberWork = false;
        IsCompleted = false;

        TotalWorkAmount = 15f;
    }

    public override bool CheckInteractable(Creature creature)
    {
        creature.IngameUI.ErrorTextUI.Hide();

        if (creature is not Crew crew)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            return false;
        }

        if (creature.CreatureState == Define.CreatureState.Interact)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            return false;
        }

        if (IsCompleted)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    protected override void WorkComplete()
    {
        NetworkObject no = Managers.ObjectMng.SpawnItemObject(_itemId, Transform.position + Transform.up * 0.1f);
        // no.transform.SetParent(gameObject.transform);

        Rpc_WorkComplete();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted)
            return;
        IsCompleted = true;

        Cover.SetActive(false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Interactable/ItemKit");
        AudioSource.volume = 1f;
        AudioSource.loop = true;
        AudioSource.Play();
    }
}
