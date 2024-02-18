public class Define
{
    #region Type
    public enum CreatureType
    {
        None,
        Crew,
        Monster,
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
        Wait,
        Action,
        Dead
    }

    public enum AnimState
    {

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

    // Name은 나중에 Json으로 관리
    #region Name
    public enum AreaName
    {
        Forest,
    }

    public enum MonsterName
    {
        Slime,
    }

    public enum ItemName
    {
        Sword,
    }
    #endregion

    // DataId는 나중에 Json으로 관리
    #region DataId
    public const int CREW_AAAA_ID = 101000;

    public const int ALIEN_BBBB_ID = 102000;

    public const int ITEM_CCCC_ID = 201000;

    public const int SKILL_DDDD_ID = 401000;
    #endregion

    #region Path
    public const string CREW_PATH = "Crews";
    public const string ALIEN_PATH = "Aliens";
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

    public const int PLAYER_COUNT = 3;
    #endregion
}
