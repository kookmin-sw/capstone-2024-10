using UnityEngine;

public static class Define
{
    #region Type

    public enum CreatureType
    {
        None,
        Crew,
        Alien,
    }

    public enum CrewActionType
    {
        None,
        Damaged,
        Dead,
        KeypadUse,
        OpenItemKit,
        OpenDoor,
        ChargeBattery,
        Insert,
        FlashBang,
        Bandage,
        Antipsychotic,
        Morphine,
    }

    public enum AlienActionType
    {
        None,
        Damaged,
        CrashDoor,
        Hit,
        BasicAttack,
        ReadyRoar,
        Roar,
        ReadyCursedHowl,
        CursedHowl,
        ReadyLeapAttack,
        LeapAttack,
    }

    public enum SectorName
    {
        None,
        F1_Corridor_A,
        F1_Corridor_B,
        F1_Corridor_C,
        F1_Corridor_D,
        F1_Corridor_E,
        F2_Corridor_A,
        F2_Corridor_B,
        F2_Corridor_C,
        F2_Corridor_D,
        F2_Corridor_E,
        Storage,
        ContainmentRoom,
        ContainmentControlRoom,
        ObservationRoom,
        SampleRoom,
        PowerRoom,
        CargoControlRoom,
        CentralControlRoom,
        Oratory,
        StaffAccommodation,
        Cafeteria,
        VisitingRoom,
        DirectorOffice,
        DiningFacility,
    }

    public enum SceneType
    {
        UnknownScene,
        ReadyScene,
        LobbyScene,
        GameScene,
        Map_JSJ,
    }

    public enum SoundType
    {
        Bgm,
        Environment,
        Effect,
        MaxCount,
    }

    public enum VolumeType
    {
        BgmVolume,
        EnvVolume,
        EffVolume,
        MasterVolume,
    }

    #endregion

    #region State

    public enum PlayerState
    {
        None,
        Ready,
    }

    public enum CreatureState
    {
        Idle,
        Move,
        Interact,
        Use,
        Damaged,
        Dead,
    }

    public enum CreaturePose
    {
        Stand,
        Sit,
        Run
    }

    #endregion

    #region Event

    public enum UIEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
        PointerEnter,
        PointerExit,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
        Hover,
    }

    #endregion

    #region NonContent

    public enum Layer
    {
        Ground = 6,
        Block = 7,
        Monster = 8,
        Player = 9,
    }

    public enum CameraMode
    {
        QuarterView,
    }

    #endregion

    #region DataId

    public const int CREW_CREWA_ID = 101000;

    public const int ALIEN_STALKER_ID = 102000;

    public const int ITEM_BATTERY_ID = 201000;
    public const int ITEM_USBKEY_ID = 201001;
    public const int ITEM_CARDKEY_ID = 201002;
    public const int ITEM_BANDAGE_ID = 202000;
    public const int ITEM_MORPHINE_ID = 202001;
    public const int ITEM_ANTIPSYCHOTIC_ID = 202002;
    public const int ITEM_FLASHBANG_ID = 203000;

    public const int SKILL_BASIC_ATTACK_ID = 301000;
    public const int SKILL_ROAR_ID = 301001;
    public const int SKILL_CURSED_HOWL_ID = 301002;
    public const int SKILL_LEAP_ATTACK_ID = 301003;

    #endregion

    #region Path

    public const string CREATURE_PATH = "Prefabs/Creatures";
    public const string ITEM_OBJECT_PATH = "Prefabs/Interactables/ItemObjects";
    public const string ITEM_PATH = "Items";
    public const string BGM_PATH = "Sounds/Bgms";
    public const string EFFECT_PATH = "Sounds/Effects";

    #endregion

    #region Value

    public const int PLAYER_COUNT = 2;
    public const int MAX_ITEM_NUM = 4;
    public const int MAX_SKILL_NUM = 4;

    public const float PASIVE_RECOVER_STAMINA = 1.5f;
    public const float DAMAGED_RECOVER_STAMINA = 60f;
    public const float RUN_USE_STAMINA = 5f;
    public const float EROSION_REDUCE_SANITY = 1f;
    public const float SIT_RECOVER_SANITY = 0.5f;

    public const int BATTERY_CHARGE_GOAL = 2;
    public const int USBKEY_INSERT_GOAL = 2;

    #endregion
}
