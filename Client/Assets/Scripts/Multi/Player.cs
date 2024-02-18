using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class Player : NetworkBehaviour
{
    [Networked] public NetworkString<_32> PlayerName { get => default; set { } }
    public Action OnPlayerNameUpdate { get; set; }

    private IEnumerator Start()
    {
        if (HasStateAuthority)
        {
            PlayerName = FusionConnection.Instance.PlayerName;
        }

        yield return new WaitUntil(() => isActiveAndEnabled);

        Managers.UIMng.MakeWorldSpaceUI<UI_NameTag>(transform);

        yield return new WaitUntil(() => PlayerName.Value != null);

        OnPlayerNameUpdate.Invoke();
    }

    void Update()
    {
        
    }
}
