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
        Battery,
    }

    public enum SectorType
    {

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

    public enum CreatureState
    {
        Idle,
        Move,
        Interact,
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

    public const int PLAYER_COUNT = 2;
    public const int MAX_ITEM_NUM = 4;
    public const float PLAYER_SPAWN_POSITION_X = 20f;
    public const float PLAYER_SPAWN_POSITION_Y = 0.3f;
    public const float PLAYER_SPAWN_POSITION_Z = 10f;

    public const float GRAVITY_VALUE = -9.81f;
    public const int PASIVE_RECOVER_STAMINA = 5;
    public const int RUN_USE_STAMINA = 10;

    #endregion

    #region PlayerState
    public enum PlayerState
    {
        None,
        Ready,
    }
    #endregion
}
