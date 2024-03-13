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
        Attack,
        Buff,
        Debuff,
        Recover,
    }

    public enum SectorType
    {

    }

    public enum SceneType
    {
        UnknownScene,
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

    #region Attribute
    public enum Stat
    {
        Name,
        WalkSpeed,
        RunSpeed,
        Hp,
        MaxHp,
        Stamina,
        SitSpeed,
        Damage,
    }

    #endregion

    #region State

    public enum CreatureState
    {
        Idle,
        Move,
        Use,
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
    public const int CREW_CREWB_ID = 101001;

    public const int ALIEN_STALKER_ID = 102000;

    public const int ITEM_ITEMA_ID = 201000;

    #endregion

    #region Path

    public const string CREW_PATH = "Prefabs/Crews";
    public const string ALIEN_PATH = "Prefabs/Aliens";

    #endregion

    #region Value

    public const int PLAYER_COUNT = 3;
    public const float PLAYER_SPAWN_POSITION_X = 20f;
    public const float PLAYER_SPAWN_POSITION_Y = 0.3f;
    public const float PLAYER_SPAWN_POSITION_Z = 10f;

    public const float GRAVITY_VALUE = -9.81f;

    #endregion
}
