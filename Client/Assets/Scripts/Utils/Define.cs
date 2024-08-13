public static class Define
{
    #region Type

    public enum CreatureType
    {
        None,
        Crew,
        Alien,
        TutoCrew,
        TutoAlien,
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
        Adrenaline,
        GameEnd,
    }

    public enum AlienActionType
    {
        None,
        GetBlind,
        CrashDoor,
        Hit,
        BasicAttack,
        ReadyRoar,
        Roar,
        ReadyCursedHowl,
        CursedHowl,
        ReadyLeapAttack,
        LeapAttack,
        HitDelay,
        GameEnd,
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
        TutorialScene,
    }

    public enum SoundType
    {
        Bgm,
        Environment,
        Effect,
        Facility,
        MaxCount,
    }

    public enum VolumeType
    {
        Master,
        Bgm,
        Environment,
        Effect,
        MaxCount,
    }

    #endregion

    #region State

    public enum PlayerState
    {
        None,
        Ready,
    }

    public enum CrewState
    {
        None,
        Alive,
        GameEnd,
        Disconnected,
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

    #region DataId

    public const int CREW_CREWA_ID = 101000;

    public const int ALIEN_STALKER_ID = 102000;

    public const int ITEM_BATTERY_ID = 201000;
    public const int ITEM_USBKEY_ID = 201001;
    public const int ITEM_CARDKEY_ID = 201002;
    public const int ITEM_BANDAGE_ID = 202000;
    public const int ITEM_ADRENALINE_ID = 202001;
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
    public const string FACILITY_PATH = "Sounds/Facilities";
    public static string AUDIO_MIXER_PATH = "Sounds/AudioMixer";

    #endregion

    #region Value

    public const float GAME_WAIT_TIME = 20.0f;
    public const float EXIT_TIME = 10.0f;

    public const int PLAYER_COUNT = 4;
    public const int MAX_ITEM_NUM = 4;
    public const int MAX_SKILL_NUM = 4;

    public const int BATTERY_CHARGE_GOAL = 9;
    public const int USBKEY_INSERT_GOAL = 6;
    public const int OPEN_PANIC_ROOM = 2;
    public const int TUTORIAL_BATTERY_CHARGE_GOAL = 2;

    #endregion

    #region Text

    public static string[] TEXT_FOR_TIP =
    {
        "Crew: Crouch to pick up the things on the floor.",
        "Crew: Close the door to run away from Stalker.",
        "Crew: You can recover sanity by crouching.",
        "Crew: Crouching makes your stamina recover faster.",
        "Crew: While Running, Your footsteps become much louder.",
        "Crew: While Crouching, Your footsteps disappear.",
        "Crew: After being attacked by Stalker, your speed increases for a moment.",
        "Stalker: Use the \"Roar\" skill to slow down the crew.",
        "Stalker: Destroy the closed door so the crew can never use it again.",
    };

    #endregion
}
