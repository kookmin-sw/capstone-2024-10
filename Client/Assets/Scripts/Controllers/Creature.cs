using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering;


public class Creature : NetworkBehaviour
{
    public NetworkObject NetworkObject { get; protected set; }
    public Transform Transform { get; protected set; }
    public CircleCollider2D Collider { get; protected set; }
    public Rigidbody2D RigidBody { get; protected set; }

    public NetworkId Id { get; set; }
    public int DataId { get; protected set; }
    public Define.CreatureType CreatureType { get; protected set; }
    public Data.CreatureData CreatureData { get; protected set; }
    public CreatureStat CreatureStat { get; protected set; }

    private Define.CreatureState _creatureState;
    public Define.CreatureState CreatureState
    {
        get => _creatureState;
        set
        {
            if (_creatureState != value)
            {
                _creatureState = value;
                UpdateAnimation();
            }
        }
    }
    public Vector3 Direction { get; protected set; }

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        Transform = gameObject.GetOrAddComponent<Transform>();
        Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
        RigidBody = gameObject.GetOrAddComponent<Rigidbody2D>();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public virtual void SetInfo(int templateID)
    {
        Transform.position = Vector3.zero;
        Id = NetworkObject.Id;
        DataId = templateID;

        if (CreatureType == Define.CreatureType.Crew)
        {
            Managers.ObjectMng.Crews[NetworkObject.Id] = this as Crew;
            CreatureData = Managers.DataMng.CrewDataDict[templateID];
        }
        else
        {
            Managers.ObjectMng.Aliens[NetworkObject.Id] = this as Alien;
            CreatureData = Managers.DataMng.AlienDataDict[templateID];
        }

        gameObject.name = $"{CreatureData.DataId}_{CreatureData.Name}";

        CreatureState = Define.CreatureState.Idle;

        StartCoroutine("CoUpdateAI");
    }

    public override void FixedUpdateNetwork()
    {
        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                UpdateIdle();
                break;
            case Define.CreatureState.Move:
                UpdateMove();
                break;
            case Define.CreatureState.Skill:
                UpdateSkill();
                break;
            case Define.CreatureState.Dead:
                UpdateDead();
                break;
        }
    }

    #region Animation
    protected void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                PlayAnimation();
                break;
            case Define.CreatureState.Move:
                PlayAnimation();
                break;
            case Define.CreatureState.Skill:
                PlayAnimation();
                break;
            case Define.CreatureState.Dead:
                PlayAnimation();
                break;
        }
    }

    public void PlayAnimation()
    {

    }
    #endregion

    #region Update

    protected virtual void UpdateIdle()
    {
    }

    protected virtual void UpdateMove()
    {
    }

    protected virtual void UpdateSkill()
    {
    }

    protected virtual void UpdateUseItem()
    {

    }

    protected virtual void UpdateDead()
    {
    }
    #endregion

    #region Event
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void OnDamaged(int damage)
    {
        CreatureStat.OnDamage(damage);

        if (CreatureStat.Hp <= 0)
        {
            OnDead();
            return;
        }

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void OnDead()
    {
        CreatureState = Define.CreatureState.Dead;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void OnMove(Vector3 vector)
    {
        Direction = vector.normalized;
        CreatureState = Define.CreatureState.Move;
    }
    #endregion
}
