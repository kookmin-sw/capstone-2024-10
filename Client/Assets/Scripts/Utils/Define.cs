using System.Numerics;

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

    // public enum SceneType
    public enum SceneType
    {
        UnknownScene,
        LobbyScene,
        GameScene,
        Map_JSJ,
    }

    // public enum SoundType
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
        Hp,
        MaxHp,
        Attack,
        Defense,
        Speed,
        Strength,
        Intelligence,
        Vitality,
        Dexterity,
    }

    #endregion

    #region State
    public enum CreatureState
    {
        Idle,
        Move,
        UseItem,
        UseSkill,
        Dead,
    }

    public enum AnimState
    {
        Idle,
        Move,
        UseItem,
        UseSkill,
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

    // Name은 나중에 Json으로 관리
    #region Name
    public enum MonsterName
    {
        
    }

    public enum ItemName
    {
        
    }

    public enum SectorName
    {

    }
    #endregion

    // DataId는 나중에 Json으로 관리
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
