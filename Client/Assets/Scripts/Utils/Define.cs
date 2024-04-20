public static class Define
{
    #region Type

    public enum CreatureType
    {
        None,
        Crew,
        Alien,
    }

    public enum ItemType
    {
        None,
        Battery,
        Medicine,
    }

    public enum SectorName
    {
        F1_HallwayA,
        MainRoom,
        OperationRoom,
        ReactorRoom,
        TubeRoom,
        F2_HallwayA,
        F2_SmallRoom
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

    public enum AnimState
    {
        Idle,
        Move,
        Use,
        Dead,
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

    public const int ITEM_Battery_ID = 201000;
    public const int ITEM_Bandage_ID = 202000;
    public const int ITEM_Morphine_ID = 202001;

    public const int SKILL_BASIC_ATTACK_ID = 301000;
    public const int SKILL_ROAR_ID = 301001;
    public const int SKILL_CURSED_HOWL_ID = 301002;
    public const int SKILL_LEAP_ATTACK_ID = 301003;

    #endregion

    #region Path

    public const string CREATURE_PATH = "Prefabs/Creatures";
    public const string ITEM_OBJECT_PATH = "Prefabs/Map/ItemObjects";
    public const string ITEM_PATH = "Items";
    public const string Bgm_PATH = "Music/Bgm";
    public const string Effect_PATH = "Music/Clicks";

    #endregion

    #region Value

    public const int PLAYER_COUNT = 2;
    public const int MAX_ITEM_NUM = 4;
    public const int MAX_SKILL_NUM = 4;

    public const float PASIVE_RECOVER_STAMINA = 5f;
    public const float DAMAGED_RECOVER_STAMINA = 30f;
    public const float RUN_USE_STAMINA = 10f;
    public const float PASIVE_REDUCE_SANITY = 1f;
    public const float SIT_RECOVER_SANITY = 0.5f;

    public const int BATTERY_COLLECT_GOAL = 3;
    #endregion
}
