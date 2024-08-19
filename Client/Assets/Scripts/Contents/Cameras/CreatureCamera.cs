using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CreatureCamera : MonoBehaviour
{
    public Creature Creature { get; set; }

    public Transform Transform { get; protected set; }
    public Camera Camera { get; protected set; }

    public Vector3 LastForward { get; protected set; }

    private void Awake()
    {
        Init();
    }

    protected void Init()
    {
        Transform = transform;
        Camera = GetComponent<Camera>();
    }

    public void SetInfo(Creature creature)
    {
        enabled = true;
        Creature = creature;
    }

    public void UpdateCameraAngle()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!Creature.HasStateAuthority || Creature.CreatureState == Define.CreatureState.Dead || !Creature.IsSpawned)
            return;

        try
        {
            if (Creature.CreatureState == Define.CreatureState.Damaged || Creature.CreatureState == Define.CreatureState.Interact || Creature.CreatureState == Define.CreatureState.Use)
            {
                Transform.forward = LastForward;
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to UpdateCameraAngle: " + e);
        }

        Quaternion rotation = Quaternion.Euler(Creature.XRotation, Creature.CurrentAngle, 0);

        Transform.rotation = rotation; // 카메라 회전 적용

        LastForward = Transform.forward;
    }
}
