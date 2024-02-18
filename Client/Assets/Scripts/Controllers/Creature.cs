using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;


public class Creature : NetworkBehaviour
{
    public CircleCollider2D Collider { get; private set; }
    public Rigidbody2D RigidBody { get; private set; }

    public ulong Id { get; set; }
    public int DataId { get; protected set; }
    public ECreatureType ECreatureType { get; protected set; }
    public Data.CreatureData CreatureData { get; protected set; }
    public CreatureStat CreatureStat { get; protected set; }

    private ECreatureState _creatureState;
    public virtual ECreatureState CreatureState
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

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        Collider = gameObject.GetOrAddComponent<CircleCollider2D>();
        RigidBody = GetComponent<Rigidbody2D>();
    }

    public virtual void SetInfo(int templateID)
    {
        DataId = templateID;

        if (ECreatureType == ECreatureType.Hero)
            CreatureData = Managers.DataMng.CrewDataDict[templateID];
        else
            CreatureData = Managers.DataMng.AlienDataDict[templateID];

        gameObject.name = $"{CreatureData.DataId}_{CreatureData.Name}";

        //Collider.offset = new Vector2(CreatureData.ColliderOffsetX, CreatureData.ColliderOffstY);
        //Collider.radius = CreatureData.ColliderRadius;

        //RigidBody.mass = CreatureData.Mass;

        CreatureState = ECreatureState.Idle;
    }

    #region Animation
    protected void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case ECreatureState.Idle:
                PlayAnimation();
                break;
            case ECreatureState.Move:
                PlayAnimation();
                break;
            case ECreatureState.Skill:
                PlayAnimation();
                break;
            case ECreatureState.Dead:
                PlayAnimation();
                break;
        }
    }

    public void PlayAnimation()
    {

    }
    #endregion

    #region Update

    public float UpdateAITick { get; protected set; } = 0.0f;

    protected IEnumerator CoUpdateAI()
    {
        while (true)
        {
            switch (CreatureState)
            {
                case ECreatureState.Idle:
                    UpdateIdle();
                    break;
                case ECreatureState.Move:
                    UpdateMove();
                    break;
                case ECreatureState.Skill:
                    UpdateSkill();
                    break;
                case ECreatureState.Dead:
                    UpdateDead();
                    break;
            }

            if (UpdateAITick > 0)
                yield return new WaitForSeconds(UpdateAITick);
            else
                yield return null;
        }
    }

    protected virtual void UpdateIdle()
    {
    }

    protected virtual void UpdateMove()
    {
    }

    protected virtual void UpdateSkill()
    {
    }

    protected virtual void UpdateDead()
    {
    }

    #endregion

    #region Battle
    public void OnDamaged(int damage)
    {
        CreatureStat.OnDamage(damage);

        if (CreatureStat.Hp <= 0)
        {
            OnDead();
            return;
        }

    }

    public void OnDead()
    {
        CreatureState = ECreatureState.Dead;
    }
    #endregion
}
